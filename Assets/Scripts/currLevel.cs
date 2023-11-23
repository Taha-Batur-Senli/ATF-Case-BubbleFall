using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class currLevel : MonoBehaviour
{
    public static Level level;
    public static Levels levels;

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

    public int[] getrows()
    {
        return level.amountOnEachRow;
    }
}
