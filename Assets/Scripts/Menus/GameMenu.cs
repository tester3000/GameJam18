﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMenu : MonoBehaviour {

    private AudioMixer audioMixer;
    private GameManager gameManager;
    public GameObject ResultPrefab;
    public GameObject IngamePrefab;
    public GameObject IconPrefab;
    public Slider musicSlider;
    public Slider ambientSlider;
    public Slider sfxSlider;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    void Start()
    {
        audioMixer = FindObjectOfType<AudioMixer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeInHierarchy)
            {
                pauseMenu.SetActive(false);
                pauseButton.SetActive(true);
                ResumeGame();
            }
            else
            {
                pauseMenu.SetActive(true);
                pauseButton.SetActive(false);
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        gameManager.ChangePauseState(true);
        LoadSliderValues();
    }

    public void ResumeGame()
    {
        gameManager.ChangePauseState(false);
        SaveSliderValues();
    }

    private void SaveSliderValues()
    {
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("Ambient", ambientSlider.value);
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
        PlayerPrefs.Save();
    }

    private void LoadSliderValues()
    {
        float ambient = PlayerPrefs.GetFloat("Ambient");
        float music = PlayerPrefs.GetFloat("Music");
        float sfx = PlayerPrefs.GetFloat("SFX");

        if (ambient > 0)
            ambientSlider.value = ambient;

        if (music > 0)
            musicSlider.value = music;

        if (sfx > 0)
            sfxSlider.value = sfx;

        ChangeAmbientVolume();
        ChangeMusicVolume();
        ChangeSFXVolume();
    }

    public void ChangeAmbientVolume()
    {
        if (gameManager.muted)
        {
            gameManager.muted = false;
            audioMixer.MuteAll(false);
        }
        audioMixer.EditSlider(0, ambientSlider.value);
        //Debug.Log("Ambient Sounds volume: " + slider.value);
    }

    public void ChangeMusicVolume()
    {
        if (gameManager.muted)
        {
            gameManager.muted = false;
            audioMixer.MuteAll(false);
        }
        audioMixer.EditSlider(1, musicSlider.value);
        //Debug.Log("Music volume: " + slider.value);
    }

    public void ChangeSFXVolume()
    {
        if (gameManager.muted)
        {
            gameManager.muted = false;
            audioMixer.MuteAll(false);
        }
        audioMixer.EditSlider(2, sfxSlider.value);
        //Debug.Log("Sound Effects volume: " + slider.value);
    }

    public void SetCountdownActive(bool state)
    {
        gameObject.transform.Find("Countdown").gameObject.SetActive(state);
    }

    public void SetCountdownText(string value)
    {
        gameObject.transform.Find("Countdown").GetComponentInChildren<TextMeshProUGUI>().SetText(value);
    }

    public void ShowIngameUI(List<PlayerObject> players)
    {
        float height = IngamePrefab.transform.position.y;
        foreach (PlayerObject p in players)
        {
            // spawn at correct height
            Vector3 pos = new Vector3(IngamePrefab.transform.position.x, height, IngamePrefab.transform.position.z);
            GameObject representation = Instantiate(IngamePrefab, pos, IngamePrefab.transform.rotation);

            representation.transform.SetParent(gameObject.transform.Find("ingamePlayers").transform, false);
            height -= 100;

            // icon, name
            representation.transform.Find("icon").GetComponent<Image>().sprite = p.icon;
            representation.transform.Find("name").GetComponent<TextMeshProUGUI>().SetText(p.playerName.ToString());
        }
    }

    public void ShowResults(List<SpawnArea> activeDogs)
    {
        gameObject.transform.Find("pauseButton").gameObject.SetActive(false);
        gameObject.transform.Find("ResultsMenu").gameObject.SetActive(true);

        //sort
        activeDogs.Sort((a,b) => b.GetSheep() - a.GetSheep());
        //viewModel.Children.Sort((a, b) => String.Compare(a.Name, b.Name))

        float height = ResultPrefab.transform.position.y;
        int rank = 1;
        int currentScore = activeDogs[0].GetSheep();
        int i = 0;
        foreach (SpawnArea spawn in activeDogs)
        {
            // spawn at correct height
            Vector3 pos = new Vector3(ResultPrefab.transform.position.x, height, ResultPrefab.transform.position.z);
            GameObject score = Instantiate(ResultPrefab, pos, ResultPrefab.transform.rotation);

            score.transform.SetParent(gameObject.transform.Find("ResultsMenu").Find("scorePlayers").transform, false);
            height -= 100;

            // set rank
            PlayerObject p = spawn.GetOwner();
            if (spawn.GetSheep() == currentScore)
            {
                score.transform.Find("rank").GetComponent<TextMeshProUGUI>().SetText(rank + ".");
            }
            else if(spawn.GetSheep() < currentScore)
            {
                rank = i+1;
                score.transform.Find("rank").GetComponent<TextMeshProUGUI>().SetText(i+1 + ".");
            }
            currentScore = spawn.GetSheep();

            // icon, name and score
            score.transform.Find("icon").GetComponent<Image>().sprite = p.icon;
            score.transform.Find("name").GetComponent<TextMeshProUGUI>().SetText(p.playerName.ToString());
            string scoreText = "captured " + spawn.GetSheep() + " sheep";
            score.transform.Find("score").GetComponent<TextMeshProUGUI>().SetText(scoreText);

            ++i;
        }
    }

    public void RestartGame()
    {
        SaveSliderValues();
        SceneManager.UnloadSceneAsync("Game");
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        FindObjectOfType<AudioEnabler>().FindButtons();
    }

    public void QuitGame()
    {
        SaveSliderValues();
        Application.Quit();
    }
}
