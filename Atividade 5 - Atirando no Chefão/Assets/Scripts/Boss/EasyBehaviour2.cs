using UnityEngine;

public class EasyBehaviour2 : StateMachineBehaviour
{
    private Boss boss;
    private float shootTimer;

    public EasyBehaviour2(Boss boss) { this.boss = boss; }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.6f)
        {
            shootTimer = 0f;
            boss.FireProjectile(1, new Vector2(-0.2f, -1f), 5f);
            boss.FireProjectile(1, new Vector2(0.2f, -1f), 5f);
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enter();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateBehavior();

        if (stateInfo.normalizedTime >= 1f)
        {
            animator.SetInteger("BehaviorIndex", 2);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Exit();
    }
}
