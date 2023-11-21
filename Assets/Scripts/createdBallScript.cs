using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class createdBallScript : MonoBehaviour
{
    [SerializeField] gameManager manager;
    public int ballIDY;
    public int ballIDX;
    public bool checkedForRemoval = false;
    public int materialIndex = -1;
    public bool dragDown = false;
    public GameObject oneUp = null;
    public GameObject toLeft = null;
    public bool belowFree = true;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SphereCollider>().material.bounciness = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (oneUp == null && toLeft != null && toLeft.GetComponent<createdBallScript>().dragDown)
        {
            dragDown = true;
        }
        else if(oneUp != null && oneUp.GetComponent<createdBallScript>().dragDown)
        {
            dragDown = true;
        }

        if(transform.position.z < manager.zLim)
        {
            dragDown = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
            rb.mass = 1;
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        if(dragDown)
        {
            manager.locationIndices[ballIDY][ballIDX] = -1;
            Physics.IgnoreCollision(manager.preventor.GetComponent<Collider>(), GetComponent<Collider>(), false);
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = 10;
            rb.freezeRotation = false;
            rb.constraints = ~RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = true;
        }
    }
}
