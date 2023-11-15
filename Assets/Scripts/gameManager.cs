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
    [SerializeField] public GameObject ignoreWhenFalling;
    //public bool canShoot = false;
    [SerializeField] public int zLim;
    [SerializeField] public string throwName = "ThrownSphere";

    [SerializeField] public GameObject preventor;
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

    public float speed = 500000f; // Adjust the speed as needed
    [SerializeField] public float widthLow;
    [SerializeField] public float heightLow;
    // [SerializeField] public float widthHigh;
    // [SerializeField] public float heightHigh;

    // Start is called before the first frame update
    void Start()
    {
        gameOver.SetActive(false);
        int matOfBall;
        int count = 0;

        while (count < amountOnEachRow.Length)
        {
            createdBalls.Add(new List<GameObject>());

            if (amountOnEachRow[count] > maxNumberOfBallsInRow || amountOnEachRow[count] < 0)
            {
                ////Debug.Log("Incorrect Number!");
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
                
                if(count + 1 < amountOnEachRow.Length && a < amountOnEachRow[count + 1])
                {
                    createdShift.GetComponent<createdBallScript>().hasOneUp = true;
                }
                else
                {
                    createdShift.GetComponent<createdBallScript>().hasOneUp = false;
                }

                if(a > 0)
                {
                    createdShift.GetComponent<createdBallScript>().toLeft = createdBalls[count][a - 1];
                }

                createdBalls[count].Add(createdShift);
            }

            count++;
        }

        backMostRowZ = heightLow + (ballHeight * count);
    }

    // Update is called once per frame
    void Update()
    {
        if (!throwReady)
        {
            //Debug.Log("generating");
            throwReady = true;
            createThrow(thrownBall.GetComponent<throwScript>().startPosition);
        }
    }

    public int callHit(GameObject target, GameObject hitter, int prior = 0, bool addhitter = true)
    {
        //Debug.Log("START");

        if (target.GetComponent<throwScript>() != null)
        {
            target.GetComponent<throwScript>().collisionCount = 2;

            ////Debug.Log("hit a previously thrown ball");
            if (target.GetComponent<throwScript>().collidedWith != null)
            {
                if(target.GetComponent<throwScript>().collidedWith.GetComponent<createdBallScript>() == null)
                {

                    //Debug.Log("in A");
                    target.GetComponent<throwScript>().collidedWith.GetComponent<throwScript>().dragDown= true;
                    target.GetComponent<throwScript>().dragDown = true;
                    hitter.GetComponent<throwScript>().dragDown = true;
                    
                    return 0;
                }
                else
                {
                    target.GetComponent<throwScript>().collisionCount++;
                    //Debug.Log("in B

                    if (target.GetComponent<throwScript>().collisionCount >= 3)
                    {
                        //Debug.Log("ss");
                        hitter.gameObject.GetComponent<throwScript>().dragDown = true;
                        callHit(target.GetComponent<throwScript>().collidedWith, target, 1);
                    }
                    else
                    {
                        //Debug.Log("ss2");
                        target.GetComponent<throwScript>().collisionCount--;
                    }

                }
            }

            return target.GetComponent<throwScript>().collisionCount;
        }
        else
        {
            //Debug.Log("in c");
            //Debug.Log("hit a generated ball");
            List<GameObject> savedPositions = new List<GameObject>();
            if(!target.GetComponent<createdBallScript>().checkedForRemoval)
            {
                savedPositions.Add(target);
                target.GetComponent<createdBallScript>().checkedForRemoval = true;
            }
            if(addhitter)
            {
                savedPositions.Add(hitter);
            }

            int count = 2 + prior;
            int targetX = target.GetComponent<createdBallScript>().ballIDX;
            int targetY = target.GetComponent<createdBallScript>().ballIDY;
            bool topInc = false, botInc = false, leftInc = false, rightInc = false;

            int indexOfTarget = target.GetComponent<createdBallScript>().materialIndex;

            //Check Right
            if (createdBalls[targetY].Count - 1 >= targetX + 1 && createdBalls[targetY][targetX + 1].activeSelf && indexOfTarget == createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                if(!createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    ////Debug.Log("Right included");
                    savedPositions.Add(createdBalls[targetY][targetX + 1]);
                    createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
                rightInc = true;
            }

            //Check Left
            if (0 <= targetX - 1 && createdBalls[targetY][targetX - 1].activeSelf && indexOfTarget == createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                if(!createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    ////Debug.Log("Left included");
                    savedPositions.Add(createdBalls[targetY][targetX - 1]);
                    createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
                leftInc = true;
            }

            ////Debug.Log(createdBalls.Count);
            //Check Top
            if (createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX && createdBalls[targetY + 1][targetX].activeSelf && indexOfTarget == createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    ////Debug.Log("Top included");
                    savedPositions.Add(createdBalls[targetY + 1][targetX]);
                    createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
                topInc = true;
            }

            //Check Down
            if (0 <= targetY - 1 && createdBalls[targetY - 1].Count - 1 >= targetX && createdBalls[targetY - 1][targetX].activeSelf && indexOfTarget == createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    ////Debug.Log("Down included");
                    savedPositions.Add(createdBalls[targetY - 1][targetX]);
                    createdBalls[targetY - 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
                botInc = true;
            }

            //Check Down Left
            if ((botInc || leftInc) && 0 <= targetY - 1 && 0 <= targetX - 1 && createdBalls[targetY - 1][targetX - 1].activeSelf && indexOfTarget == createdBalls[targetY - 1][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY - 1][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    savedPositions.Add(createdBalls[targetY - 1][targetX - 1]);
                    createdBalls[targetY - 1][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
            }

            //Check Down Right
            if ((botInc || rightInc) && 0 <= targetY - 1 && createdBalls[targetY - 1].Count - 1 >= targetX + 1 && createdBalls[targetY - 1][targetX + 1].activeSelf && indexOfTarget == createdBalls[targetY - 1][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY - 1][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    savedPositions.Add(createdBalls[targetY - 1][targetX + 1]);
                    createdBalls[targetY - 1][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
            }
                
            //Check Top Left
            if ((topInc || leftInc) && createdBalls.Count - 1 >= targetY + 1 && 0 <= targetX - 1 && createdBalls[targetY + 1].Count - 1 >= targetX + 1 && createdBalls[targetY + 1][targetX - 1].activeSelf && indexOfTarget == createdBalls[targetY + 1][targetX - 1].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY + 1][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    savedPositions.Add(createdBalls[targetY + 1][targetX - 1]);
                    createdBalls[targetY + 1][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
            }

            //Check Top Right
            if ((topInc || rightInc) && createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX + 1 && createdBalls[targetY + 1][targetX + 1].activeSelf && indexOfTarget == createdBalls[targetY + 1][targetX + 1].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY + 1][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval)
                {
                    savedPositions.Add(createdBalls[targetY + 1][targetX + 1]);
                    createdBalls[targetY + 1][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
            }

            if (count >= 3)
            {
                foreach (var item in savedPositions)
                {
                    if(item.GetComponent<createdBallScript>() != null)
                    {
                        if(item.GetComponent<createdBallScript>().checkedForRemoval)
                        {
                            item.GetComponent<createdBallScript>().dragDown = true;
                            goDown(item, item.GetComponent<createdBallScript>().ballIDY, item.GetComponent<createdBallScript>().ballIDX);
                        }

                        callHit(item, null, 1, false);
                    }
                    else
                    {
                        if(item.GetComponent<createdBallScript>() == null)
                        {
                            item.GetComponent<throwScript>().dragDown = true;
                        }

                        if(item.GetComponent<createdBallScript>() != null)
                        {
                            item.GetComponent<createdBallScript>().dragDown = true;
                        }
                    }
                }

                for(int i = 0; i < createdBalls.Count; i++)
                {
                    for (int b = 0; b < createdBalls[i].Count; b++)
                    {
                        checkAround(createdBalls[i][b]);
                    }
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

            return count;
        }
    }

    public void goDown(GameObject item, int yCoord, int xCoord)
    {
        for(int coordY = 0; coordY < yCoord; coordY++)
        {
            if (createdBalls[coordY].Count - 1 >= xCoord)
            {
                createdBalls[coordY][xCoord].GetComponent<createdBallScript>().dragDown = true;
            }
        }
    }

    public void createThrow(Vector3 startPos)
    {
        GameObject newOne = Instantiate(thrownBall);
        newOne.transform.position = startPos;
        line.GetComponent<lineScript>().startPos = newOne.transform;
        newOne.SetActive(true);
        throwReady = true;
    }

    public void checkAround(GameObject ball)
    {
        /*
         * 
    public int checkAround(int count, int indexOfTarget)
    {
        //Check Right
        if (createdBalls[targetY].Count - 1 >= targetX + 1 && createdBalls[targetY][targetX + 1].activeSelf && indexOfTarget == createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().materialIndex)
        {
            if (!createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval)
            {
                ////Debug.Log("Right included");
                savedPositions.Add(createdBalls[targetY][targetX + 1]);
                createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }
            rightInc = true;
        }

        //Check Left
        if (0 <= targetX - 1 && createdBalls[targetY][targetX - 1].activeSelf && indexOfTarget == createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().materialIndex)
        {
            if (!createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval)
            {
                ////Debug.Log("Left included");
                savedPositions.Add(createdBalls[targetY][targetX - 1]);
                createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }
            leftInc = true;
        }

        ////Debug.Log(createdBalls.Count);
        //Check Top
        if (createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX && createdBalls[targetY + 1][targetX].activeSelf && indexOfTarget == createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().materialIndex)
        {
            if (!createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval)
            {
                ////Debug.Log("Top included");
                savedPositions.Add(createdBalls[targetY + 1][targetX]);
                createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }
            topInc = true;
        }

        return count;
    }
         */
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
