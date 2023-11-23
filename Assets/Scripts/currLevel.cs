using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class currLevel : MonoBehaviour
{
    public static Level level;
    public static Levels levels;
    public List<List<int>> elems = new List<List<int>>();
    public int rowCount = -1;

    private void Start()
    {
        
    }

    public void setLevels(Levels given)
    {
        levels = given;
    }

    public void loadLevel(int idL)
    {
        level = levels.levels[idL];
        SceneManager.LoadScene("GameplayScene");
    }

    public int getmatLen()
    {
        return level.matLen;
    }

    public int getID()
    {
        return level.levelID;
    }

    public void getrows()
    {
        elems.Add(new List<int>());
        rowCount++;

        foreach (int id in level.amountOnEachRow)
        {
            if(id < -1)
            {
                elems.Add(new List<int>());
                rowCount++;
            }
            else
            {
                elems[rowCount].Add(id);
            }
        }
    }
}
