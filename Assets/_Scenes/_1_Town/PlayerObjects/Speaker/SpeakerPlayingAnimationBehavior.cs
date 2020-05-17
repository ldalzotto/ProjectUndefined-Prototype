using UnityEngine;

public class SpeakerPlayingAnimationBehavior : StateMachineBehaviour
{
    public float DeltaTimeThresholdForChange = 1f;

    [Range(0f,1f)]
    public float SmoothDamp = 0.5f;

    private float CurrentDynamicSize = 0f;
    private float TargetDynamicSize = 0f;
    private float elapsedTime = 0f;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.elapsedTime += Time.deltaTime;
        if(this.elapsedTime >= DeltaTimeThresholdForChange)
        {
            this.elapsedTime -= DeltaTimeThresholdForChange;
            this.TargetDynamicSize = Random.Range(0f, 1f);
        }
        this.CurrentDynamicSize = Mathf.Lerp(this.CurrentDynamicSize, this.TargetDynamicSize, this.SmoothDamp);

        animator.SetFloat("SpeakerSize", this.CurrentDynamicSize);
    }
}
