using UnityEngine;
using UnityEngine.Pool;

public class BossProjectile : MonoBehaviour
{
    private float speed = 5f;
    private Vector2 direction = Vector2.down;
    private float lifetime = 4f;
    private float timer;
    private bool hasHit = false;

    private IObjectPool<GameObject> pool;

    public void SetPool(IObjectPool<GameObject> poolInstance)
    {
        pool = poolInstance;
    }

    public void Setup(Vector2 dir, float moveSpeed)
    {
        direction = dir.normalized;
        speed = moveSpeed;
    }

    void OnEnable()
    {
        timer = lifetime;
        hasHit = false;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReleaseToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // Ignora colisão com o próprio Boss
        if (other.CompareTag("Boss")) return;

        // Se atingir o Escudo: destrói o projétil sem aplicar dano ao jogador
        if (other.CompareTag("Shield"))
        {
            hasHit = true;
            ReleaseToPool();
            return;
        }

        // Se atingir o Jogador (quando o escudo está desativado)
        if (other.CompareTag("Player"))
        {
            hasHit = true;

            if (other.TryGetComponent<HealthSystem>(out var health))
            {
                health.TakeDamage(1);
            }

            ReleaseToPool();
        }
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