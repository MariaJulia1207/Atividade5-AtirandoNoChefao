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
    public float fallSpeed = 2f;

    private IObjectPool<GameObject> pool;
    private float screenBottomLimit = -10f;
    private bool isCollected = false;

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
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

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
        if (player.TryGetComponent<PlayerControl>(out var playerControl))
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
                    playerControl.ActivateSpeedBoost(effectValue, duration);
                    break;

                case PowerUpType.Shield:
                    playerControl.ActivateShield(duration);
                    break;

                case PowerUpType.DamageBoost:
                    playerControl.ActivateDamageBoost(duration);
                    break;
            }
        }

        ReleaseToPool();
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