using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string newGameSceneName = "";
    public string mainMenuSceneName = "";

    public void NewGameButton() {
        SceneManager.LoadScene(newGameSceneName);
    }

    public void SettingsButton() { 
        //todo
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void MainMenuButton() {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
