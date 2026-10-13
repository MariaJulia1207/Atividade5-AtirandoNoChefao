using UnityEngine;

public class MediumStateBehaviour : StateMachineBehaviour
{
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
}
