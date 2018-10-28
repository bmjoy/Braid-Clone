using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.IdleTimer.Reset();
    }

}
