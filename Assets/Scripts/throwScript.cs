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

    public bool isShot = false;
    public bool dragDown = false;
    bool doOnce = false;
    public bool sentinel = false;
    public int matID;
    public int placedX;
    public int placedY;

    // Start is called before the first frame update
    void Start()
    {
        placedX = -1;
        placedY = -1;
        dragDown = false;
        collidedWith = null;
        collidedWithRegardless = null;
        isShot = false;
        doOnce = false;
        collisionCount = 0;
        sentinel = false;
        matID = UnityEngine.Random.Range(0, manager.matsToGive.Length);

        GetComponent<MeshRenderer>().material = manager.matsToGive[matID];
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
        if (Input.GetMouseButton(0) && !manager.gameOver.gameObject.activeSelf)
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
        if (collision.collider.GetType() == typeof(SphereCollider))
        {
            if(!doOnce)
            {
                collidedWithRegardless = collision.gameObject;
                manager.placeBall(collidedWithRegardless, gameObject);
                doOnce = true;
            }
        }
    }
}
