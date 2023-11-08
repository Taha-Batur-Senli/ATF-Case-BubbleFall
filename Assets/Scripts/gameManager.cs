using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject thrownBall;
    [SerializeField] GameObject hitBallTemplate;
    [SerializeField] public GameObject line;
    [SerializeField] public Material[] matsToGive;

    //[SerializeField] public int amountToCreate;
    [SerializeField] public int[] amountOnEachRow;
    [SerializeField] public int cullChance;
    [SerializeField] public int ballWidth = 7;
    [SerializeField] public int ballHeight = 7;

    [SerializeField] public float distanceToFloor = -196.67f;
    [SerializeField] public int maxNumberOfBallsInRow = 7;

    private List<List<GameObject>> createdBalls = new List<List<GameObject>>();

    [SerializeField] public float widthLow;
    [SerializeField] public float heightLow;
    // [SerializeField] public float widthHigh;
    // [SerializeField] public float heightHigh;

    // Start is called before the first frame update
    void Start()
    {
        int matOfBall;
        int count = 0;

        while (count < amountOnEachRow.Length)
        {
            createdBalls.Add(new List<GameObject>());

            if (amountOnEachRow[count] > maxNumberOfBallsInRow || amountOnEachRow[count] < 0)
            {
                Debug.Log("Incorrect Number!");
                amountOnEachRow[count] = maxNumberOfBallsInRow;
            }
           
            for (int a = 0; a < amountOnEachRow[count]; a++)
            {
                GameObject createdShift = Instantiate(hitBallTemplate);
                createdShift.transform.position = new Vector3(widthLow + (ballWidth * a), distanceToFloor, heightLow + (ballHeight * count));
                matOfBall = UnityEngine.Random.Range(0, matsToGive.Length);
                createdShift.GetComponent<MeshRenderer>().material = matsToGive[matOfBall];
                createdShift.GetComponent<createdBallScript>().ballIDY = count;
                createdShift.GetComponent<createdBallScript>().ballIDX = a;
                createdShift.GetComponent<createdBallScript>().materialIndex = matOfBall;
                createdBalls[count].Add(createdShift);
            }

            count++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callHit(GameObject target, GameObject hitter)
    {
        Debug.Log("hit called");

        if(target.GetComponent<throwScript>() != null)
        {
            Debug.Log("hit a previously thrown ball");
        }
        else
        {
            Debug.Log("hit a generated ball");
            List<GameObject> savedPositions = new List<GameObject>();
            savedPositions.Add(target);
            savedPositions.Add(hitter);
            int count = 2;
            target.GetComponent<createdBallScript>().checkedForRemoval = true;
            int targetX = target.GetComponent<createdBallScript>().ballIDX;
            int targetY = target.GetComponent<createdBallScript>().ballIDY;

            int indexOfTarget = target.GetComponent<createdBallScript>().materialIndex;

            //Check Right
            if (createdBalls[targetY].Count - 1 >= targetX + 1 && indexOfTarget == createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                Debug.Log("Right included");
                savedPositions.Add(createdBalls[targetY][targetX + 1]);
                createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            //Check Left
            if (0 <= targetX - 1 && indexOfTarget == createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                Debug.Log("Left included");
                savedPositions.Add(createdBalls[targetY][targetX - 1]);
                createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            Debug.Log(createdBalls.Count);
            //Check Top
            if (createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX && indexOfTarget == createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                Debug.Log("Top included");
                savedPositions.Add(createdBalls[targetY + 1][targetX]);
                createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            Debug.Log(targetY - 1);
            //Check Down
            if (0 <= targetY - 1 && createdBalls[targetY - 1].Count - 1 >= targetX && indexOfTarget == createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                Debug.Log("Down included");
                savedPositions.Add(createdBalls[targetY - 1][targetX]);
                createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            if(count >= 3)
            {
                foreach (var item in savedPositions)
                {
                    item.SetActive(false);
                }
                line.SetActive(false);
                createThrow(thrownBall.GetComponent<throwScript>().startPosition);
            }

            /*
            //Check Corners
            if ()
            {

                count++;
            }

            if ()
            {

                count++;
            }

            if ()
            {

                count++;
            }

            if ()
            {

                count++;
            }*/
        }

    }

    public int checkAround(int count, int indexOfTarget)
    {
        

        return count;
    }
    public void createThrow(Vector3 startPos)
    {
        GameObject newOne = Instantiate(thrownBall);
        newOne.transform.position = startPos;
        line.GetComponent<lineScript>().startPos = newOne.transform;
    }
}
