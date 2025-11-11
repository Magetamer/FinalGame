using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;   // Scene name or index to load
    public string portalID;      // Unique ID (e.g., "Tree1", "Tree2")

    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();

        // Disable teleport if already used
        if (PlayerPrefs.GetInt(portalID, 0) == 1)
        {
            if (col != null)
                col.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Mark this portal as used
            PlayerPrefs.SetInt(portalID, 1);
            PlayerPrefs.Save();

            // Disable collider so it can’t be reused
            if (col != null)
                col.enabled = false;

            // Load the target scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
