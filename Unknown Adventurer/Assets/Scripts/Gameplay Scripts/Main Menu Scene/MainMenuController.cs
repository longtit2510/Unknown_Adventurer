using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("Main Menu Theme");
    }
    public void Play()
    {
        FindObjectOfType<AudioManager>().Stop("Main Menu Theme");
        SceneManager.LoadScene("Gameplay");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
