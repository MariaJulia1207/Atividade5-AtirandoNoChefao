using UnityEngine;

public class HardStateBehaviour : StateMachineBehaviour
{
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
}
