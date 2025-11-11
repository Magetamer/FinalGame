using UnityEngine;

public class ResetPrefs : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs cleared.");
    }
}
