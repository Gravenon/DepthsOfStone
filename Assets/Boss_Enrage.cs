using UnityEngine;

/// <summary>
/// State machine behaviour on the Imunitet (invulnerability) state.
/// Slows down the animator so the phase-transition animation plays longer.
/// </summary>
public class Boss_Enrage : StateMachineBehaviour
{
    [Tooltip("Animator playback speed during the invulnerability animation (< 1 = slower)")]
    public float animationSpeed = 0.4f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Boss_Health>().isInvulneable = true;
        animator.speed = animationSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 1f;
        animator.GetComponent<Boss_Health>().isInvulneable = false;
    }
}
