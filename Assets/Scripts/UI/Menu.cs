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
    public MouseLock mouseLock;
    public Slider musicSlider;
    public Slider SFXSlider;

    private void Start()
    {
        if (SoundManager.instance)
        {
            SFXSlider.value = SoundManager.instance.SFXVolume;            
            musicSlider.value = SoundManager.instance.musicVolume;
        }
    }

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
        mouseLock?.SettingsClosed();
    }

    public void SoundSliderInput(Slider slider) {
        SoundManager.ChangeVolumeSFX(slider.value);
    }

    public void MusicSliderInput(Slider slider)
    {
        SoundManager.ChangeVolumeMusic(slider.value);
    }
}
