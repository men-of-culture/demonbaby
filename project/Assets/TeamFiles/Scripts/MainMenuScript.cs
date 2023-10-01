using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject changeScenePrefab;

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
        changeScenePrefab.GetComponent<SceneChangerScript>().FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
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
