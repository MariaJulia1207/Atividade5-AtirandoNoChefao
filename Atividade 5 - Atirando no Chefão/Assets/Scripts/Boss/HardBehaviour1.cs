using UnityEngine;

public class HardBehaviour1 : StateMachineBehaviour
{
    private readonly Boss boss;
    private float shootTimer;

    public HardBehaviour1(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
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
