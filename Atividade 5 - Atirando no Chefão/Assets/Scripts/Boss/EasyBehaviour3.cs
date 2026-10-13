using UnityEngine;

public class EasyBehaviour3 : StateMachineBehaviour
{
    private Boss boss;
    private float shootTimer;

    public EasyBehaviour3(Boss boss) { this.boss = boss; }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateBehavior()
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