using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string newGameSceneName = "";
    public string mainMenuSceneName = "";
    public GameObject settingsPanel;
    public GameObject mainMenuButton;
    public GameObject quitButton;

    public void NewGameButton() {
        SceneManager.LoadScene(newGameSceneName);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void MainMenuButton() {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenMainMenuSettings() {
        settingsPanel.SetActive(true);
        quitButton.SetActive(false);
        mainMenuButton.SetActive(false);
    }

    public void OpenIngameSettings()
    {
        settingsPanel.SetActive(true);
        quitButton.SetActive(true);
        mainMenuButton.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SoundSliderInput(Slider slider) { 
    //todo, when we will have sound system
    }

    public void MusicSliderInput(Slider slider)
    {
        //todo, when we will have sound system
    }
}
