using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createdBallScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callHit()
    {
        if (Physics.Raycast(gameObject.transform.position, Vector3.forward, out RaycastHit raycasthitF))
        {
            if (raycasthitF.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(gameObject.GetComponent<MeshRenderer>().material.name))
            {
                Debug.Log("ss");
            }
        }

        if (Physics.Raycast(gameObject.transform.position, Vector3.left, out RaycastHit raycasthitL))
        {
            if (raycasthitL.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(gameObject.GetComponent<MeshRenderer>().material.name))
            {
                Debug.Log("ss2");
            }
        }

        if (Physics.Raycast(gameObject.transform.position, Vector3.right, out RaycastHit raycasthitR))
        {
            if (raycasthitR.collider.gameObject.GetComponent<MeshRenderer>().material.name.Equals(gameObject.GetComponent<MeshRenderer>().material.name))
            {
                Debug.Log("ss3");
            }
        }
    }
}
