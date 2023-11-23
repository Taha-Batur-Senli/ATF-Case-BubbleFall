using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public class gameManager : MonoBehaviour
{
    [SerializeField] currLevel levelData;
    [SerializeField] GameObject thrownBall;
    [SerializeField] GameObject hitBallTemplate;
    [SerializeField] public GameObject backCube;
    [SerializeField] public GameObject line;
    [SerializeField] public Material[] matsToGive;
    [SerializeField] public GameObject gameOver;
    [SerializeField] public GameObject ignoreWhenFalling;
    [SerializeField] public GameObject preventor;
    [SerializeField] public int matLen = 7;
    [SerializeField] public int zLim;
    [SerializeField] public int distance;
    [SerializeField] public int zStartThrow;
    [SerializeField] public int failOnRow = 2;
    [SerializeField] public int cameraAdvanceAmount = 7;
    [SerializeField] public float xDiffTreshold = 1.5f;
    [SerializeField] public int emptyRowCount = 0;
    [SerializeField] public int[] amountOnEachRow;
    [SerializeField] public float ballWidth = 7;
    [SerializeField] public float ballHeight = 7;
    [SerializeField] public float distanceToFloor = -196.67f;
    [SerializeField] public int maxNumberOfBallsInRow = 7;
    [SerializeField] public float widthLow;
    [SerializeField] public float heightLow;
    [SerializeField] public int levelID = 1;
    [SerializeField] public int maxLevel = 10;
    [SerializeField] public GameObject nextLevelButton;

    [TextAreaAttribute]
    public string MyTextArea;

    int[] totalRowCount;
    public UnityEngine.Vector3 startpos;
    public float backMostRowZ;
    public List<List<GameObject>> createdBalls = new List<List<GameObject>>();
    public List<List<int>> locationIndices = new List<List<int>>();
    public bool throwReady = true;
    private int advanceRowCount = 0;
    GameObject currentBall = null;

    // Start is called before the first frame update
    void Start()
    {
        matLen = levelData.getmatLen();
        levelID = levelData.getID();
        levelData.getrows();

        int t = 0; 
        startpos = new UnityEngine.Vector3(thrownBall.transform.position.x, distanceToFloor, thrownBall.transform.position.z);
        distance = (int)heightLow - zStartThrow;
        emptyRowCount = (distance / (int)ballWidth);

        totalRowCount = new int[levelData.rowCount + emptyRowCount];

        gameOver.SetActive(false);
        int matOfBall;
        int count = 0;

        for(int x = 0; x < emptyRowCount; x++)
        {
            createdBalls.Add(new List<GameObject>());

            for(int y = 0; y < maxNumberOfBallsInRow; y++)
            {
                createdBalls[x].Add(null);
            }
        }

        for (int i = 0; i < totalRowCount.Length; i++)
        {
            locationIndices.Add(new List<int>());

            for (int y = 0; y < maxNumberOfBallsInRow; y++)
            {
                locationIndices[i].Add(-1);
            }
        }

        for (int a = 0; a < levelData.rowCount; a++)
        {
            createdBalls.Add(new List<GameObject>());

            for (int b = 0; b < maxNumberOfBallsInRow; b++)
            {
                if(levelData.elems[a][b] >= 0)
                {
                    GameObject createdShift = Instantiate(hitBallTemplate);
                    createdShift.SetActive(true);
                    createdShift.transform.position = new UnityEngine.Vector3(widthLow + (ballWidth * b), distanceToFloor, heightLow + (ballHeight * a));
                    matOfBall = levelData.elems[a][b];
                    createdShift.GetComponent<MeshRenderer>().material = matsToGive[matOfBall];
                    createdShift.GetComponent<createdBallScript>().ballIDY = a;
                    createdShift.GetComponent<createdBallScript>().ballIDX = b;
                    createdShift.GetComponent<createdBallScript>().materialIndex = matOfBall;

                    if (b > 0)
                    {
                        createdShift.GetComponent<createdBallScript>().toLeft = createdBalls[a + emptyRowCount][b - 1];
                    }

                    createdBalls[a + emptyRowCount].Add(createdShift);
                    locationIndices[a][b] = matOfBall;
                }
                else
                {
                    createdBalls[a + emptyRowCount].Add(null);
                }
            }
        }

        for (int a = 0; a < levelData.rowCount - 1; a++)
        {
            for (int b = 0; b < maxNumberOfBallsInRow; b++)
            {
                if (createdBalls[a + emptyRowCount][b] != null && createdBalls[a + emptyRowCount + 1][b] != null)
                {
                    createdBalls[a + emptyRowCount][b].GetComponent<createdBallScript>().oneUp = createdBalls[a + emptyRowCount + 1][b];
                }

                if (b + 1 < maxNumberOfBallsInRow && createdBalls[a + emptyRowCount][b] != null && createdBalls[a + emptyRowCount][b + 1] != null)
                {
                    createdBalls[a + emptyRowCount][b].GetComponent<createdBallScript>().toRight = createdBalls[a + emptyRowCount][b + 1];
                }
            }
        }

        if (matLen > matsToGive.Length || matLen < 0)
        {
            matLen = matsToGive.Length;
        }

        if(levelID >= maxLevel)
        {
            Destroy(nextLevelButton);
        }

        /*for (int a = 0; a < emptyRowCount; a++)
        {
            createdBalls.Add(new List<GameObject>());

            for (int x  = 0; x < maxNumberOfBallsInRow; x++)
            {
                createdBalls[a].Add(null);
            }
        }*/

        /*while (count < amountOnEachRow.Length)
        {
            createdBalls.Add(new List<GameObject>());

            if (amountOnEachRow[count] > maxNumberOfBallsInRow || amountOnEachRow[count] < 0)
            {
                amountOnEachRow[count] = maxNumberOfBallsInRow;
            }
           
            for (int a = 0; a < amountOnEachRow[count]; a++)
            {
                GameObject createdShift = Instantiate(hitBallTemplate);
                createdShift.SetActive(true);
                createdShift.transform.position = new UnityEngine.Vector3(widthLow + (ballWidth * a), distanceToFloor, heightLow + (ballHeight * count));
                matOfBall = UnityEngine.Random.Range(0, matLen);
                createdShift.GetComponent<MeshRenderer>().material = matsToGive[matOfBall];
                createdShift.GetComponent<createdBallScript>().ballIDY = count;
                createdShift.GetComponent<createdBallScript>().ballIDX = a;
                createdShift.GetComponent<createdBallScript>().materialIndex = matOfBall;

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
        }*/

        backMostRowZ = heightLow + (ballHeight * count);

        float posZ = 0;
        int index = levelData.rowCount + emptyRowCount - 1;

        posZ = (ballHeight * 2) + createdBalls[index][0].transform.position.z;

        backCube.transform.position = new UnityEngine.Vector3(backCube.transform.position.x, backCube.transform.position.y, posZ);

        createThrow(startpos);
    }

    // Update is called once per frame
    void Update()
    {
        if(checkIfOver())
        {
            gameOver.SetActive(true);
            enabled = false;
        }

        if (!throwReady)
        {
            throwReady = true;
        }

        if (checkIfAdvance(advanceRowCount + emptyRowCount))
        {
            advanceRowCount++;
            Camera.main.transform.position = new UnityEngine.Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + cameraAdvanceAmount);
            startpos = new UnityEngine.Vector3(startpos.x, startpos.y, startpos.z + cameraAdvanceAmount);
            preventor.transform.position = new UnityEngine.Vector3(preventor.transform.position.x, preventor.transform.position.y, preventor.transform.position.z + cameraAdvanceAmount);
            failOnRow++;
            currentBall.transform.position = startpos;
        }
    }

    private bool checkIfOver()
    {
        for(int y = 0; y < createdBalls.Count; y++)
        {
            for(int x = 0; x < maxNumberOfBallsInRow; x++)
            {
                if (createdBalls[y][x] != null && !createdBalls[y][x].GetComponent<createdBallScript>().dragDown)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void placeBall(GameObject hitBall, GameObject thrownBall)
    {
        int yPlace = hitBall.GetComponent<createdBallScript>().ballIDY + emptyRowCount;
        int xPlace = hitBall.GetComponent<createdBallScript>().ballIDX;
        bool checkup = false;
        int xLoc = 0;
        int yLoc = 0;

        float diffbwXs = thrownBall.transform.position.x - hitBall.transform.position.x;

        if (yPlace == failOnRow)
        {
            gameOver.SetActive(true);
        }
        else if(Math.Abs(diffbwXs) < xDiffTreshold)
        {
            if (thrownBall.transform.position.z > hitBall.transform.position.z && yPlace + 1 < totalRowCount.Length - 1 && createdBalls[yPlace + 1][xPlace] == null)
            {
                checkup = generateForThrown(xPlace, yPlace + 1, createdBalls[yPlace][xPlace].transform.position.x, createdBalls[yPlace][xPlace].transform.position.z + ballHeight, thrownBall, checkup);
                xLoc = xPlace;
                yLoc = yPlace + 1;
            }
            else if (thrownBall.transform.position.z < hitBall.transform.position.z && yPlace - 1 > 0 && createdBalls[yPlace - 1][xPlace] == null)
            {
                checkup = generateForThrown(xPlace, yPlace - 1, createdBalls[yPlace][xPlace].transform.position.x, createdBalls[yPlace][xPlace].transform.position.z - ballHeight, thrownBall, checkup);
                xLoc = xPlace;
                yLoc = yPlace - 1;
            }
            else
            {
                if (thrownBall.transform.position.x > hitBall.transform.position.x && xPlace + 1 < maxNumberOfBallsInRow && createdBalls[yPlace][xPlace + 1] == null)
                {
                    checkup = generateForThrown(xPlace + 1, yPlace, createdBalls[yPlace][xPlace].transform.position.x + ballWidth, createdBalls[yPlace][xPlace].transform.position.z, thrownBall, checkup);
                    xLoc = xPlace + 1;
                    yLoc = yPlace;
                }
                else if (thrownBall.transform.position.x < hitBall.transform.position.x && xPlace - 1 >= 0 && createdBalls[yPlace][xPlace - 1] == null)
                {
                    checkup = generateForThrown(xPlace - 1, yPlace, createdBalls[yPlace][xPlace].transform.position.x - ballWidth, createdBalls[yPlace][xPlace].transform.position.z, thrownBall, checkup);
                    xLoc = xPlace - 1;
                    yLoc = yPlace;
                }
            }
        }
        else
        {
            if (thrownBall.transform.position.x > hitBall.transform.position.x && xPlace + 1 < maxNumberOfBallsInRow && createdBalls[yPlace][xPlace + 1] == null)
            {
                checkup = generateForThrown(xPlace + 1, yPlace, createdBalls[yPlace][xPlace].transform.position.x + ballWidth, createdBalls[yPlace][xPlace].transform.position.z, thrownBall, checkup);
                xLoc = xPlace + 1;
                yLoc = yPlace;
            }
            else if (thrownBall.transform.position.x < hitBall.transform.position.x && xPlace - 1 >= 0 && createdBalls[yPlace][xPlace - 1] == null)
            {
                checkup = generateForThrown(xPlace - 1, yPlace, createdBalls[yPlace][xPlace].transform.position.x - ballWidth, createdBalls[yPlace][xPlace].transform.position.z, thrownBall, checkup);
                xLoc = xPlace - 1;
                yLoc = yPlace;
            }
            else
            {
                if (thrownBall.transform.position.z > hitBall.transform.position.z && yPlace + 1 < totalRowCount.Length - 1 && createdBalls[yPlace + 1][xPlace] == null)
                {
                    checkup = generateForThrown(xPlace, yPlace + 1, createdBalls[yPlace][xPlace].transform.position.x, createdBalls[yPlace][xPlace].transform.position.z + ballHeight, thrownBall, checkup);
                    xLoc = xPlace;
                    yLoc = yPlace + 1;
                }
                else if (thrownBall.transform.position.z < hitBall.transform.position.z && yPlace - 1 > 0 && createdBalls[yPlace - 1][xPlace] == null)
                {
                    checkup = generateForThrown(xPlace, yPlace - 1, createdBalls[yPlace][xPlace].transform.position.x, createdBalls[yPlace][xPlace].transform.position.z - ballHeight, thrownBall, checkup);
                    xLoc = xPlace;
                    yLoc = yPlace - 1;
                }
            }
        }

        if(!gameOver.activeSelf && (checkup || hitBall.GetComponent<createdBallScript>().materialIndex.Equals(thrownBall.GetComponent<throwScript>().matID)))
        {
            hitCheck(createdBalls[yLoc][xLoc], xLoc, yLoc, onLast: true);
        }

        Destroy(thrownBall);
        createThrow(startpos);
    }

    private bool generateForThrown(int xloc, int yloc, float xCoord, float zCoord, GameObject thrownBall, bool checkup)
    {
        GameObject createdShift = Instantiate(hitBallTemplate);
        createdShift.transform.position = new UnityEngine.Vector3(xCoord, distanceToFloor, zCoord);
        createdShift.GetComponent<MeshRenderer>().material = thrownBall.gameObject.GetComponent<MeshRenderer>().material;
        createdShift.GetComponent<createdBallScript>().ballIDY = yloc - emptyRowCount;
        createdShift.GetComponent<createdBallScript>().ballIDX = xloc;
        createdShift.GetComponent<createdBallScript>().materialIndex = thrownBall.GetComponent<throwScript>().matID;
        createdShift.SetActive(true);
        locationIndices[yloc][xloc] = thrownBall.GetComponent<throwScript>().matID;
        totalRowCount[yloc]++;

        if(yloc + 1 < createdBalls.Count && createdBalls[yloc + 1][xloc] != null)
        {
            createdShift.GetComponent<createdBallScript>().oneUp = createdBalls[yloc + 1][xloc];

            if(createdBalls[yloc + 1][xloc].GetComponent<createdBallScript>().materialIndex.Equals(thrownBall.GetComponent<throwScript>().matID))
            {
                checkup = true;
            }
        }

        if (yloc - 1 >= 0 && createdBalls[yloc - 1][xloc] != null)
        {
            createdBalls[yloc - 1][xloc].GetComponent<createdBallScript>().oneUp = createdShift;

            if (createdBalls[yloc - 1][xloc].GetComponent<createdBallScript>().materialIndex.Equals(thrownBall.GetComponent<throwScript>().matID))
            {
                checkup = true;
            }
        }

        if (xloc - 1 >= 0 && createdBalls[yloc][xloc - 1] != null)
        {
            createdShift.GetComponent<createdBallScript>().toLeft = createdBalls[yloc][xloc - 1];
            createdBalls[yloc][xloc - 1].GetComponent<createdBallScript>().toRight = createdShift;

            if (createdBalls[yloc][xloc - 1].GetComponent<createdBallScript>().materialIndex.Equals(thrownBall.GetComponent<throwScript>().matID))
            {
                checkup = true;
            }
        }

        if (xloc + 1 < maxNumberOfBallsInRow && createdBalls[yloc][xloc + 1] != null)
        {
            createdBalls[yloc][xloc + 1].GetComponent<createdBallScript>().toLeft = createdShift;
            createdShift.GetComponent<createdBallScript>().toRight = createdBalls[yloc][xloc + 1];

            if (createdBalls[yloc][xloc + 1].GetComponent<createdBallScript>().materialIndex.Equals(thrownBall.GetComponent<throwScript>().matID))
            {
                checkup = true;
            }
        }

        createdBalls[yloc][xloc] = createdShift;

        return checkup;
    }

    public void hitCheck(GameObject target, int xloc, int yloc, List<GameObject> list = null, bool onLast = false)
    {
        if(list == null)
        {
            list = new List<GameObject>();
        }

        if (!target.GetComponent<createdBallScript>().checkedForRemoval)
        {
            target.GetComponent<createdBallScript>().checkedForRemoval = true;
            list.Add(target);

            if (yloc + 1 < createdBalls.Count && createdBalls[yloc + 1][xloc] != null && !createdBalls[yloc + 1][xloc].GetComponent<createdBallScript>().checkedForRemoval && createdBalls[yloc + 1][xloc].GetComponent<createdBallScript>().materialIndex.Equals(target.GetComponent<createdBallScript>().materialIndex))
            {
                hitCheck(createdBalls[yloc + 1][xloc], xloc, yloc + 1, list);
            }

            if (yloc - 1 >= 0 && createdBalls[yloc - 1][xloc] != null && !createdBalls[yloc - 1][xloc].GetComponent<createdBallScript>().checkedForRemoval && createdBalls[yloc - 1][xloc].GetComponent<createdBallScript>().materialIndex.Equals(target.GetComponent<createdBallScript>().materialIndex))
            {
                hitCheck(createdBalls[yloc - 1][xloc], xloc, yloc - 1, list);
            }

            if (xloc - 1 >= 0 && createdBalls[yloc][xloc - 1] != null && !createdBalls[yloc][xloc - 1].GetComponent<createdBallScript>().checkedForRemoval && createdBalls[yloc][xloc - 1].GetComponent<createdBallScript>().materialIndex.Equals(target.GetComponent<createdBallScript>().materialIndex))
            {
                hitCheck(createdBalls[yloc][xloc - 1], xloc - 1, yloc, list);
            }

            if (xloc + 1 < maxNumberOfBallsInRow && createdBalls[yloc][xloc + 1] != null && !createdBalls[yloc][xloc + 1].GetComponent<createdBallScript>().checkedForRemoval && createdBalls[yloc][xloc + 1].GetComponent<createdBallScript>().materialIndex.Equals(target.GetComponent<createdBallScript>().materialIndex))
            {
                hitCheck(createdBalls[yloc][xloc + 1], xloc + 1, yloc, list);
            }
        }

        if(onLast)
        {
            if (list.Count >= 3)
            {
                foreach (var item in list)
                {
                    item.GetComponent<createdBallScript>().dragDown = true;
                }

            }
            else
            {
                foreach (var item in list)
                {
                    item.GetComponent<createdBallScript>().checkedForRemoval = false;
                }
            }
        }
        
    }

    private bool checkIfAdvance(int idY)
    {
        for(int y = idY; y > 0; y--)
        {
            for(int x = 0; x < maxNumberOfBallsInRow; x++)
            {
                if (createdBalls[y][x] != null && !createdBalls[y][x].GetComponent<createdBallScript>().dragDown)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void createThrow(UnityEngine.Vector3 startPos)
    {
        GameObject newOne = Instantiate(thrownBall);
        newOne.SetActive(true);
        newOne.transform.position = startPos;
        line.GetComponent<lineScript>().startPos = newOne.transform;
        newOne.SetActive(true);
        throwReady = true;
        currentBall = newOne;
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void goToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void nextLevel()
    {
        levelData.loadLevel(levelID);
    }
}
