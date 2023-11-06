using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject thrownBall;
    [SerializeField] private GameObject line;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void createThrow(Vector3 startPos)
    {
        GameObject newOne = Instantiate(thrownBall);
        newOne.transform.position = startPos;
        line.GetComponent<lineScript>().startPos = newOne.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
