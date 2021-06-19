/*
* File: UIManager.cs
*
* Author: Mara Dusevic (s200494@students.aie.edu.au)
* Date Created: Friday 18 June 2021
* Date Last Modified: Friday 18 June 2021
*
* Used to change the active scene as well as exit
* the application. Can be used to be attached to
* buttons on canvas'.
*
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Loads main menu scene
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    // Loads level scene using the scene's name
    public void ChangeLevel(string a_name)
    {
        // If given name cannot be found, return with warning.
        // Otherwise, load scene with the name.
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

    // Reloads tghe current scene open
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }

    // Exits the application
    public void ExitGame()
    {
        Application.Quit();
    }
}