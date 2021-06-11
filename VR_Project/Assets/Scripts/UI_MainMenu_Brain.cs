using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu_Brain : MonoBehaviour
{
    public void OnMainMenuButtonPress(int Level)
    {
        try { SceneManager.LoadScene(Level, LoadSceneMode.Single); }
        catch { Debug.Log("ERROR : That scene does not exist!"); }
    } // loads a scene based on the level number or scene index int he build settings. If errors occur, check the build settings and double check the index of the desired level   
}
