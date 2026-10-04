using UnityEngine;
using UnityEngine.UI;

public class LifeHUD : MonoBehaviour
{
    // Arraste os 3 ícones de navinha da UI para este Array no Inspector
    [SerializeField] private GameObject[] shipLifeIcons;

    private HealthSystem healthSystem;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null && playerObject.TryGetComponent<HealthSystem>(out healthSystem))
        {
            // Inscreve a HUD para atualizar sempre que a vida mudar
            healthSystem.OnHealthChanged += AtualizarHUD;
            AtualizarHUD(healthSystem.CurrentHealth, healthSystem.MaxHealth);
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= AtualizarHUD;
        }
    }

    private void AtualizarHUD(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < shipLifeIcons.Length; i++)
        {
            if (shipLifeIcons[i] == null) continue;

            // Se o índice for menor que a vida atual, a navinha fica visível.
            // Se o jogador tiver 2 de vida, os ícones 0 e 1 ficam ativos e o ícone 2 desaparece.
            shipLifeIcons[i].SetActive(i < currentHealth);
        }
    }
}