using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This was just a script a designer made to run application.quit from a button
public class Quit : MonoBehaviour
{
    // Start is called before the first frame update
    public void QuitGame()
    {
        Application.Quit();
    }
}
