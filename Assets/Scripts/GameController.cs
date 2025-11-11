using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;


    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    public static event Action OnReset;

    // Start is called before the first frame update
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoad.OnHoldComplete += LoadNextLevel;
        loadCanvas.SetActive(false);

        PlayerHealth.OnPlayerDied += GameOverScreen;
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        MusicManager.PauseBackgroundMusic();
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.PlayBackgroundMusic(true);
        SceneManager.LoadScene(1);
        OnReset.Invoke();
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        Debug.Log("Retry: All PlayerPrefs cleared.");

    }



    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            loadCanvas.SetActive(true);
            Debug.Log("Level Complete!");
        }
    }
    void LoadLevel(int level)
    {
        loadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(-33, 2, 0);

        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = 0;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex);
    }
}
