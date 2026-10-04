using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Health,
        Speed,
        Shield,
        DamageBoost
    }

    public PowerUpType powerUpType;
    public float effectValue = 1f;
    public float duration = 5f;
    public float fallSpeed = 2f; // Velocidade de queda do item

    private IObjectPool<GameObject> pool;
    private float screenBottomLimit = -10f;
    private bool isCollected = false;

    // Configura o pool e a cota de destruição ao ser gerado
    public void SetPool(IObjectPool<GameObject> poolInstance, float bottomLimit)
    {
        pool = poolInstance;
        screenBottomLimit = bottomLimit;
    }

    void OnEnable()
    {
        isCollected = false;
    }

    void Update()
    {
        // Faz o PowerUp cair em direção ao fundo da tela
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        // Se passar do limite inferior da tela sem ser coletado, retorna ao pool
        if (transform.position.y <= screenBottomLimit)
        {
            ReleaseToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;
            ApplyEffect(other.gameObject);
        }
    }

    public virtual void ApplyEffect(GameObject player)
    {
        switch (powerUpType)
        {
            case PowerUpType.Health:
                if (player.TryGetComponent<HealthSystem>(out var health))
                {
                    health.Heal((int)effectValue);
                }
                break;

            case PowerUpType.Speed:
                if (player.TryGetComponent<PlayerControl>(out var playerControl))
                {
                    playerControl.StartCoroutine(ApplyTemporarySpeed(playerControl));
                }
                break;

            case PowerUpType.Shield:
                // Lógica para ativar o escudo
                break;

            case PowerUpType.DamageBoost:
                // Lógica para aumentar dano
                break;
        }

        ReleaseToPool();
    }

    private IEnumerator ApplyTemporarySpeed(PlayerControl player)
    {
        player.moveSpeed += effectValue;
        yield return new WaitForSeconds(duration);
        player.moveSpeed -= effectValue;
    }

    private void ReleaseToPool()
    {
        if (pool != null)
        {
            pool.Release(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}