using System.Collections;
using UnityEngine;

public class LifeHUD : MonoBehaviour
{
    [SerializeField] private GameObject[] shipLifeIcons;
    private HealthSystem healthSystem;

    private IEnumerator Start()
    {
        // Aguarda até o Player ser instanciado na cena de gameplay
        while (healthSystem == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null && playerObject.TryGetComponent<HealthSystem>(out healthSystem))
            {
                healthSystem.OnHealthChanged += AtualizarHUD;
                AtualizarHUD(healthSystem.CurrentHealth, healthSystem.MaxHealth);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
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
            shipLifeIcons[i].SetActive(i < currentHealth);
        }
    }
}