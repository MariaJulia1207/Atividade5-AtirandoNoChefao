using UnityEngine;

public class MediumBehaviour3 : StateMachineBehaviour
{
    private readonly Boss boss;
    private float shootTimer;

    public MediumBehaviour3(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.3f)
        {
            shootTimer = 0f;
            boss.FireProjectile(2, Vector2.down, 8f);
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
