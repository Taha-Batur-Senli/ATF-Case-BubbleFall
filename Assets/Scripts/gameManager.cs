using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject thrownBall;
    [SerializeField] GameObject hitBallTemplate;
    [SerializeField] public GameObject line;
    [SerializeField] public Material[] matsToGive;
    [SerializeField] public GameObject gameOver;

    public float backMostRowZ;

    [SerializeField] public int endGameOnZ;

    //[SerializeField] public int amountToCreate;
    [SerializeField] public int[] amountOnEachRow;
    [SerializeField] public int cullChance;
    [SerializeField] public int ballWidth = 7;
    [SerializeField] public int ballHeight = 7;

    [SerializeField] public float distanceToFloor = -196.67f;
    [SerializeField] public int maxNumberOfBallsInRow = 7;

    private List<List<GameObject>> createdBalls = new List<List<GameObject>>();
    public bool throwReady = true;

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
                //debug.Log("Incorrect Number!");
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

        backMostRowZ = heightLow + (ballHeight * count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int callHit(GameObject target, GameObject hitter, int prior = 0)
    {
        //debug.Log("hit called");
        //THERE IS A BUG HERE WHICH CAUSES PRIORLY COLLUDED OBJECT TO BE DELETED ALONGSIDE THE NEW ONE REGARDLESS OF COLOR,
        //PRIOR COLLIDED OBJECT IS A PREGEN, AND THE OTHER TWO THAT HIT EACH OTHER ARE IN AN UNRELATED POSITION BUT HAVE SAME COLOR, FIX!!

        if(target.GetComponent<throwScript>() != null)
        {
            //debug.Log("hit a previously thrown ball");
            if(target.GetComponent<throwScript>().collidedWith != null)
            {
                if(target.GetComponent<throwScript>().collidedWith.GetComponent<createdBallScript>() == null)
                {

                    Debug.Log("in A");
                    target.GetComponent<throwScript>().collidedWith.SetActive(false);
                    target.SetActive(false);
                    hitter.SetActive(false);
                    if(!throwReady)
                    {
                        Debug.Log("generating");
                        throwReady= true;
                        createThrow(thrownBall.GetComponent<throwScript>().startPosition);
                    }
                    return 0;
                }
                else
                {
                    Debug.Log("in B");
                    target.GetComponent<throwScript>().collisionCount++;

                    if (!throwReady)
                    {
                        Debug.Log("generating2");
                        throwReady = true;
                        createThrow(thrownBall.GetComponent<throwScript>().startPosition);
                    }

                    if (target.GetComponent<throwScript>().collisionCount >= 3)
                    {
                        hitter.gameObject.SetActive(false);
                        callHit(target.GetComponent<throwScript>().collidedWith, target, 1);
                    }
                    else
                    {
                        target.GetComponent<throwScript>().collisionCount--;
                    }

                    return target.GetComponent<throwScript>().collisionCount;
                }
            }
            else
            {
                //This one's previous value was 2 and it worked fine
                return 2;
            }
        }
        else
        {
            Debug.Log("in c");
            //debug.Log("hit a generated ball");
            List<GameObject> savedPositions = new List<GameObject>();
            savedPositions.Add(target);
            savedPositions.Add(hitter);
            int count = 2 + prior;
            target.GetComponent<createdBallScript>().checkedForRemoval = true;
            int targetX = target.GetComponent<createdBallScript>().ballIDX;
            int targetY = target.GetComponent<createdBallScript>().ballIDY;
            bool topInc = false, botInc = false, leftInc = false, rightInc = false;

            int indexOfTarget = target.GetComponent<createdBallScript>().materialIndex;

            //Check Right
            if (createdBalls[targetY].Count - 1 >= targetX + 1 && indexOfTarget == createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                //debug.Log("Right included");
                savedPositions.Add(createdBalls[targetY][targetX + 1]);
                createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
                rightInc = true;
            }

            //Check Left
            if (0 <= targetX - 1 && indexOfTarget == createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                //debug.Log("Left included");
                savedPositions.Add(createdBalls[targetY][targetX - 1]);
                createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
                leftInc = true;
            }

            //debug.Log(createdBalls.Count);
            //Check Top
            if (createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX && indexOfTarget == createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                //debug.Log("Top included");
                savedPositions.Add(createdBalls[targetY + 1][targetX]);
                createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
                topInc= true;
            }

            //Check Down
            if (0 <= targetY - 1 && createdBalls[targetY - 1].Count - 1 >= targetX && indexOfTarget == createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                //debug.Log("Down included");
                savedPositions.Add(createdBalls[targetY - 1][targetX]);
                createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
                botInc = true;
            }

            //Check Down Left
            if ((botInc || leftInc) && 0 <= targetY - 1 && 0 <= targetX - 1 && indexOfTarget == createdBalls[targetY - 1][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                savedPositions.Add(createdBalls[targetY - 1][targetX - 1]);
                createdBalls[targetY - 1][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            //Check Down Right
            if ((botInc || rightInc) && 0 <= targetY - 1 && createdBalls[targetY - 1].Count - 1 >= targetX + 1 && indexOfTarget == createdBalls[targetY - 1][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                savedPositions.Add(createdBalls[targetY - 1][targetX + 1]);
                createdBalls[targetY - 1][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }
                
            //Check Top Left
            if ((topInc || leftInc) && 0 <= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX - 1 && indexOfTarget == createdBalls[targetY + 1][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                savedPositions.Add(createdBalls[targetY + 1][targetX - 1]);
                createdBalls[targetY + 1][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            //Check Top Right
            if ((topInc || rightInc) && 0 <= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX + 1 && indexOfTarget == createdBalls[targetY + 1][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                savedPositions.Add(createdBalls[targetY + 1][targetX + 1]);
                createdBalls[targetY + 1][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }

            if (count >= 3)
            {
                foreach (var item in savedPositions)
                {
                    item.SetActive(false);
                }
                line.SetActive(false);
                count = 0;
            }
            else
            {
                foreach (var item in savedPositions)
                {
                    if(item.GetComponent<createdBallScript>() != null)
                    {
                        item.GetComponent<createdBallScript>().checkedForRemoval = false;
                    }
                }
            }

            if (!throwReady)
            {
                Debug.Log("generating3");
                throwReady = true;
                createThrow(thrownBall.GetComponent<throwScript>().startPosition);
            }

            return count;
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
        newOne.SetActive(true);
        throwReady = true;
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
