using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
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
    [SerializeField] public int distance;
    [SerializeField] public int zStartThrow;
    public UnityEngine.Vector3 startpos;

    [SerializeField] public GameObject preventor;
    public float backMostRowZ;

    [SerializeField] public int endGameOnZ;
    [SerializeField] public int emptyRowCount = 0;

    //[SerializeField] public int amountToCreate;
    [SerializeField] public int[] amountOnEachRow;
    [SerializeField] public float ballWidth = 7;
    [SerializeField] public float ballHeight = 7;

    [SerializeField] public float distanceToFloor = -196.67f;
    [SerializeField] public int maxNumberOfBallsInRow = 7;

    public List<List<GameObject>> createdBalls = new List<List<GameObject>>();
    public List<List<int>> locationIndices = new List<List<int>>();
    public bool throwReady = true;

    public float speed = 500000f; // Adjust the speed as needed
    [SerializeField] public float widthLow;
    [SerializeField] public float heightLow;
    // [SerializeField] public float widthHigh;
    // [SerializeField] public float heightHigh;

    // Start is called before the first frame update
    void Start()
    {
        startpos = new UnityEngine.Vector3(thrownBall.transform.position.x, distanceToFloor, thrownBall.transform.position.z);
        distance = (int) heightLow - zStartThrow;
        emptyRowCount = (distance / (int) ballWidth);

        gameOver.SetActive(false);
        int matOfBall;
        int count = 0;

        for(int i = 0; i < amountOnEachRow.Length + emptyRowCount; i++)
        {
            locationIndices.Add(new List<int>());

            for(int y = 0; y < maxNumberOfBallsInRow; y++)
            {
                locationIndices[i].Add(-1);
            }
        }

        for (int a = 0; a < emptyRowCount; a++)
        {
            createdBalls.Add(new List<GameObject>());

            for (int x  = 0; x < maxNumberOfBallsInRow; x++)
            {
                createdBalls[a].Add(null);
            }
        }

        while (count < amountOnEachRow.Length)
        {
            createdBalls.Add(new List<GameObject>());

            if (amountOnEachRow[count] > maxNumberOfBallsInRow || amountOnEachRow[count] < 0)
            {
                amountOnEachRow[count] = maxNumberOfBallsInRow;
            }
           
            for (int a = 0; a < amountOnEachRow[count]; a++)
            {
                GameObject createdShift = Instantiate(hitBallTemplate);
                createdShift.transform.position = new UnityEngine.Vector3(widthLow + (ballWidth * a), distanceToFloor, heightLow + (ballHeight * count));
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
                    createdShift.GetComponent<createdBallScript>().toLeft = createdBalls[count + emptyRowCount][a - 1];
                }

                createdBalls[count + emptyRowCount].Add(createdShift);
                locationIndices[count][a] = matOfBall;
            }

            for(int a = amountOnEachRow[count]; a < maxNumberOfBallsInRow; a++)
            {
                createdBalls[count + emptyRowCount].Add(null);
            }

            count++;
        }

        backMostRowZ = heightLow + (ballHeight * count);

        createThrow(startpos);
    }

    // Update is called once per frame
    void Update()
    {
        if (!throwReady)
        {
            throwReady = true;
        }
    }

    public void placeBall(GameObject hitBall, GameObject thrownBall)
    {
        int yPlace = hitBall.GetComponent<createdBallScript>().ballIDY + emptyRowCount;
        int xPlace = hitBall.GetComponent<createdBallScript>().ballIDX;

        if (yPlace == 0)
        {
            gameOver.SetActive(true);
        }
        else if(yPlace + 1 < amountOnEachRow.Length - 1 && createdBalls[yPlace + 1][xPlace] == null)
        {
            generateForThrown(xPlace, yPlace + 1, createdBalls[yPlace][xPlace].transform.position.x, createdBalls[yPlace][xPlace].transform.position.z + ballHeight, thrownBall);
        }
        else if(xPlace + 1 < maxNumberOfBallsInRow && createdBalls[yPlace][xPlace + 1] == null)
        {
            generateForThrown(xPlace + 1, yPlace, createdBalls[yPlace][xPlace].transform.position.x + ballWidth, createdBalls[yPlace][xPlace].transform.position.z, thrownBall);
        }
        else if (xPlace - 1 >= 0 && createdBalls[yPlace][xPlace - 1] == null)
        {
            generateForThrown(xPlace - 1, yPlace, createdBalls[yPlace][xPlace].transform.position.x - ballWidth, createdBalls[yPlace][xPlace].transform.position.z, thrownBall);
        }
        else if (yPlace - 1 > 0 && createdBalls[yPlace - 1][xPlace] == null)
        {
            generateForThrown(xPlace, yPlace - 1, createdBalls[yPlace][xPlace].transform.position.x, createdBalls[yPlace][xPlace].transform.position.z - ballHeight, thrownBall);
        }

        if(thrownBall.gameObject.GetComponent<MeshRenderer>().material.name.Equals(hitBall.GetComponent<MeshRenderer>().material.name))
        {
            hitCheck(thrownBall);
        }

        Destroy(thrownBall);
        createThrow(startpos);
    }

    private void generateForThrown(int xloc, int yloc, float xCoord, float zCoord, GameObject thrownBall)
    {
        GameObject createdShift = Instantiate(hitBallTemplate);
        createdShift.transform.position = new UnityEngine.Vector3(xCoord, distanceToFloor, zCoord);
        createdShift.GetComponent<MeshRenderer>().material = thrownBall.gameObject.GetComponent<MeshRenderer>().material;
        createdShift.GetComponent<createdBallScript>().ballIDY = yloc;
        createdShift.GetComponent<createdBallScript>().ballIDX = xloc;
        createdShift.GetComponent<createdBallScript>().materialIndex = thrownBall.GetComponent<throwScript>().matID;

        locationIndices[yloc][xloc] = thrownBall.GetComponent<throwScript>().matID;
        amountOnEachRow[yloc]++;
        createdBalls[yloc][xloc] = createdShift;
    }

    public int hitCheck(GameObject target)
    {
        return 0;
        /*List<GameObject> savedPositions = new List<GameObject>();
        if (!target.GetComponent<createdBallScript>().checkedForRemoval)
        {
            savedPositions.Add(target);
            target.GetComponent<createdBallScript>().checkedForRemoval = true;
        }

        int count = 2;
        int targetX = target.GetComponent<createdBallScript>().ballIDX;
        int targetY = target.GetComponent<createdBallScript>().ballIDY;
        bool topInc = false, botInc = false, leftInc = false, rightInc = false;

        int indexOfTarget = target.GetComponent<createdBallScript>().materialIndex;

        //Check Right
        if (createdBalls[targetY].Count - 1 >= targetX + 1 && createdBalls[targetY][targetX + 1].activeSelf && indexOfTarget == createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().materialIndex)
        {
            if (!createdBalls[targetY][targetX + 1].GetComponent<createdBallScript>().checkedForRemoval)
            {
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
                savedPositions.Add(createdBalls[targetY][targetX - 1]);
                createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                count++;
            }
            leftInc = true;
        }

        //Check Top
        if (createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX && createdBalls[targetY + 1][targetX].activeSelf && indexOfTarget == createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().materialIndex)
        {
            if (!createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval)
            {
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
                if (item.GetComponent<createdBallScript>() != null)
                {
                    if (item.GetComponent<createdBallScript>().checkedForRemoval)
                    {
                        item.GetComponent<createdBallScript>().dragDown = true;
                        goDown(item, item.GetComponent<createdBallScript>().ballIDY, item.GetComponent<createdBallScript>().ballIDX);
                    }

                }
                else
                {
                    item.GetComponent<throwScript>().dragDown = true;
                    goDown(item, item.GetComponent<throwScript>().placedY, item.GetComponent<throwScript>().placedX);
                }

                hitCheck(item);
            }

            line.SetActive(false);
            count = 0;
        }
        else
        {
            foreach (var item in savedPositions)
            {
                if (item.GetComponent<createdBallScript>() != null)
                {
                    item.GetComponent<createdBallScript>().checkedForRemoval = false;
                }
            }
        }

        return savedPositions.Count;*/
    }

    public int callHit(GameObject target, GameObject hitter, int prior = 0, bool addhitter = true)
    {
        if (target.GetComponent<throwScript>() != null)
        {
            target.GetComponent<throwScript>().collisionCount = 2;

            if (target.GetComponent<throwScript>().collidedWith != null)
            {
                if(target.GetComponent<throwScript>().collidedWith.GetComponent<createdBallScript>() == null)
                {
                    target.GetComponent<throwScript>().collidedWith.GetComponent<throwScript>().dragDown= true;
                    target.GetComponent<throwScript>().dragDown = true;
                    hitter.GetComponent<throwScript>().dragDown = true;
                    
                    return 0;
                }
                else
                {
                    target.GetComponent<throwScript>().collisionCount++;

                    if (target.GetComponent<throwScript>().collisionCount >= 3)
                    {
                        hitter.gameObject.GetComponent<throwScript>().dragDown = true;
                        callHit(target.GetComponent<throwScript>().collidedWith, target, 1);
                    }
                    else
                    {
                        target.GetComponent<throwScript>().collisionCount--;
                    }

                }
            }

            return target.GetComponent<throwScript>().collisionCount;
        }
        else
        {
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
                    savedPositions.Add(createdBalls[targetY][targetX - 1]);
                    createdBalls[targetY][targetX - 1].GetComponent<createdBallScript>().checkedForRemoval = true;
                    count++;
                }
                leftInc = true;
            }

            //Check Top
            if (createdBalls.Count - 1 >= targetY + 1 && createdBalls[targetY + 1].Count - 1 >= targetX && createdBalls[targetY + 1][targetX].activeSelf && indexOfTarget == createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().materialIndex)
            {
                if (!createdBalls[targetY + 1][targetX].GetComponent<createdBallScript>().checkedForRemoval)
                {
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

    public void createThrow(UnityEngine.Vector3 startPos)
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
