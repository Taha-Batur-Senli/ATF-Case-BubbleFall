using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] currLevel levelData;
    public TextAsset jsonFile;
    Levels levelsInJson;

    // Start is called before the first frame update
    void Start()
    {
        levelsInJson = JsonUtility.FromJson<Levels>(jsonFile.text);
        levelData.setLevels(levelsInJson);
    }
}

[System.Serializable]
public class Levels
{
    public Level[] levels;
}

[System.Serializable]
public class Level
{
    public int levelID;
    public int matLen;
    public int[] amountOnEachRow;
}
