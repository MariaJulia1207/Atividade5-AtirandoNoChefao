using UnityEngine;

public class EasyBehaviour1 : StateMachineBehaviour
{
    private Boss boss;
    private float shootTimer;

    public EasyBehaviour1(Boss boss) { this.boss = boss; }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
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
