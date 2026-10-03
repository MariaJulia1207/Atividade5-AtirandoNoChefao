using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D player;
    public GameObject projectilePrefab;

    public float minShootInterval = 0.2f;
    public float maxShootInterval = 1f;

    private ObjectPool<GameObject> projectilePool;
    private bool isShooting = false;

    void Awake()
    {
        // Inicialização do Object Pool nativo da Unity
        projectilePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(projectilePrefab),
            actionOnGet: (obj) => {
                obj.transform.position = transform.position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
                if (obj.TryGetComponent<Projectile>(out var proj))
                {
                    proj.SetPool(projectilePool);
                }
            },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    void Start()
    {
        if (player == null)
            player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MovePlayer();
        HandleShooting();
    }

    void MovePlayer()
    {
        Vector2 inputVector = Vector2.zero;

        // Leitura contínua das teclas usando a nova Input System
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            inputVector.x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            inputVector.x += 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            inputVector.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            inputVector.y -= 1f;

        // Aplica o vetor resultante de uma só vez para não sobrescrever eixos
        player.linearVelocity = inputVector.normalized * moveSpeed;

        // Restringe a posição à tela da câmera
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void HandleShooting()
    {
        // Dispara imediatamente ao pressionar
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            isShooting = true;
            Shoot();
        }

        // Interrompe o loop de tiro ao soltar a tecla
        if (Keyboard.current.xKey.wasReleasedThisFrame)
        {
            isShooting = false;
            CancelInvoke(nameof(Shoot));
        }
    }

    void Shoot()
    {
        if (!isShooting) return;

        // Reutiliza ou cria um projétil do pool
        projectilePool.Get();

        // Se a tecla continuar pressionada, agenda o próximo disparo em intervalo aleatório
        if (Keyboard.current.xKey.isPressed)
        {
            float shootInterval = Random.Range(minShootInterval, maxShootInterval);
            Invoke(nameof(Shoot), shootInterval);
        }
    }
}