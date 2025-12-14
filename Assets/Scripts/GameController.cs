using System;
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

    private void OnDestroy()
    {
        // Unsubscribe from static events
        PlayerHealth.OnPlayerDied -= GameOverScreen;
    }

    void GameOverScreen()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        MusicManager.PauseBackgroundMusic();
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        MusicManager.PlayBackgroundMusic(true);
        Time.timeScale = 1;

        PlayerPrefs.DeleteAll();
        Debug.Log("Retry: All PlayerPrefs cleared.");

        // Wait until scene is fully loaded to invoke OnReset
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(1);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnReset?.Invoke();
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        if (progressSlider != null)
            progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            MarkLevelCompleted();
            if (loadCanvas != null)
                loadCanvas.SetActive(true);

            Debug.Log("Level Complete!");
        }
    }

    void LoadLevel(int level)
    {
        if (loadCanvas != null)
            loadCanvas.SetActive(false);

        if (levels != null && levels.Count > 0)
        {
            levels[currentLevelIndex].SetActive(false);
            levels[level].SetActive(true);
        }

        if (player != null)
            player.transform.position = new Vector3(-33, 2, 0);

        currentLevelIndex = level;
        progressAmount = 0;
        if (progressSlider != null)
            progressSlider.value = 0;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex);
    }

    void MarkLevelCompleted()
    {
        string levelKey = $"LevelCompleted_{currentLevelIndex}";
        PlayerPrefs.SetInt(levelKey, 1);

        if (levels != null && levels.Count > 0)
            Debug.Log($"Completed level: {levels[currentLevelIndex].name}");

        PlayerPrefs.Save();
    }
}
