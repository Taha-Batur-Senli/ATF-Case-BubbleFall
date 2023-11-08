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
    [SerializeField] private GameObject line;
    [SerializeField] private gameManager manager;

    private Vector3 startPosition;
    private GameObject collidedWith;

    bool sentinel = false;
    bool isShot = false;
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = manager.matsToGive[UnityEngine.Random.Range(0, manager.matsToGive.Length)];
        startPosition = transform.position;
        line.SetActive(false);
    }

    private void OnMouseUp()
    {
        Ray ray = mainCam.ScreenPointToRay( Input.mousePosition );
        if(Physics.Raycast(ray, out RaycastHit raycasthit ) && !isShot)
        {
            if (raycasthit.collider.gameObject.GetComponent<MeshRenderer>().material.name != "Ground (Instance)" && raycasthit.collider.gameObject.transform.position.x != startPosition.x && raycasthit.collider.gameObject.transform.position.z != startPosition.z)
            {
                line.SetActive(true);
                Vector3 targetPosition = new Vector3(raycasthit.point.x, transform.position.y, raycasthit.point.z);
                line.GetComponent<lineScript>().endPos = targetPosition;
                StartCoroutine(LerpPosition(targetPosition, 1));
                isShot = true;
            }
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;

        while (time < duration && !sentinel)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if(!sentinel)
        {
            transform.position = targetPosition;
            //goUntilHit(targetPosition);
        }
        createThrow(startPosition);
        line.SetActive(false);
    }

    private void goUntilHit(Vector3 endDir)
    {
        Vector3 direction = (endDir - startPosition).normalized;
        if (Physics.SphereCast(startPosition, manager.ballWidth, direction, out RaycastHit hit))
        {
            if(hit.collider.name == "SphereCollider")
            {
                Debug.Log("ss");
            }
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
                manager.callHit(collision.gameObject, gameObject);
            }
            collided = true;
        }
    }

    public void createThrow(Vector3 startPos)
    {
        GameObject newOne = Instantiate(gameObject);
        newOne.transform.position = startPos;
        line.GetComponent<lineScript>().startPos = newOne.transform;
    }
}
