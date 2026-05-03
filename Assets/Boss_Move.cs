using UnityEngine;

public class Boss_Move : StateMachineBehaviour
{
    public float speed       = 2.5f;
    public float meleeRange  = 1.8f;   // trigger melee attack
    public float rangedRange = 5f;    // trigger ranged/laser attack
    public float stopRange   = 1.4f;  // stop moving when this close — prevents overlapping player

    private Transform _player;
    private Rigidbody2D _rb;
    private Boss _boss;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb     = animator.GetComponent<Rigidbody2D>();
        _boss   = animator.GetComponent<Boss>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _boss.LookAtPlayer();

        float dist   = Vector3.Distance(_player.position, _rb.position);
        bool  isRage = animator.GetBool("InRage");

        // Only move if outside stop range
        if (dist > stopRange)
        {
            Vector3 newPos = Vector3.MoveTowards(_rb.position, _player.position, speed * Time.fixedDeltaTime);
            _rb.MovePosition(newPos);
        }

        if (dist <= meleeRange)
            animator.SetTrigger("AttackMelee");
        else if (dist >= rangedRange)
            animator.SetTrigger(isRage ? "AttackLaser" : "AttackRanged");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackMelee");
        animator.ResetTrigger("AttackRanged");
        animator.ResetTrigger("AttackLaser");
    }
}
