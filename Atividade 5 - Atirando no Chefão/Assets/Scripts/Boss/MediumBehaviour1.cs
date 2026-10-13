using UnityEngine;

public class MediumBehaviour1 : StateMachineBehaviour
{
    private readonly Boss boss;
    private float shootTimer;

    public MediumBehaviour1(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
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
