using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public Canvas mainMenuCanvas;
    public Canvas helpMenuCanvas;

    public PlayerState playerState;
    public AudioSource audioSource;

    public void Start() {
        audioSource = GetComponent<AudioSource>(); 
    }

    public void Quit() {
        Application.Quit();
    }

    
    public void StartGame() {
        audioSource.Play();
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Help() {
        audioSource.Play();
        mainMenuCanvas.enabled = false;
        helpMenuCanvas.enabled = true;
    }

    public void Return() {
        audioSource.Play();
        helpMenuCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }

    public void Host() {
        audioSource.Play();
        playerState.isHost = true;
        StartGame();
    }
}
