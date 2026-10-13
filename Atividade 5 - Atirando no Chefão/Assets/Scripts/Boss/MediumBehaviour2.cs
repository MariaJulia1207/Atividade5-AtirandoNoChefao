using UnityEngine;

public class MediumBehaviour2 : StateMachineBehaviour
{
    private readonly Boss boss;
    private float shootTimer;

    public MediumBehaviour2(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
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

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enter();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateBehavior();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Exit();
    }
}
