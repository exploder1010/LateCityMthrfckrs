using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class BestTimeDictionary : MonoBehaviour {

    public Dictionary<string, float> bestTimes = new Dictionary<string, float>();
    public GameObject levelSelect;
    //private GameData gameData;
    private string gameDataProjectFilePath = "/StreamingAssets/data.json";

//    // Update is called once per frame
//    void Update ()
//    {
//		if(SceneManager.GetActiveScene().name == "MainMenu")
//        {
//            //set levelSelect
//        }
//	}

//    void CreateBestTimes()
//    {
//        foreach(level in levelSelect)
//        {
//            bestTimes.Add(level.gameObject.name, 0f);
//        }
//        //save to file
//    }

//    void LoadBestTimes()
//    {
//        foreach(level in levelSelect)
//        {
//            level.bestTime = bestTimes[level.gameObject.name];
//        }
//        //somehow get that shit on screen
//    }

//    void UpdateBestTimes(float time)
//    {
//        if(bestTimes[SceneManager.GetActiveScene().name] < time)
//        {
//            bestTimes[SceneManager.GetActiveScene().name] = time;
//            //save to file
//        }
//    }

//    void SaveData()
//    {
//        string JASON = JsonUtility.ToJson(data);
//        string filePath = Application.dataPath + ;
//        File.WriteAllText(filePath, JASON);
//    }

//    void LoadData()
//    {
//        string filePath = Application.dataPath + ;
//        if (File.Exists(filePath))
//        {
//            string JASON = File.ReadAllText(filePath);
//            data = JsonUtility.FromJson<Data>(JASON);
//        }
//        else
//        {
//            data = new GameData();
//        }
//    }
}
