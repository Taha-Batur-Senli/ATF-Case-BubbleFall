using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class lineScript : MonoBehaviour
{
    [SerializeField] public Transform startPos;
    [SerializeField] public gameManager manager;
    public bool getShot = false;

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
        if (getShot)
        {
            rend.SetPosition(0, startPos.position);
            rend.SetPosition(1, endPos);
            rend.startWidth = 0.5f;
            rend.endWidth = 0.5f;
            gameObject.SetActive(true);
        }
    }
}
