using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Boss : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 1000f; // Vida alta para durar no mínimo 1 minuto
    public float currentHealth;

    [Header("Prefabs de Projéteis")]
    public GameObject projectileBossPrefab1;
    public GameObject projectileBossPrefab2;
    public GameObject projectileBossPrefab3;

    [Header("Limites de Movimentação (Eixo X)")]
    public float minX = -6f;
    public float maxX = 6f;

    // Pools individuais para cada prefab de projétil
    private ObjectPool<GameObject> pool1;
    private ObjectPool<GameObject> pool2;
    private ObjectPool<GameObject> pool3;

    // Estados de Vida do Chefão
    public enum HealthPhase { High, Medium, Low }
    public HealthPhase currentPhase;

    // Gerenciador de Comportamento Atual
    private IBossBehavior currentBehavior;
    private int currentBehaviorIndex = 0;
    private float behaviorTimer = 0f;
    public float behaviorSwitchInterval = 7f; // Alterna comportamentos a cada 7s

    // Lista com as 3 variações de comportamento de cada fase
    private IBossBehavior[] highHealthBehaviors;
    private IBossBehavior[] mediumHealthBehaviors;
    private IBossBehavior[] lowHealthBehaviors;

    void Awake()
    {
        currentHealth = maxHealth;

        // Criar pools independentes para os 3 tipos de projétil
        pool1 = CreatePool(projectileBossPrefab1);
        pool2 = CreatePool(projectileBossPrefab2);
        pool3 = CreatePool(projectileBossPrefab3);

        // Inicializar os 9 comportamentos (3 por fase)
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
        // Atualizar fase de vida com base na saúde atual
        UpdateHealthPhase();

        // Controlar a alternância automática entre os 3 comportamentos da fase atual
        behaviorTimer += Time.deltaTime;
        if (behaviorTimer >= behaviorSwitchInterval)
        {
            behaviorTimer = 0f;
            currentBehaviorIndex = (currentBehaviorIndex + 1) % 3;
            SetBehavior(GetCurrentPhaseBehaviors()[currentBehaviorIndex]);
        }

        // Executar a lógica do comportamento ativo
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

        // Se mudou de fase de vida, reseta a rotação de comportamentos
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
        switch (currentPhase)
        {
            case HealthPhase.High: return highHealthBehaviors;
            case HealthPhase.Medium: return mediumHealthBehaviors;
            default: return lowHealthBehaviors;
        }
    }

    private void SetBehavior(IBossBehavior newBehavior)
    {
        currentBehavior?.Exit();
        currentBehavior = newBehavior;
        currentBehavior?.Enter();
    }

    // Método genérico para criar ObjectPool
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

    // Função pública para que os comportamentos façam disparos
    public void FireProjectile(int prefabIndex, Vector2 direction, float speed)
    {
        ObjectPool<GameObject> targetPool = prefabIndex switch
        {
            1 => pool1,
            2 => pool2,
            _ => pool3
        };

        GameObject proj = targetPool.Get();
        proj.transform.position = transform.position;
        proj.transform.rotation = Quaternion.identity;

        if (proj.TryGetComponent<BossProjectile>(out var bossProj))
        {
            bossProj.SetPool(targetPool);
            bossProj.Setup(direction, speed);
        }
    }

    // Receber dano vindo dos Tiros do Jogador
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            TakeDamage(10f);
        }
    }
}