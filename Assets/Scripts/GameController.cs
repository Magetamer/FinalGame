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

    [Header("Win Screen")]
    public GameObject winScreen;

    public static event Action OnReset;
    public static event Action OnPlayerWin;

    void Start()
    {
        progressAmount = 0;
        if (progressSlider != null)
            progressSlider.value = 0;

        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoad.OnHoldComplete += LoadNextLevel;

        loadCanvas?.SetActive(false);

        PlayerHealth.OnPlayerDied += GameOverScreen;
        gameOverScreen?.SetActive(false);

        OnPlayerWin += WinScreen;
        winScreen?.SetActive(false);
    }

    private void OnDestroy()
    {
        Gem.OnGemCollect -= IncreaseProgressAmount;
        HoldToLoad.OnHoldComplete -= LoadNextLevel;
        PlayerHealth.OnPlayerDied -= GameOverScreen;
        OnPlayerWin -= WinScreen;
    }

    //Game Over Screen

    void GameOverScreen()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        MusicManager.PauseBackgroundMusic();
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen?.SetActive(false);
        winScreen?.SetActive(false);

        MusicManager.PlayBackgroundMusic(true);
        Time.timeScale = 1;

        PlayerPrefs.DeleteAll();
        Debug.Log("Retry: All PlayerPrefs cleared.");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(1); // HubWorld scene index
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnReset?.Invoke();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Win Screen for final gem collect

    void WinScreen()
    {
        if (winScreen != null)
            winScreen.SetActive(true);

        MusicManager.PauseBackgroundMusic();
        Time.timeScale = 0;
    }

    // Leftover code, mostly not used
    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;

        if (progressSlider != null)
            progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            MarkLevelCompleted();
            loadCanvas?.SetActive(true);
            Debug.Log("Level Complete!");
        }
    }

    void LoadLevel(int level)
    {
        loadCanvas?.SetActive(false);

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
        PlayerPrefs.Save();

        if (levels != null && levels.Count > 0)
            Debug.Log($"Completed level: {levels[currentLevelIndex].name}");
    }

    public static void TriggerWin()
    {
        OnPlayerWin?.Invoke();
    }

}
