using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }
    #endregion

    #region Refs
    
    [Header("Flow Buttons")]
    public Button button1;
    public Button button2;
    public Button button3;
    [Header("Roll UI")] 
    public GameObject rollInfoView;
    public Text txtDifficulty;
    public Text txtPhysical;
    public Text txtSocial;
    public Text txtMental;
    public Text txtRollCount;
    public Text txtSuccessCount;
    public Text txtResult;
    
    #endregion

    private void Start()
    {
        SetButtonState(1);
    }
    
    public void RollDice()
    {
       int successCap = 6; 
       int difficulty =  PixelCrushers.DialogueSystem.DialogueLua.GetVariable("Roll.Difficulty").asInt;
       int physical =  PixelCrushers.DialogueSystem.DialogueLua.GetVariable("PlayerStats.Physical").asInt;
       int social =  PixelCrushers.DialogueSystem.DialogueLua.GetVariable("PlayerStats.Social").asInt;
       int mental =  PixelCrushers.DialogueSystem.DialogueLua.GetVariable("PlayerStats.Mental").asInt;
       int rollCount = (physical + social + mental);
       int successCount = 0;
       bool success = false;

       for (int i = 0; i < rollCount; i++)
       {
           int roll = Random.Range(0, 10);
           if (roll >= successCap ) successCount++;
       }
       success = (successCount >= difficulty);
       
       txtDifficulty.text = "Difficulty: " + difficulty;
       txtPhysical.text = "Physical: " + physical;
       txtSocial.text = "Social: " + social;
       txtMental.text = "Mental: " + mental;
       txtRollCount.text = "Total Roll: " + rollCount + "d10";
       txtSuccessCount.text = "Success Count: " + successCount;
       txtResult.text = (success) ? "Result: Success" : "Result: Failed";

       PixelCrushers.DialogueSystem.DialogueLua.SetVariable("Roll.Success", success);
       
       rollInfoView.SetActive(true);
       SetButtons(3);
    }

    public void EndRoll()
    {
        rollInfoView.SetActive(false);
    }

    public void SetButtons(int val)
    {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        switch (val)
        {
            case 1:
                button1.gameObject.SetActive(true);
                break;
            case 2:
                button2.gameObject.SetActive(true);
                break;
            case 3:
                button3.gameObject.SetActive(true);
                break;
            case 0:
                EndRoll();
                break;
        }
    }
    public void SetButtonState(double val)
    {
        SetButtons((int)val);
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("SetButtonState", this, SymbolExtensions.GetMethodInfo(() => SetButtonState((double)0)));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("SetButtonState");
    }
}
