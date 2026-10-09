using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D player;

    [Header("Projéteis")]
    public GameObject projectilePrefab;
    public GameObject projectileBoostPrefab; // Prefab do tiro com +5 de dano
    public float minShootInterval = 0.2f;
    public float maxShootInterval = 1f;

    [Header("Efeitos visuais e de defesa")]
    public TrailRenderer speedTrail;
    public GameObject shieldObject; // Objeto filho com SpriteRenderer e CircleCollider2D

    private ObjectPool<GameObject> normalProjectilePool;
    private ObjectPool<GameObject> boostProjectilePool;

    private bool isShooting = false;
    private bool isDamageBoosted = false;

    private Coroutine speedCoroutine;
    private Coroutine shieldCoroutine;
    private Coroutine damageBoostCoroutine;

    void Awake()
    {
        // Pool para projéteis normais
        normalProjectilePool = CreateProjectilePool(projectilePrefab);

        // Pool para projéteis com boost de dano
        if (projectileBoostPrefab != null)
        {
            boostProjectilePool = CreateProjectilePool(projectileBoostPrefab);
        }
    }

    void Start()
    {
        if (player == null)
            player = GetComponent<Rigidbody2D>();

        // Garante que o trail e o escudo comecem desativados
        if (speedTrail != null) speedTrail.enabled = false;
        if (shieldObject != null) shieldObject.SetActive(false);
    }

    void Update()
    {
        MovePlayer();
        HandleShooting();
    }

    private ObjectPool<GameObject> CreateProjectilePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: (obj) => {
                obj.transform.position = transform.position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
                if (obj.TryGetComponent<Projectile>(out var proj))
                {
                    proj.SetPool(isDamageBoosted ? boostProjectilePool : normalProjectilePool);
                }
            },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    void MovePlayer()
    {
        Vector2 inputVector = Vector2.zero;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            inputVector.x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            inputVector.x += 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            inputVector.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            inputVector.y -= 1f;

        player.linearVelocity = inputVector.normalized * moveSpeed;

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void HandleShooting()
    {
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            isShooting = true;
            Shoot();
        }

        if (Keyboard.current.xKey.wasReleasedThisFrame)
        {
            isShooting = false;
            CancelInvoke(nameof(Shoot));
        }
    }

    void Shoot()
    {
        if (!isShooting) return;

        // Dispara do pool amplificado ou do pool normal
        if (isDamageBoosted && boostProjectilePool != null)
        {
            boostProjectilePool.Get();
        }
        else
        {
            normalProjectilePool.Get();
        }

        if (Keyboard.current.xKey.isPressed)
        {
            float shootInterval = Random.Range(minShootInterval, maxShootInterval);
            Invoke(nameof(Shoot), shootInterval);
        }
    }

    // ==========================================
    // MÉTODOS DE ATIVAÇÃO DOS POWER-UPS
    // ==========================================

    public void ActivateSpeedBoost(float boostValue, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostRoutine(boostValue, duration));
    }

    private IEnumerator SpeedBoostRoutine(float boostValue, float duration)
    {
        moveSpeed += boostValue;
        if (speedTrail != null) speedTrail.enabled = true;

        yield return new WaitForSeconds(duration);

        moveSpeed -= boostValue;
        if (speedTrail != null) speedTrail.enabled = false;
    }

    public void ActivateShield(float duration)
    {
        if (shieldCoroutine != null) StopCoroutine(shieldCoroutine);
        shieldCoroutine = StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        if (shieldObject != null) shieldObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        if (shieldObject != null) shieldObject.SetActive(false);
    }

    public void ActivateDamageBoost(float duration)
    {
        if (damageBoostCoroutine != null) StopCoroutine(damageBoostCoroutine);
        damageBoostCoroutine = StartCoroutine(DamageBoostRoutine(duration));
    }

    private IEnumerator DamageBoostRoutine(float duration)
    {
        isDamageBoosted = true;

        yield return new WaitForSeconds(duration);

        isDamageBoosted = false;
    }
}