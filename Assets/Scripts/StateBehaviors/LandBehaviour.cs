using UnityEngine;

public class LandBehaviour : StateMachineBehaviour
{

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayLandingSound();
    }

    private void PlayLandingSound()
    {
        AudioManager.Instance.Play("playerLand");
    }
}
