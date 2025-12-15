using UnityEngine;

public class WinCollectible : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;

        GameController.TriggerWin();
        Destroy(gameObject);
    }
}
