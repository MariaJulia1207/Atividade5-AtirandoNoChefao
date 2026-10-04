using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public float damage = 1f;

    private IObjectPool<GameObject> pool;
    private float timer;
    private bool hasHit = false; // Trava para impedir múltiplos acertos no mesmo frame

    public void SetPool(IObjectPool<GameObject> poolInstance)
    {
        pool = poolInstance;
    }

    void OnEnable()
    {
        timer = lifetime;
        hasHit = false; // Reseta a trava ao sair do pool
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReleaseToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Se já atingiu um alvo neste ciclo, ignora colisões adicionais
        if (hasHit) return;

        if (other.CompareTag("Boss"))
        {
            hasHit = true;

            if (other.TryGetComponent<Boss>(out var boss))
            {
                boss.TakeDamage(damage); // Aplica exatamente 1 ponto de dano
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