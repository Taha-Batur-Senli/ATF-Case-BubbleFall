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
    public bool hasOneUp = false;
    public GameObject toLeft = null;
    GameObject collidedBefore = null;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SphereCollider>().material.bounciness = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasOneUp && toLeft != null && toLeft.GetComponent<createdBallScript>().dragDown)
        {
            dragDown = true;
        }
        else if(hasOneUp && manager.createdBalls[ballIDY + 1][ballIDX].GetComponent<createdBallScript>().dragDown)
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.Equals(manager.ignoreWhenFalling) && dragDown)
        {
            gameObject.GetComponent<SphereCollider>().material.bounciness = 0;
            Physics.IgnoreCollision(manager.ignoreWhenFalling.GetComponent<Collider>(), GetComponent<Collider>());
        }

        if (collision.collider.GetType() == typeof(SphereCollider) && collision.gameObject.GetComponent<throwScript>() != null && !collision.gameObject.GetComponent<throwScript>().dragDown && !collision.gameObject.GetComponent<throwScript>().sentinel)
        {
            if (collision.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(GetComponent<MeshRenderer>().material.name))
            {
                if (collidedBefore != null)
                {
                    collision.gameObject.GetComponent<throwScript>().dragDown = true;
                    dragDown = true;
                    collidedBefore.GetComponent<throwScript>().dragDown = true;
                }
                else
                {
                    collidedBefore = collision.gameObject;
                }
            }

            if (collision.transform.position.z > transform.position.z)
            {
                if (ballIDY + 1 <= manager.amountOnEachRow.Length && manager.locationIndices[ballIDY + 1][ballIDX] == -1)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x, collision.gameObject.transform.position.y, transform.position.z + manager.ballHeight);
                }
                else if (collision.transform.position.x > transform.position.x && ballIDX + 1 <= manager.maxNumberOfBallsInRow && manager.locationIndices[ballIDY][ballIDX + 1] == -1)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x + manager.ballWidth, collision.gameObject.transform.position.y, transform.position.z);
                }
                else if (collision.transform.position.x < transform.position.x && ballIDX - 1 >= 0 && manager.locationIndices[ballIDY][ballIDX - 1] == -1)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x - manager.ballWidth, collision.gameObject.transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (ballIDY - 1 >= 0 && manager.locationIndices[ballIDY - 1][ballIDX] == -1)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x, collision.gameObject.transform.position.y, transform.position.z - manager.ballHeight);
                }
                else if (collision.transform.position.x > transform.position.x && ballIDX + 1 <= manager.maxNumberOfBallsInRow && manager.locationIndices[ballIDY][ballIDX + 1] == -1)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x + manager.ballWidth, collision.gameObject.transform.position.y, transform.position.z);
                }
                else if (collision.transform.position.x < transform.position.x && ballIDX - 1 >= 0 && manager.locationIndices[ballIDY][ballIDX - 1] == -1)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x - manager.ballWidth, collision.gameObject.transform.position.y, transform.position.z);
                }
            }

            collision.gameObject.GetComponent<throwScript>().sentinel = true;

            /*Ray rayLeft = new Ray(this.transform.position, new Vector3(transform.position.x - manager.ballWidth, transform.position.y, transform.position.z));
            Ray rayRight = new Ray(this.transform.position, new Vector3(transform.position.x + manager.ballWidth, transform.position.y, transform.position.z));
            Ray rayUp = new Ray(this.transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + manager.ballHeight));
            Ray rayDown = new Ray(this.transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - manager.ballHeight));

            bool leftFree = true;
            bool rightFree = true;
            bool upFree = true;
            bool downFree = true;

            if (Physics.Raycast(rayLeft, out RaycastHit raycasthit1))
            {
                leftFree = false;
            }
            else if (Physics.Raycast(rayRight, out RaycastHit raycasthit2))
            {
                rightFree = false;
            }
            else if (Physics.Raycast(rayUp, out RaycastHit raycasthit3))
            {
                upFree = false;
            }
            else if (Physics.Raycast(rayDown, out RaycastHit raycasthit4))
            {
                downFree = false;
            }

            if(collision.transform.position.z > transform.position.z)
            {
                if(upFree)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x, collision.gameObject.transform.position.y, transform.position.z + manager.ballHeight);
                }
                else if (collision.transform.position.x > transform.position.x && rightFree)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x + manager.ballHeight, collision.gameObject.transform.position.y, transform.position.z);
                }
                else if(collision.transform.position.x < transform.position.x && leftFree) 
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x - manager.ballHeight, collision.gameObject.transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (downFree)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x, collision.gameObject.transform.position.y, transform.position.z - manager.ballHeight);
                }
                else if (collision.transform.position.x > transform.position.x && rightFree)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x + manager.ballHeight, collision.gameObject.transform.position.y, transform.position.z);
                }
                else if (collision.transform.position.x < transform.position.x && leftFree)
                {
                    collision.gameObject.transform.position = new Vector3(transform.position.x - manager.ballHeight, collision.gameObject.transform.position.y, transform.position.z);
                }
            }

            collision.gameObject.GetComponent<throwScript>().sentinel = true;
        }*/

        }

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


        if (collision.gameObject.GetComponent<throwScript>() != null && collision.gameObject.GetComponent<throwScript>().dragDown == true)
        {
            dragDown = true;
        }

        if (collision.gameObject.GetComponent<createdBallScript>() != null && collision.gameObject.GetComponent<createdBallScript>().dragDown == true)
        {
            dragDown = true;
        }
    }

    private void isItFree()
    {

    }
}
