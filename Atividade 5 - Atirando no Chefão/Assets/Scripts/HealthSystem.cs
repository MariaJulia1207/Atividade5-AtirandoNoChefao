using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    // Eventos para notificar a HUD e o jogo sem necessidade de checar no Update
    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        // Notifica a HUD no início do jogo
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnPlayerDied?.Invoke();
        gameObject.SetActive(false);
    }
}