using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class throwScript : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private gameManager manager;
    public Vector3 startPosition;

    public GameObject collidedWith;
    public GameObject collidedWithRegardless;
    public int collisionCount;

    bool fellOnce = false;
    public bool isShot = false;
    bool collided = false;
    public bool dragDown = false;
    bool doOnce = false;
    public bool sentinel = false;

    // Start is called before the first frame update
    void Start()
    {
        dragDown = false;
        collidedWith = null;
        collidedWithRegardless = null;
        collided = false;
        fellOnce = false;
        isShot = false;
        doOnce = false;
        collisionCount = 0;
        sentinel = false;

        GetComponent<MeshRenderer>().material = manager.matsToGive[UnityEngine.Random.Range(0, manager.matsToGive.Length)];
        startPosition = transform.position;
        manager.line.SetActive(false);
        Physics.IgnoreCollision(manager.preventor.GetComponent<Collider>(), GetComponent<Collider>());
        Physics.IgnoreCollision(manager.ignoreWhenFalling.GetComponent<Collider>(), GetComponent<Collider>(), false);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        rb.useGravity = false;
        gameObject.GetComponent<SphereCollider>().material.bounciness = 0.8f;
    }

    private void Update()
    {
        if(!isShot)
        {
            dragDown = false;
        }

        if (isShot && collidedWithRegardless != null && collidedWithRegardless.GetComponent<createdBallScript>() != null && collidedWithRegardless.GetComponent<createdBallScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (isShot && collidedWithRegardless != null && collidedWithRegardless.GetComponent<throwScript>() != null && collidedWithRegardless.GetComponent<throwScript>().dragDown == true)
        {
            dragDown = true;
        }

        if(collisionCount >= 3)
        {
            dragDown = true;
        }

        if (collided && transform.position.z < manager.endGameOnZ && !fellOnce)
        {
            manager.gameOver.SetActive(true);
        }

        if (transform.position.z < manager.zLim)
        {
            dragDown = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
            rb.mass = 1;
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        if (dragDown && isShot)
        {
            Physics.IgnoreCollision(manager.preventor.GetComponent<Collider>(), GetComponent<Collider>(), false);
            fellOnce = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = 10;
            rb.freezeRotation = false;
            rb.constraints = ~RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = true; 
        }
        else
        {
            if (collided)
            {
                dragDown = false;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
                rb.mass = 1;
                rb.freezeRotation = true;
                rb.useGravity = false;
            }
        }

        if(Input.GetMouseButton(0) && !manager.gameOver.gameObject.activeSelf)
        {
            getLoc();
            manager.line.GetComponent<lineScript>().getShot = true;
            manager.line.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0) && !manager.gameOver.gameObject.activeSelf)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = 1;
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            Shoot();
        }
    }

    public void getLoc()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycasthit))
        {
            Vector3 targetPosition = new Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
            manager.line.GetComponent<lineScript>().endPos = targetPosition;
        }
    }

    private void Shoot()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycasthit) && !isShot)
        {
            // Get the direction from this object to the point where the ray hit
            Vector3 direction = raycasthit.point - transform.position;

            // Normalize the direction vector to get a unit vector
            direction.Normalize();
            isShot = true;
            Vector3 targetPosition = new Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
            manager.line.GetComponent<lineScript>().endPos = targetPosition;
            manager.line.SetActive(false);
            ShootBall(direction);
        }
    }

    public void ShootBall(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * 150;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.Equals(manager.ignoreWhenFalling) && dragDown)
        {
            gameObject.GetComponent<SphereCollider>().material.bounciness = 0;
            Physics.IgnoreCollision(manager.ignoreWhenFalling.GetComponent<Collider>(), GetComponent<Collider>());
        }

        if (collision.gameObject.Equals(manager.preventor) && dragDown)
        {
            Vector3 targetPos;

            if (transform.position.x > 0 )
            {
                targetPos = transform.position + new Vector3(10, 0, 0);
            }
            else
            {
                targetPos = transform.position - new Vector3(10, 0, 0);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, 3);
        }

        if (collision.collider.GetType() == typeof(SphereCollider))
        {
            if(!doOnce)
            {
                collidedWithRegardless = collision.gameObject;
                doOnce = true;
            }

            if (!collided && isShot)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

                if (collision.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(GetComponent<MeshRenderer>().material.name))
                {
                    collidedWith = collision.gameObject;
                    collisionCount = manager.callHit(collision.gameObject, gameObject);
                }

                manager.throwReady = false;
                collided = true;
            }
        }

        if (isShot && collision.gameObject.GetComponent<throwScript>() != null && collision.gameObject.GetComponent<throwScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (isShot && collision.gameObject.GetComponent<createdBallScript>() != null && collision.gameObject.GetComponent<createdBallScript>().dragDown == true)
        {
            dragDown = true;
        }
    }
}
