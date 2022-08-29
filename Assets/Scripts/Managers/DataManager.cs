using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.Wrappers;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataManager : MonoBehaviour
{
    #region Singleton
    private static DataManager _instance;

    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DataManager>();
            return _instance;
        }
    }
    #endregion

    public TestFlowSave saveData;
    
    public void SaveGameState()
    {
        /*
        SaveSystem saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem != null)
        {
            SaveSystem.SaveToSlot(1);
        }
        else
        {
            string saveData = PersistentDataManager.GetSaveData();
            PlayerPrefs.SetString("SavedGame", saveData);
        }
        */

        if (saveData == null)
        {
            saveData = new TestFlowSave();
        }
        saveData.buttonState = UIManager.Instance.buttonsState;
        saveData.success = PixelCrushers.DialogueSystem.DialogueLua.GetVariable("Roll.Success").asBool;
        saveData.difficulty = PixelCrushers.DialogueSystem.DialogueLua.GetVariable("Roll.Difficulty").asInt;
        saveData.successCount = UIManager.Instance.successCount;
        WriteJson("TestFlowSaveData.json", JsonUtility.ToJson(saveData));
    }

    public void LoadGameState()
    {
        /*
        PersistentDataManager.LevelWillBeUnloaded();
        SaveSystem saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem != null)
        {
            if (SaveSystem.HasSavedGameInSlot(1))
            {
                SaveSystem.LoadFromSlot(1);
            }
        }
        */

        string jsonData = ReadJsonToString("TestFlowSaveData.json");
        saveData = JsonUtility.FromJson<TestFlowSave>(jsonData);
        UIManager.Instance.buttonsState = saveData.buttonState;
        UIManager.Instance.successCount = saveData.successCount;
        PixelCrushers.DialogueSystem.DialogueLua.SetVariable("Roll.Success", saveData.success);
        PixelCrushers.DialogueSystem.DialogueLua.SetVariable("Roll.Difficulty", saveData.difficulty);
    }

    public void ResetSaveData()
    { 
        //reset back to default state;
        if (saveData == null)
        {
            saveData = new TestFlowSave();
        }
        saveData.buttonState = 1;
        saveData.successCount = 0;
        saveData.difficulty = 0;
        saveData.success = false;
        WriteJson("TestFlowSaveData.json", JsonUtility.ToJson(saveData));
    }


    #region DataManagement

    private string ReadJsonToString(string fileName)
    {
        string fullPath = Path.Combine("Assets/Data", fileName);
        if (File.Exists(fullPath))
        {
            StreamReader sr = new StreamReader(fullPath);
            string contents = sr.ReadToEnd();
            sr.Close();
            return contents;
        }
        else
        {
            return null;
        }
    }
    
    private  void WriteJson( string fileName, string json)
    {
        StreamWriter sw;
        
        string fullPath = Path.Combine("Assets/Data", fileName);

        try
        {
            if (!File.Exists(fullPath))
            {
                sw = File.CreateText(fullPath);
                sw.WriteLine(json);
                sw.Close();
            }
            else
            {
                File.Delete(fullPath);
                sw = File.CreateText(fullPath);
                sw.WriteLine(json);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    #endregion
    
    
    
    private void OnEnable()
    {
        Lua.RegisterFunction("ResetSaveData", this, SymbolExtensions.GetMethodInfo(() => ResetSaveData()));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("ResetSaveData");
    }
}
