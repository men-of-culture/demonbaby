using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public Canvas mainMenuCanvas;
    public Canvas helpMenuCanvas;

    public PlayerState playerState;

    public void Quit() {
        Application.Quit();
    }

    
    public void StartGame() {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Help() {
        mainMenuCanvas.enabled = false;
        helpMenuCanvas.enabled = true;
    }

    public void Return() {
        helpMenuCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }

    public void Host() {
        playerState.isHost = true;
        StartGame();
    }
}
