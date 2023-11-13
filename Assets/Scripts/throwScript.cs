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
    public int collisionCount;

    bool sentinel = false;
    bool isShot = false;
    bool collided = false;
    public bool dragDown = false;

    // Start is called before the first frame update
    void Start()
    {
        collidedWith = null;
        GetComponent<MeshRenderer>().material = manager.matsToGive[UnityEngine.Random.Range(0, manager.matsToGive.Length)];
        startPosition = transform.position;
        manager.line.SetActive(false);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.constraints = RigidbodyConstraints.FreezePositionX;
        rb.mass = 1;
        rb.freezeRotation = true;
        rb.useGravity = false;
        gameObject.GetComponent<SphereCollider>().material.bounciness = 0;
    }

    private void Update()
    {
        if (dragDown && isShot)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = 10;
            rb.freezeRotation = false;
            rb.constraints = ~RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = true;
        }
        else
        {
            dragDown = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
            rb.mass = 1;
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        if (Input.GetMouseButtonDown(0) && !manager.gameOver.gameObject.activeSelf)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycasthit) && !isShot)
        {
            if (raycasthit.collider.gameObject.GetComponent<MeshRenderer>().material.name != "Ground (Instance)" && raycasthit.collider.gameObject.transform.position.x != startPosition.x && raycasthit.collider.gameObject.transform.position.z != startPosition.z)
            {
                manager.throwReady = false;
                isShot = true;
                Vector3 targetPosition = new Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
                manager.line.GetComponent<lineScript>().endPos = targetPosition;
                manager.line.SetActive(true);
                manager.line.GetComponent<lineScript>().getShot = true;
                StartCoroutine(LerpPosition(targetPosition, 1));
            }
        }
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
        if (collision.collider.GetType() == typeof(SphereCollider) && !collided && isShot)
        {
            sentinel = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            if (collision.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(GetComponent<MeshRenderer>().material.name) )
            {
                collidedWith = collision.gameObject;
                collisionCount = manager.callHit(collision.gameObject, gameObject);
                //Debug.Log(collidedWith.GetComponent<MeshRenderer>().material);
            }
            collided = true;
        }

        if(collision.gameObject.GetComponent<throwScript>() != null && collision.gameObject.GetComponent<throwScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (collision.gameObject.GetComponent<createdBallScript>() != null && collision.gameObject.GetComponent<createdBallScript>().dragDown == true)
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
                targetPos = transform.position + new Vector3(7, 0, 0);
            }
            else
            {
                targetPos = transform.position - new Vector3(7, 0, 0);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, 3);
        }
    }
}
