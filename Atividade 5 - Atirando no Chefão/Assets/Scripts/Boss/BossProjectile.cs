using UnityEngine;
using UnityEngine.Pool;

public class BossProjectile : MonoBehaviour
{
    private float speed = 5f;
    private Vector2 direction = Vector2.down;
    private float lifetime = 4f;
    private float timer;

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
        if (other.CompareTag("Player"))
        {
            // Causar dano ao jogador (se aplicável)
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