using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class DataToSave
{
    public int SavedPlayerScore;
    public List<MatchData> SavedMatchDatas = new List<MatchData>();
    public List<MatchCard> SavedCardData;
    public int TotalCards;

}



public class SaveLoadDataManager : MonoBehaviour
{
    private string savePathOfFile;
    void Start()
    {
        savePathOfFile = Application.persistentDataPath + "/saveData.save";
    }

    public void SaveGame()
    {
        DataToSave saveData = new DataToSave();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(savePathOfFile);
        binaryFormatter.Serialize(fileStream, saveData);
        fileStream.Close();
    }

    public void LoadGame()
    {
        if(File.Exists(savePathOfFile))
        {
            BinaryFormatter binaryFormatterLoadData = new BinaryFormatter();
            FileStream fileStreamLoadData = File.Open(savePathOfFile, FileMode.Open);
            DataToSave LoadedData = (DataToSave)binaryFormatterLoadData.Deserialize(fileStreamLoadData);
            fileStreamLoadData.Close();
        }
    }
}
