using System;
using UnityEngine;

public class ClimbBehavior : StateMachineBehaviour
{
    private Player _player;
    private Rigidbody2D _rb;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = animator.gameObject.GetComponent<Player>();
        _rb = _player.GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        CurrentLayer = Layer.IGNORE_PLATFORMS;
        AudioManager.Instance.Play(Sound.PLAYER_CLIMB);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rb.gravityScale = 1f;
        CurrentLayer = Layer.PLAYER;
        animator.speed = 1;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //If the player is on the ladder but not moving, or the player has reached the top of the ladder, then the animation speed should be zero, else it should be one.
        animator.speed = Convert.ToInt32(!_player.IsClimbingLadder);
    }

    #region Layer handling
    private Layer CurrentLayer
    {
        set => _player.gameObject.layer = (int)value;
    }

    private enum Layer
    {
        IGNORE_PLATFORMS = 9, PLAYER = 11
    }
    #endregion
}
