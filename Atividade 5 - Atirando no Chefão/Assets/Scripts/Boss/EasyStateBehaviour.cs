using UnityEngine;

public class EasyStateBehaviour : StateMachineBehaviour
{
    public class EasyBehavior1 : IBossBehavior
    {
        private Boss boss;
        private float shootTimer;

        public EasyBehavior1(Boss boss) { this.boss = boss; }
        public void Enter() { }
        public void Exit() { }

        public void UpdateBehavior()
        {
            float x = Mathf.Sin(Time.time * 1f) * boss.maxX;
            boss.transform.position = new Vector3(x, boss.transform.position.y, 0f);

            shootTimer += Time.deltaTime;
            if (shootTimer >= 0.8f)
            {
                shootTimer = 0f;
                boss.FireProjectile(1, Vector2.down, 5f);
            }
        }
    }

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
}
