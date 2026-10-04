using UnityEngine;
using UnityEngine.Pool;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    public float spawnInterval = 5f;
    public float screenTop = 10f;
    public float screenBottom = -10f;
    public float screenLeft = -15f;
    public float screenRight = 15f;

    // Array de pools (um para cada prefab de power-up)
    private ObjectPool<GameObject>[] powerUpPools;
    private float lastSpawnTime;

    void Awake()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        // Cria um pool dedicado para cada prefab presente no array
        powerUpPools = new ObjectPool<GameObject>[powerUpPrefabs.Length];

        for (int i = 0; i < powerUpPrefabs.Length; i++)
        {
            int prefabIndex = i; // Cópia local do índice para a função lambda de criação
            powerUpPools[i] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(powerUpPrefabs[prefabIndex]),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                defaultCapacity: 5,
                maxSize: 20
            );
        }
    }

    void Update()
    {
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            SpawnPowerUp();
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        float x = Random.Range(screenLeft, screenRight);
        Vector3 spawnPosition = new Vector3(x, screenTop, 0f);

        // Obtém o objeto a partir do pool sorteado
        GameObject powerUpObj = powerUpPools[randomIndex].Get();
        powerUpObj.transform.position = spawnPosition;
        powerUpObj.transform.rotation = Quaternion.identity;

        // Injeta a referência do pool e do limite inferior no PowerUp
        if (powerUpObj.TryGetComponent<PowerUp>(out var powerUp))
        {
            powerUp.SetPool(powerUpPools[randomIndex], screenBottom);
        }
    }
}