using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createdBallScript : MonoBehaviour
{
    [SerializeField] gameManager manager;
    public int ballIDY;
    public int ballIDX;
    public bool checkedForRemoval = false;
    public int materialIndex = -1;
    public bool dragDown = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SphereCollider>().material.bounciness = 0;
    }

    // Update is called once per frame
    void Update()
    {
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
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = 10;
            rb.freezeRotation = false;
            rb.constraints = ~RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<throwScript>() != null && collision.gameObject.GetComponent<throwScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (collision.gameObject.GetComponent<createdBallScript>() != null && collision.gameObject.GetComponent<createdBallScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (collision.gameObject.GetComponent<CapsuleCollider>() != null && dragDown)
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
