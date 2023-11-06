using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lineScript : MonoBehaviour
{
    [SerializeField] public Transform startPos;

    private LineRenderer rend;
    public Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<LineRenderer>();
        rend.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            rend.SetPosition(0, startPos.position);
            rend.SetPosition(1, endPos);
            rend.startWidth = 0.5f;
            rend.endWidth = 0.5f;
        }
    }

}
