using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class throwScript : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private gameManager manager;

    public GameObject collidedWith;
    public bool isShot = false;
    public int matID;

    //Regular Speed: 150
    private int speed = 150;
    private bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        collidedWith = null;
        isShot = false;
        doOnce = false;
        matID = UnityEngine.Random.Range(0, manager.matLen);

        GetComponent<MeshRenderer>().material = manager.matsToGive[matID];
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
        UnityEngine.Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);
        if (!(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0))
        {
            manager.createThrow(manager.startpos);
            Destroy(gameObject);
        }

        if (!isShot && Input.GetMouseButton(0) && !manager.gameOver.gameObject.activeSelf && !EventSystem.current.IsPointerOverGameObject())
        {
            getLoc();
            manager.line.GetComponent<lineScript>().getShot = true;
            manager.line.SetActive(true);
        }

        if (!isShot && Input.GetMouseButtonUp(0) && !manager.gameOver.gameObject.activeSelf && !EventSystem.current.IsPointerOverGameObject())
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
            UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
            manager.line.GetComponent<lineScript>().endPos = targetPosition;
        }
    }

    private void Shoot()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycasthit) && !isShot)
        {
            // Get the direction from this object to the point where the ray hit
            UnityEngine.Vector3 direction = raycasthit.point - transform.position;

            // Normalize the direction vector to get a unit vector
            direction.Normalize();
            isShot = true;
            UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
            manager.line.GetComponent<lineScript>().endPos = targetPosition;
            manager.line.SetActive(false);
            ShootBall(direction);
        }
    }

    public void ShootBall(UnityEngine.Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetType() == typeof(SphereCollider) && !collision.collider.GetComponent<createdBallScript>().dragDown && !doOnce)
        {
            collidedWith = collision.gameObject;
            manager.placeBall(collidedWith, gameObject);
            doOnce = true;
        }

        if (collision.collider.name.Equals("BackCube"))
        {
            int pos = (int) transform.position.x / manager.maxNumberOfBallsInRow;
            int mid = manager.maxNumberOfBallsInRow / 2;

            manager.generateForThrown(mid + pos, manager.totalRowCount.Length - 1, manager.widthLow + (manager.ballWidth * (pos + mid)), manager.heightLow + (manager.ballHeight * (manager.levelData.rowCount - 1)), gameObject, true);
            manager.createThrow(manager.startpos);
            Destroy(gameObject);
        }
    }
}
