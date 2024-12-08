using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import the Scene Management namespace

public class MainMenu : MonoBehaviour
{
    // Method to load the SampleScene
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); 
    }
}
