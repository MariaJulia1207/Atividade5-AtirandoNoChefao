using UnityEngine;

// ==========================================
// FASE DE VIDA ALTA (3 COMPORTAMENTOS FÁCEIS)
// ==========================================

// Comportamento 1: Movimento Lento Horizontal + Tiro Reto Simples
public class EasyBehavior1 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public EasyBehavior1(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        // Movimentação
        float x = Mathf.Sin(Time.time * 1f) * boss.maxX;
        boss.transform.position = new Vector3(x, boss.transform.position.y, 0f);

        // Tiro
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.8f)
        {
            shootTimer = 0f;
            boss.FireProjectile(1, Vector2.down, 5f);
        }
    }
}

// Comportamento 2: Movimento Parado + Tiro Duplo Paralelo
public class EasyBehavior2 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public EasyBehavior2(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.6f)
        {
            shootTimer = 0f;
            boss.FireProjectile(1, new Vector2(-0.2f, -1f), 5f);
            boss.FireProjectile(1, new Vector2(0.2f, -1f), 5f);
        }
    }
}

// Comportamento 3: Movimento em V + Tiro Rápido Único
public class EasyBehavior3 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public EasyBehavior3(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        float x = Mathf.PingPong(Time.time * 3f, boss.maxX * 2) - boss.maxX;
        boss.transform.position = new Vector3(x, boss.transform.position.y, 0f);

        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.4f)
        {
            shootTimer = 0f;
            boss.FireProjectile(1, Vector2.down, 7f);
        }
    }
}

// ==========================================
// FASE DE VIDA MÉDIA (3 COMPORTAMENTOS MÉDIOS)
// ==========================================

// Comportamento 4: Movimento Rápido + Tiro Triplo em Leque
public class MediumBehavior1 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public MediumBehavior1(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        float x = Mathf.Sin(Time.time * 1.5f) * boss.maxX;
        boss.transform.position = new Vector3(x, boss.transform.position.y, 0f);

        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.5f)
        {
            shootTimer = 0f;
            boss.FireProjectile(2, Vector2.down, 6f);
            boss.FireProjectile(2, new Vector2(-0.4f, -1f), 6f);
            boss.FireProjectile(2, new Vector2(0.4f, -1f), 6f);
        }
    }
}

// Comportamento 5: Movimento Senoidal no Y + Tiro Circular
public class MediumBehavior2 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public MediumBehavior2(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        float y = 3f + Mathf.Sin(Time.time * 2f) * 1f;
        boss.transform.position = new Vector3(boss.transform.position.x, y, 0f);

        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.7f)
        {
            shootTimer = 0f;
            int count = 6;
            for (int i = 0; i < count; i++)
            {
                float angle = i * (360f / count);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                boss.FireProjectile(2, dir, 5f);
            }
        }
    }
}

// Comportamento 6: Teleporte Lateral + Rajada Direcionada
public class MediumBehavior3 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public MediumBehavior3(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.3f)
        {
            shootTimer = 0f;
            boss.FireProjectile(2, Vector2.down, 8f);
        }
    }
}

// ==========================================
// FASE DE VIDA BAIXA (3 COMPORTAMENTOS DIFÍCEIS)
// ==========================================

// Comportamento 7: Movimento Errático + Tiro Quádruplo Rápido
public class HardBehavior1 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public HardBehavior1(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        float x = Mathf.Sin(Time.time * 2f) * boss.maxX;
        boss.transform.position = new Vector3(x, boss.transform.position.y, 0f);

        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.25f)
        {
            shootTimer = 0f;
            boss.FireProjectile(3, new Vector2(-1f, -1f), 9f);
            boss.FireProjectile(3, new Vector2(-0.5f, -1f), 9f);
            boss.FireProjectile(3, new Vector2(0.5f, -1f), 9f);
            boss.FireProjectile(3, new Vector2(1f, -1f), 9f);
        }
    }
}

// Comportamento 8: Tiro Espiral (Spiral Pattern)
public class HardBehavior2 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;
    private float angle = 0f;

    public HardBehavior2(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.1f)
        {
            shootTimer = 0f;
            angle += 25f;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            boss.FireProjectile(3, dir, 7f);
        }
    }
}

// Comportamento 9: Frenesi Final (Chuva de Projéteis com Ângulos Aleatórios)
public class HardBehavior3 : IBossBehavior
{
    private Boss boss;
    private float shootTimer;

    public HardBehavior3(Boss boss) { this.boss = boss; }
    public void Enter() { }
    public void Exit() { }

    public void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.15f)
        {
            shootTimer = 0f;
            Vector2 randomDir = new Vector2(Random.Range(-0.8f, 0.8f), -1f);
            boss.FireProjectile(3, randomDir, Random.Range(6f, 10f));
        }
    }
}