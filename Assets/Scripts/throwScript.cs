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

    bool sentinel = false;
    bool fellOnce = false;
    public bool isShot = false;
    bool collided = false;
    public bool dragDown = false;
    private int speedTerm = 30;

    // Start is called before the first frame update
    void Start()
    {
        collidedWith = null;
        collidedWithRegardless = null;
        GetComponent<MeshRenderer>().material = manager.matsToGive[UnityEngine.Random.Range(0, manager.matsToGive.Length)];
        startPosition = transform.position;
        manager.line.SetActive(false);
        Physics.IgnoreCollision(manager.preventor.GetComponent<Collider>(), GetComponent<Collider>());
        Physics.IgnoreCollision(manager.ignoreWhenFalling.GetComponent<Collider>(), GetComponent<Collider>(), false);
        isShot = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        /* rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.constraints = RigidbodyConstraints.FreezePositionX;
        rb.mass = 1;
        rb.freezeRotation = true;*/
        rb.useGravity = false;
        gameObject.GetComponent<SphereCollider>().material.bounciness = 1;
    }

    private void Update()
    {
        if (collidedWithRegardless != null && collidedWithRegardless.GetComponent<createdBallScript>() != null && collidedWithRegardless.GetComponent<createdBallScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (collidedWithRegardless != null && collidedWithRegardless.GetComponent<throwScript>() != null && collidedWithRegardless.GetComponent<throwScript>().dragDown == true)
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
            Debug.Log(manager.line.GetComponent<lineScript>().endPos);
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
            /*
             * 
           if (raycasthit.collider.gameObject.GetComponent<MeshRenderer>().material.name != "Ground (Instance)" && raycasthit.collider.gameObject.transform.position.z != startPosition.z)
           {

               StartCoroutine(LerpPosition(targetPosition, 1));
           }
             */
        }
    }

    public void ShootBall(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * 150;
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        // float time = 0;

        var dist = Vector3.Distance(startPosition, targetPosition);

        for (float i = 0.0f; i < 1.0; i += (175 * Time.deltaTime) / dist)
        {
            if (sentinel)
            {
                i = 1f;
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, i);
            }

            yield return null;
        }

       /* while (time < duration && !sentinel)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time * 2 / duration);
            time += Time.deltaTime;
            yield return null;
        }*/

        if (!manager.throwReady)
        {
            manager.createThrow(startPosition);
        }
        manager.line.SetActive(false);

        Debug.Log(transform.position.z);
        Debug.Log(manager.endGameOnZ);

        if (transform.position.z < manager.endGameOnZ && !dragDown)
        {
            manager.gameOver.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.Equals(manager.ignoreWhenFalling) && dragDown)
        {
            Physics.IgnoreCollision(manager.ignoreWhenFalling.GetComponent<Collider>(), GetComponent<Collider>());
        }

        if (collision.gameObject.Equals(manager.preventor) && dragDown)
        {
            Vector3 targetPos;

            if (transform.position.x > 0 )
            {
                targetPos = transform.position + new Vector3(8, 0, 0);
            }
            else
            {
                targetPos = transform.position - new Vector3(8, 0, 0);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, 3);
        }

        if (collision.collider.GetType() == typeof(SphereCollider))
        {
            collidedWithRegardless = collision.gameObject;

            /*if(collision.gameObject.GetComponent<throwScript>() != null && !collision.gameObject.GetComponent<throwScript>().isShot)
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }
            else
            {/*/
            if (!collided && isShot)
                {
                    sentinel = true;
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

                    if (collision.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(GetComponent<MeshRenderer>().material.name))
                    {
                        collidedWith = collision.gameObject;
                        collisionCount = manager.callHit(collision.gameObject, gameObject);
                        //Debug.Log(collidedWith.GetComponent<MeshRenderer>().material);
                    }

                    manager.throwReady = false;
                    collided = true;
                }
            //}

        }

        if(isShot && collision.gameObject.GetComponent<throwScript>() != null && collision.gameObject.GetComponent<throwScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (isShot && collision.gameObject.GetComponent<createdBallScript>() != null && collision.gameObject.GetComponent<createdBallScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (collision.gameObject.GetComponent<CapsuleCollider>() != null && dragDown && isShot)
        {
            gameObject.GetComponent<SphereCollider>().material.bounciness = 1;

            int toWhere = UnityEngine.Random.Range(0, 4);

            Vector3 targetPos;

            if (toWhere < 2)
            {
                targetPos = transform.position + new Vector3(15, 0, 0);
            }
            else
            {
                targetPos = transform.position - new Vector3(15, 0, 0);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, 3);
        }
    }
}
