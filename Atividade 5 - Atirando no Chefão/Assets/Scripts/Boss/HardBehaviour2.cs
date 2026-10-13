using UnityEngine;

public class HardBehaviour2 : StateMachineBehaviour
{
    private readonly Boss boss;
    private float shootTimer;
    private float angle;

    public HardBehaviour2(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
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
