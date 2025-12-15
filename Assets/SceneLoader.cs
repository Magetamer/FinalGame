using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;   // Scene name or index to load
    public string portalID;      // Unique ID to keep track of what portal to disable

    [Header("Unlock Settings")]
    public bool requiresBothLevelsDone = false; // Unlock condition for final level

    private Collider2D col;

    public SpriteRenderer spriteRenderer;
    public GameObject otherObject;
    public string disableObjectKey;

    private void Start()
    {
        col = GetComponent<Collider2D>();

        // Disable portal if already used
        if (PlayerPrefs.GetInt(portalID, 0) == 1)
        {
            if (col != null)
                col.enabled = false;
        }

        if (otherObject != null)
        {
            otherObject.SetActive(PlayerPrefs.GetInt(disableObjectKey, 0) == 0);
        }

        // Disable portal if it requires both levels and they aren't done yet
        if (requiresBothLevelsDone)
        {
            if (PlayerPrefs.GetInt("DashUnlocked", 0) == 1)
            {
                col.enabled = true;
                spriteRenderer.enabled = true;
            }
            else
            {
                col.enabled = false;
                spriteRenderer.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || col == null || !col.enabled)
            return;

        // Mark portal as used
        PlayerPrefs.SetInt(portalID, 1);
        PlayerPrefs.Save();
        Debug.Log($"Portal used: {portalID}");

        // Attempt to unlock dash
        TryUnlockDash();

        // Disable collider so it can't be reused
        col.enabled = false;

        if (otherObject != null)
        {
            otherObject.SetActive(false); //hide gem to indicate level was already accessed
            PlayerPrefs.SetInt(disableObjectKey, 1);
            PlayerPrefs.Save();
        }


        // Load target scene
        SceneManager.LoadScene(sceneToLoad);
    }

    private void TryUnlockDash()
    {
        bool treeDone = PlayerPrefs.GetInt("TreeGem", 0) == 1;
        bool chestDone = PlayerPrefs.GetInt("ChestGem", 0) == 1;

        if (treeDone && chestDone)
        {
            PlayerPrefs.SetInt("DashUnlocked", 1);
            PlayerPrefs.Save();
            Debug.Log("Dash Unlocked!");
        }
    }
}
