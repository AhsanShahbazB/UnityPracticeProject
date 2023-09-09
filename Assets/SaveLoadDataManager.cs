using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class DataToSave
{
    public int SavedPlayerScore;
    public List<string> cardFruitName = new List<string>();
    public int TotalCards;

}


public class SaveLoadDataManager : MonoBehaviour
{
    private string savePathOfFile;
    void Start()
    {
        savePathOfFile = Application.persistentDataPath + "/SaveData/saveData.txt";
    }

    public void SaveGame()
    {
        DataToSave saveData = new DataToSave();
            saveData.SavedPlayerScore = GameManager.Instance.PlayerScore;
            saveData.TotalCards = GameManager.Instance.TotalCards;
        for (int i = 0; i < GameManager.Instance.MatchCards.Count; i++)
        {
            switch (GameManager.Instance.MatchCards[i].matchFruitsForCard.Fruits)
            {
                case MatchFruits.Apple:
                    saveData.cardFruitName.Add("Apple");
                    break;
                case MatchFruits.Mango:
                    saveData.cardFruitName.Add("Mango");
                    break;
                case MatchFruits.Banana:
                    saveData.cardFruitName.Add("Banana");
                    break;
                case MatchFruits.Orange:
                    saveData.cardFruitName.Add("Orange");
                    break;
                case MatchFruits.Peach:
                    saveData.cardFruitName.Add("Peach");
                    break;
            }
        }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            var directoryName = Path.GetDirectoryName(savePathOfFile);
            if (Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            if(File.Exists(savePathOfFile))
            {
                File.Delete(savePathOfFile);
            }
            FileStream fileStream = File.Create(savePathOfFile);
            try
            {
                binaryFormatter.Serialize(fileStream, saveData);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            fileStream.Close();

            StartCoroutine(ShowNoSaveDataText("Data Saved", "Score", GameManager.Instance.WarningText_02));
        }
        public void LoadGame()
        {
        if(File.Exists(savePathOfFile))
        {
            BinaryFormatter binaryFormatterLoadData = new BinaryFormatter();
            FileStream fileStreamLoadData = File.Open(savePathOfFile, FileMode.Open);
            try
            {
                DataToSave LoadedData = (DataToSave)binaryFormatterLoadData.Deserialize(fileStreamLoadData);
                GameManager.Instance.PlayerScore = LoadedData.SavedPlayerScore;
                GameManager.Instance.TotalCards = LoadedData.TotalCards;
                GameManager.Instance.LoadGameToUser(LoadedData);
            }
            catch (Exception e)
            {
                File.Delete(savePathOfFile);
                StartCoroutine(ShowNoSaveDataText("No Save Data Found", "Card Match Game",GameManager.Instance.WarningText_02));
            }

            fileStreamLoadData.Close();
        }
        else
        {
            StartCoroutine(ShowNoSaveDataText("No Save Data Found", "Card Match Game",GameManager.Instance.WarningText_01));
        }
    }

    public IEnumerator ShowNoSaveDataText(string firstText,string secondText,Text textComponentToShowText)
    {
        textComponentToShowText.text = firstText;
        yield return new WaitForSeconds(2f);
        textComponentToShowText.text = secondText;

    }
}