using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    private IObjectPool<GameObject> pool;
    private float timer;

    // Registra a referência do pool que gerencia este projétil
    public void SetPool(IObjectPool<GameObject> poolInstance)
    {
        pool = poolInstance;
    }

    void OnEnable()
    {
        // Reinicia o tempo de vida sempre que a bala é ativada do pool
        timer = lifetime;
    }

    void Update()
    {
        // Movimento do projétil
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Contagem regressiva do tempo de vida
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReleaseToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            Destroy(other.gameObject);
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