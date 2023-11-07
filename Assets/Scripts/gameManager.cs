using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject thrownBall;
    [SerializeField] GameObject hitBallTemplate;
    [SerializeField] public Material[] matsToGive;

    [SerializeField] public int amountToCreate;
    [SerializeField] public int cullChance;
    [SerializeField] public int ballWidth = 7;
    [SerializeField] public int ballHeight = 7;

    public const float distanceToFloor = -196.67f;

    [SerializeField] public float widthHigh;
    [SerializeField] public float widthLow;
    [SerializeField] public float heightLow;
    // [SerializeField] public float heightHigh;


    // Start is called before the first frame update
    void Start()
    {
        GameObject[] arr = new GameObject[amountToCreate];
        GameObject created = Instantiate(hitBallTemplate);
        int chance;

        created.transform.position = new Vector3(widthLow, distanceToFloor, heightLow);
        arr[0] = created;

        arr[0].GetComponent<MeshRenderer>().material = matsToGive[UnityEngine.Random.Range(0, matsToGive.Length)];

        for (int a = 1; a < amountToCreate; a++)
        {
            if(arr[a - 1].transform.position.x + ballWidth >= widthHigh)
            {
                GameObject createdShift = Instantiate(hitBallTemplate);
                createdShift.transform.position = new Vector3(arr[0].transform.position.x, arr[0].transform.position.y, arr[a - 1].transform.position.z + ballHeight);
                arr[a] = createdShift;
            }
            else
            {
                GameObject createdNew = Instantiate(hitBallTemplate);
                createdNew.transform.position = new Vector3(arr[a - 1].transform.position.x + ballWidth, arr[a - 1].transform.position.y, arr[a - 1].transform.position.z);
                arr[a] = createdNew;
            }
            arr[a].GetComponent<MeshRenderer>().material = matsToGive[UnityEngine.Random.Range(0, matsToGive.Length)];

            chance = UnityEngine.Random.Range(0, 10);

            if(chance < cullChance)
            {
                arr[a].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
