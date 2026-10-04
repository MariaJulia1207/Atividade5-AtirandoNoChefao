using UnityEngine;
using UnityEngine.Pool;

public class Boss : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("Prefabs de Projéteis")]
    public GameObject projectileBossPrefab1;
    public GameObject projectileBossPrefab2;
    public GameObject projectileBossPrefab3;

    [Header("Ponto de Disparo")]
    [Tooltip("Arraste um objeto filho posicionado abaixo do Boss para os tiros não nascerem dentro dele")]
    public Transform firePoint;

    [Header("Limites de Movimentação (Eixo X)")]
    public float minX = -6f;
    public float maxX = 6f;

    private ObjectPool<GameObject> pool1;
    private ObjectPool<GameObject> pool2;
    private ObjectPool<GameObject> pool3;

    public enum HealthPhase { High, Medium, Low }
    public HealthPhase currentPhase;

    private IBossBehavior currentBehavior;
    private int currentBehaviorIndex = 0;
    private float behaviorTimer = 0f;
    public float behaviorSwitchInterval = 7f;

    private IBossBehavior[] highHealthBehaviors;
    private IBossBehavior[] mediumHealthBehaviors;
    private IBossBehavior[] lowHealthBehaviors;

    void Awake()
    {
        currentHealth = maxHealth;

        pool1 = CreatePool(projectileBossPrefab1);
        pool2 = CreatePool(projectileBossPrefab2);
        pool3 = CreatePool(projectileBossPrefab3);

        highHealthBehaviors = new IBossBehavior[] {
            new EasyBehavior1(this),
            new EasyBehavior2(this),
            new EasyBehavior3(this)
        };

        mediumHealthBehaviors = new IBossBehavior[] {
            new MediumBehavior1(this),
            new MediumBehavior2(this),
            new MediumBehavior3(this)
        };

        lowHealthBehaviors = new IBossBehavior[] {
            new HardBehavior1(this),
            new HardBehavior2(this),
            new HardBehavior3(this)
        };

        UpdateHealthPhase();
    }

    void Update()
    {
        UpdateHealthPhase();

        behaviorTimer += Time.deltaTime;
        if (behaviorTimer >= behaviorSwitchInterval)
        {
            behaviorTimer = 0f;
            currentBehaviorIndex = (currentBehaviorIndex + 1) % 3;
            SetBehavior(GetCurrentPhaseBehaviors()[currentBehaviorIndex]);
        }

        currentBehavior?.UpdateBehavior();
    }

    private void UpdateHealthPhase()
    {
        float healthPercent = currentHealth / maxHealth;
        HealthPhase newPhase;

        if (healthPercent > 0.50f)
            newPhase = HealthPhase.High;
        else if (healthPercent > 0.20f)
            newPhase = HealthPhase.Medium;
        else
            newPhase = HealthPhase.Low;

        if (newPhase != currentPhase || currentBehavior == null)
        {
            currentPhase = newPhase;
            currentBehaviorIndex = 0;
            behaviorTimer = 0f;
            SetBehavior(GetCurrentPhaseBehaviors()[0]);
        }
    }

    private IBossBehavior[] GetCurrentPhaseBehaviors()
    {
        return currentPhase switch
        {
            HealthPhase.High => highHealthBehaviors,
            HealthPhase.Medium => mediumHealthBehaviors,
            _ => lowHealthBehaviors
        };
    }

    private void SetBehavior(IBossBehavior newBehavior)
    {
        currentBehavior?.Exit();
        currentBehavior = newBehavior;
        currentBehavior?.Enter();
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    public void FireProjectile(int prefabIndex, Vector2 direction, float speed)
    {
        ObjectPool<GameObject> targetPool = prefabIndex switch
        {
            1 => pool1,
            2 => pool2,
            _ => pool3
        };

        GameObject proj = targetPool.Get();

        // Usa o FirePoint se estiver atribuído, senão usa uma posição deslocada para baixo
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + (Vector3.down * 1.2f);
        proj.transform.position = spawnPos;
        proj.transform.rotation = Quaternion.identity;

        if (proj.TryGetComponent<BossProjectile>(out var bossProj))
        {
            bossProj.SetPool(targetPool);
            bossProj.Setup(direction, speed);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // Apenas a tag "PlayerProjectile" causa dano ao Boss
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            // O dano é processado pelo script do projétil do jogador para evitar chamadas duplas
        }
    }
}