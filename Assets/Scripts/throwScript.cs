using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = manager.matsToGive[UnityEngine.Random.Range(0, manager.matsToGive.Length)];
        startPosition = transform.position;
        manager.line.SetActive(false);
    }

    private void OnMouseUp()
    {
        manager.throwReady = false;
        Ray ray = mainCam.ScreenPointToRay( Input.mousePosition );
        if(Physics.Raycast(ray, out RaycastHit raycasthit ) && !isShot)
        {
            if (raycasthit.collider.gameObject.GetComponent<MeshRenderer>().material.name != "Ground (Instance)" && raycasthit.collider.gameObject.transform.position.x != startPosition.x && raycasthit.collider.gameObject.transform.position.z != startPosition.z)
            {
                manager.line.SetActive(true);
                Vector3 targetPosition = new Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
                manager.line.GetComponent<lineScript>().endPos = targetPosition;
                StartCoroutine(LerpPosition(targetPosition, 1));
                isShot = true;
            }
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        //targetPosition.x = targetPosition.x * manager.backMostRowZ;
        //targetPosition.z = manager.backMostRowZ;
        float time = 0;

        while (time < duration && !sentinel)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            //Vector3 pos = targetPosition - startPosition;
            //transform.position += pos * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }
        if(!manager.throwReady)
        {
            manager.createThrow(startPosition);
        }
        manager.line.SetActive(false);
        
        if(transform.position.z < manager.endGameOnZ)
        {
            manager.gameOver.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetType() == typeof(SphereCollider))
        {
            sentinel = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            if (!collided && collision.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(GetComponent<MeshRenderer>().material.name) )
            {
                collidedWith = collision.gameObject;
                collisionCount = manager.callHit(collision.gameObject, gameObject);
            }
            collided = true;
        }
    }
}
