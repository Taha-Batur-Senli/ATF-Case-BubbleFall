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
    public GameObject toRight = null;
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
            Destroy(gameObject);
        }

        if(dragDown)
        {
            manager.locationIndices[ballIDY + manager.emptyRowCount][ballIDX] = -1;
            manager.createdBalls[ballIDY + manager.emptyRowCount][ballIDX] = null;

            Physics.IgnoreCollision(manager.preventor.GetComponent<Collider>(), GetComponent<Collider>(), false);
            Physics.IgnoreCollision(manager.ignoreWhenFalling.GetComponent<Collider>(), GetComponent<Collider>());
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = 10;
            rb.freezeRotation = false;
            rb.constraints = ~RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.Equals(manager.preventor) && dragDown)
        {
            Vector3 targetPos;

            if (transform.position.x > 0)
            {
                targetPos = transform.position + new Vector3(10, 0, 0);
            }
            else
            {
                targetPos = transform.position - new Vector3(10, 0, 0);
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, 3);
        }
    }
}
