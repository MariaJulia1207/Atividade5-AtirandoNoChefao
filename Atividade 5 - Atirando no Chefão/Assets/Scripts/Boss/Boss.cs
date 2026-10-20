using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

public class Boss : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float phaseTransitionInvulnerabilityDuration = 2f;

    [Header("Prefabs de Projéteis")]
    public GameObject projectileBossPrefab1;
    public GameObject projectileBossPrefab2;
    public GameObject projectileBossPrefab3;

    [Header("Ponto de Disparo")]
    [Tooltip("Um objeto filho posicionado abaixo do Boss para os tiros não nascerem dentro dele")]
    public Transform firePoint;

    [Header("Limites de Movimentação (Eixo X)")]
    public float minX = -6f;
    public float maxX = 6f;

    [Header("Referências de UI e Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private BossHealthBar bossHealthBar;
    [SerializeField] private string bossPhaseParameter = "BossPhase";
    [SerializeField] private string behaviorIndexParameter = "BehaviorIndex";
    [SerializeField] private string previousBehaviorParameter = "PreviousBehavior";
    [SerializeField] private string phaseChangedTriggerParameter = "PhaseChanged";
    [SerializeField] private string invulnerableBoolParameter = "Invulnerable";
    [SerializeField] private string hurtTriggerParameter = "Hit";

    public enum HealthPhase { High, Medium, Low }
    public HealthPhase currentPhase;

    private ObjectPool<GameObject> pool1;
    private ObjectPool<GameObject> pool2;
    private ObjectPool<GameObject> pool3;

    private int currentBehaviorIndex = 0;
    private float behaviorTimer = 0f;
    public float behaviorSwitchInterval = 7f;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;

    private StateMachineBehaviour currentBehavior;
    private StateMachineBehaviour[] highHealthBehaviors;
    private StateMachineBehaviour[] mediumHealthBehaviors;
    private StateMachineBehaviour[] lowHealthBehaviors;

    private HealthPhase lastSyncedAnimatorPhase;

    public bool IsInvulnerable => invulnerabilityTimer > 0f;

    void Awake()
    {
        currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();

        pool1 = CreatePool(projectileBossPrefab1);
        pool2 = CreatePool(projectileBossPrefab2);
        pool3 = CreatePool(projectileBossPrefab3);

        InitializeBehaviors();

        currentPhase = DetermineHealthPhase();
        SyncAnimatorPhase();
        UpdateHealthBar();
        currentBehaviorIndex = 0;
        SyncBehaviorIndex();
        SetBehavior(GetCurrentPhaseBehaviors()[0]);
    }

    private void InitializeBehaviors()
    {
        highHealthBehaviors = new StateMachineBehaviour[] {
            new EasyBehaviour1(this),
            new EasyBehaviour2(this),
            new EasyBehaviour3(this)
        };

        mediumHealthBehaviors = new StateMachineBehaviour[] {
            new MediumBehaviour1(this),
            new MediumBehaviour2(this),
            new MediumBehaviour3(this)
        };

        lowHealthBehaviors = new StateMachineBehaviour[] {
            new HardBehaviour1(this),
            new HardBehaviour2(this),
            new HardBehaviour3(this)
        };
    }

    void Update()
    {
        if (isDead)
            return;

        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
            SetInvulnerableVisualState(true);

            if (invulnerabilityTimer <= 0f)
            {
                invulnerabilityTimer = 0f;
                SetInvulnerableVisualState(false);
                StartNextBehavior();
            }

            return;
        }

        UpdateHealthPhase();

        if (animator != null)
        {
            animator.SetInteger(bossPhaseParameter, (int)currentPhase);
            animator.SetBool(invulnerableBoolParameter, false);
        }

        if (bossHealthBar == null)
        {
            bossHealthBar = FindAnyObjectByType<BossHealthBar>();
        }

        behaviorTimer += Time.deltaTime;
        if (behaviorTimer >= behaviorSwitchInterval)
        {
            behaviorTimer = 0f;
            StartNextBehavior();
        }

        InvokeBehaviorMethod(currentBehavior, "UpdateBehavior");
    }

    private void UpdateHealthPhase()
    {
        HealthPhase newPhase = DetermineHealthPhase();
        if (newPhase == currentPhase && currentBehavior != null)
            return;

        currentPhase = newPhase;
        SyncAnimatorPhase();
        UpdateHealthBar();
        BeginPhaseTransition();
    }

    private void SyncAnimatorPhase()
    {
        if (animator == null)
            return;

        animator.SetInteger(bossPhaseParameter, (int)currentPhase);
        lastSyncedAnimatorPhase = currentPhase;
    }

    private void SyncBehaviorIndex()
    {
        if (animator == null)
            return;

        animator.SetInteger(behaviorIndexParameter, currentBehaviorIndex);
    }

    private HealthPhase DetermineHealthPhase()
    {
        float healthPercent = currentHealth / maxHealth;

        if (healthPercent > 0.50f)
            return HealthPhase.High;
        if (healthPercent > 0.20f)
            return HealthPhase.Medium;
        return HealthPhase.Low;
    }

    private void BeginPhaseTransition()
    {
        invulnerabilityTimer = phaseTransitionInvulnerabilityDuration;
        behaviorTimer = 0f;
        currentBehaviorIndex = 0;
        SyncBehaviorIndex();

        InvokeBehaviorMethod(currentBehavior, "Exit");
        currentBehavior = null;

        if (animator != null)
        {
            animator.SetTrigger(phaseChangedTriggerParameter);
            animator.SetBool(invulnerableBoolParameter, true);
        }

        UpdateHealthBar();
    }

    private void StartNextBehavior()
    {
        StateMachineBehaviour[] behaviors = GetCurrentPhaseBehaviors();
        if (behaviors == null || behaviors.Length == 0)
            return;

        currentBehaviorIndex = (currentBehaviorIndex + 1) % behaviors.Length;
        SyncBehaviorIndex();
        SetBehavior(behaviors[currentBehaviorIndex]);
    }

    private StateMachineBehaviour[] GetCurrentPhaseBehaviors()
    {
        return currentPhase switch
        {
            HealthPhase.High => highHealthBehaviors,
            HealthPhase.Medium => mediumHealthBehaviors,
            _ => lowHealthBehaviors
        };
    }

    private void SetBehavior(StateMachineBehaviour newBehavior)
    {
        InvokeBehaviorMethod(currentBehavior, "Exit");
        currentBehavior = newBehavior;
        SyncBehaviorIndex();
        InvokeBehaviorMethod(currentBehavior, "Enter");
    }

    private static void InvokeBehaviorMethod(StateMachineBehaviour behavior, string methodName)
    {
        if (behavior == null)
            return;

        MethodInfo method = behavior.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        method?.Invoke(behavior, null);
    }

    private Color GetPhaseColor(HealthPhase phase)
    {
        return phase switch
        {
            HealthPhase.High => new Color(0.2f, 0.85f, 0.4f, 1f),
            HealthPhase.Medium => new Color(1f, 0.7f, 0.2f, 1f),
            _ => new Color(0.85f, 0.2f, 0.2f, 1f)
        };
    }

    private void UpdateHealthBar()
    {
        if (bossHealthBar == null)
            return;

        bossHealthBar.Refresh(currentHealth, maxHealth, currentPhase, invulnerabilityTimer > 0f);
    }

    private void SetInvulnerableVisualState(bool value)
    {
        if (animator == null)
            return;

        animator.SetBool(invulnerableBoolParameter, value);
    }

    private void TriggerDamageAnimation()
    {
        if (animator == null)
            return;

        animator.SetTrigger(hurtTriggerParameter);
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
        if (IsInvulnerable)
            return;

        ObjectPool<GameObject> targetPool = prefabIndex switch
        {
            1 => pool1,
            2 => pool2,
            _ => pool3
        };

        GameObject proj = targetPool.Get();

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + (Vector3.down * 1.2f);
        proj.transform.position = spawnPos;
        proj.transform.rotation = Quaternion.identity;

        if (proj.TryGetComponent<BossProjectile>(out var bossProj))
        {
            bossProj.SetPool(targetPool);
            bossProj.Setup(direction, speed);
        }
    }

    private void NotifyDamageAnimation()
    {
        if (animator == null)
            return;

        animator.SetInteger(previousBehaviorParameter, currentBehaviorIndex);
        animator.SetTrigger(hurtTriggerParameter);
    }

    public void TakeDamage(float damage)
    {
        if (isDead || IsInvulnerable)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        NotifyDamageAnimation();
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            isDead = true;
            InvokeBehaviorMethod(currentBehavior, "Exit");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.FinishGame(GameManager.EndGameResult.Victory);
            }

            Destroy(gameObject);
            return;
        }

        UpdateHealthPhase();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            // O dano é processado pelo script do projétil do jogador para evitar chamadas duplas
        }
    }
}