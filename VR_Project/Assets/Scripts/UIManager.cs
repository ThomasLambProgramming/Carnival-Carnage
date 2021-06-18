using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void ChangeLevel(string a_name)
    {
        if (SceneManager.GetSceneByName(a_name) == null)
        {
            Debug.LogWarning("No scene named " + a_name + " exists.");
            return;
        }
        else
        {
            SceneManager.LoadScene(a_name, LoadSceneMode.Single);
        }
    }

    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
