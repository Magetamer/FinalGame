using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayerDied;

    private bool isDead = false; // prevent multiple death triggers

    void Start()
    {
        ResetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameController.OnReset += ResetHealth;
    }

    private void OnDestroy()
    {
        GameController.OnReset -= ResetHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(enemy.damage);
        }

        Trap trap = collision.GetComponent<Trap>();
        if (trap != null && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
            healthUI.SetMaxHearts(maxHealth);

        isDead = false;
    }

    private void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthUI != null)
            healthUI.UpdateHearts(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth < 1 && !isDead)
        {
            isDead = true;
            OnPlayerDied?.Invoke(); // null-safe
        }
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
        }
    }
}
