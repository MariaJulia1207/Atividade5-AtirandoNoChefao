using UnityEngine;

public class HardBehaviour3 : StateMachineBehaviour
{
    private readonly Boss boss;
    private float shootTimer;

    public HardBehaviour3(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= 0.15f)
        {
            shootTimer = 0f;
            Vector2 randomDir = new Vector2(Random.Range(-0.8f, 0.8f), -1f);
            boss.FireProjectile(3, randomDir, Random.Range(6f, 10f));
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
