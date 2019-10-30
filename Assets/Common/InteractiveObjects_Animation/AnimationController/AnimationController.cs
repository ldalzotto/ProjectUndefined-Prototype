using System;
using AnimatorPlayable;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObject_Animation
{
    public class AnimationController
    {
        private NavMeshAgent Agent;
        private AnimatorPlayableObject AnimatorPlayableObject;
        private Rigidbody Rigidbody;

        private BoolVariable RootMotionEnabled;


        public AnimationController(NavMeshAgent agent, AnimatorPlayableObject animatorPlayableObject, Rigidbody rigidbody,
            Action OnRootMotionEnabled = null, Action OnRootMotionDisabled = null)
        {
            Agent = agent;
            AnimatorPlayableObject = animatorPlayableObject;
            Rigidbody = rigidbody;

            this.RootMotionEnabled = new BoolVariable(false,
                () =>
                {
                    this.OnRootMotionEnabled();
                    if (OnRootMotionEnabled != null)
                    {
                        OnRootMotionEnabled.Invoke();
                    }
                },
                () =>
                {
                    this.OnRootMotionDisabled();
                    if (OnRootMotionDisabled != null)
                    {
                        OnRootMotionDisabled.Invoke();
                    }
                });
        }

        public void Tick(float d)
        {
            if (this.RootMotionEnabled.GetValue())
            {
                this.Agent.nextPosition = this.AnimatorPlayableObject.Animator.transform.position;
            }
        }

        public void PlayContextAction(SequencedAnimationInput ContextActionAnimation, bool rootMotion, Action OnAnimationFinished = null)
        {
            this.RootMotionEnabled.SetValue(rootMotion);
            this.AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID, ContextActionAnimation, () => { this.OnAnimationFinished(OnAnimationFinished); });
        }

        public void KillContextAction(SequencedAnimationInput ContextActionAnimation)
        {
            this.AnimatorPlayableObject.DestroyLayer(AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID);
        }

        private void OnAnimationFinished(Action parentCallback)
        {
            this.RootMotionEnabled.SetValue(false);
            parentCallback.Invoke();
        }

        private void OnRootMotionEnabled()
        {
            this.AnimatorPlayableObject.Animator.applyRootMotion = true;
        }

        private void OnRootMotionDisabled()
        {
            this.AnimatorPlayableObject.Animator.applyRootMotion = false;
        }
    }
}

/*
 * using System;
using System.Collections;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace CoreGame
{
    public class BaseCutsceneController
    {
        private AnimationDataManager _animationDataManager;
        private NavMeshAgent Agent;
        private Animator Animator;
        private ObjectRotateManager ObjectRotateManager;

        #region Data Components Dependencies

        private TransformMoveManagerComponentV3 PlayerInputMoveManagerComponentV3;

        #endregion

        private POICutsceneMoveManager POICutsceneMoveManager;
        private Rigidbody Rigidbody;

        public BaseCutsceneController()
        {
        }

        public BaseCutsceneController(Rigidbody rigidbody, NavMeshAgent agent, Animator animator)
        {
            Rigidbody = rigidbody;
            Agent = agent;
            Animator = animator;
        }

        protected void BaseInit(Rigidbody rigidBody, NavMeshAgent agent, Animator animator, TransformMoveManagerComponentV3 transformMoveManagerComponent = null, AnimationDataManager animationDataManager = null)
        {
            #region Data Components Dependencies

            PlayerInputMoveManagerComponentV3 = transformMoveManagerComponent;

            #endregion

            Rigidbody = rigidBody;
            Agent = agent;
            Animator = animator;

            //If we want the controller to not move agent/rb
            if (PlayerInputMoveManagerComponentV3 != null) POICutsceneMoveManager = new POICutsceneMoveManager(Rigidbody, Agent);

            _animationDataManager = animationDataManager;
            ObjectRotateManager = new ObjectRotateManager(rigidBody);
        }

        public void Tick(float d)
        {
            this.POICutsceneMoveManager.IfNotNull((POICutsceneMoveManager) => POICutsceneMoveManager.Tick(d, PlayerInputMoveManagerComponentV3.SpeedMultiplicationFactor, PlayerInputMoveManagerComponentV3.RotationSpeed));
            _animationDataManager.IfNotNull((PlayerAnimationDataManager) => PlayerAnimationDataManager.Tick(GetCurrentNormalizedSpeedMagnitude()));
            ObjectRotateManager.Tick(d);
        }

        public void Warp(Transform warpPosition)
        {
            Agent.Warp(warpPosition.position);
            Agent.transform.position = warpPosition.position;
            Agent.transform.rotation = warpPosition.rotation;
        }

        public IEnumerator SetAIDestination(Transform destination, float normalizedSpeed, AnimationCurve speedFactorOverDistance)
        {
            yield return POICutsceneMoveManager.SetDestination(destination, normalizedSpeed, speedFactorOverDistance);
            //Force position to ensure the destination is correctly reached
            Warp(destination);
        }

        public IEnumerator PlayAnimationAndWait(AnimationID animationID, float crossFadeDuration, Func<IEnumerator> animationEndCallback, bool updateModelImmediately, bool framePerfectEndDetection)
        {
            isAnimationPlaying = true;
            yield return AnimationPlayerHelper.PlayAndWait(Animator, CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration, animationEndCallback, updateModelImmediately, framePerfectEndDetection);
            isAnimationPlaying = false;
        }

        public void Play(AnimationID animationID, float crossFadeDuration, bool updateModelImmediately)
        {
            AnimationPlayerHelper.Play(Animator, CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration, updateModelImmediately);
        }

        public void StopAnimation(AnimationID animationID)
        {
            AnimationPlayerHelper.Play(Animator, CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[AnimationID.ACTION_LISTENING], 0f);
            isAnimationPlaying = false;
        }

        public float GetCurrentNormalizedSpeedMagnitude()
        {
            return POICutsceneMoveManager.GetCurrentNormalizedSpeedMagnitude();
        }

        public void AskRotation(Quaternion targetRotation, float speed)
        {
            ObjectRotateManager.AskRotation(targetRotation, speed);
        }

        #region State 

        private bool askedForWarp;
        private bool isAnimationPlaying;
        public bool IsAnimationPlaying => isAnimationPlaying;

        public bool IsDirectedByAi()
        {
            if (POICutsceneMoveManager == null)
                return false;
            else
                return POICutsceneMoveManager.IsDirectedByAi;
        }

        public bool IsRotating()
        {
            return ObjectRotateManager.IsRotating;
        }

        public bool IsCutscenePlaying()
        {
            return IsAnimationPlaying || IsDirectedByAi() || IsRotating();
        }

        #endregion
    }

    internal class POICutsceneMoveManager
    {
        private float currentPathTotalDistance;
        private float distanceAttenuatedNormalizedSpeedMagnitude;

        private bool isDirectedByAi;
        private float normalizedSpeedMagnitude = 1f;
        private NavMeshAgent playerAgent;
        private Rigidbody PlayerRigidBody;
        private AnimationCurve speedFactorOverDistance;

        public POICutsceneMoveManager(Rigidbody playerRigidBody, NavMeshAgent playerAgent)
        {
            PlayerRigidBody = playerRigidBody;
            this.playerAgent = playerAgent;
        }

        public bool IsDirectedByAi => isDirectedByAi;


        public void Tick(float d, float SpeedMultiplicationFactor, float AIRotationSpeed)
        {
            if (isDirectedByAi)
            {
                if (playerAgent.velocity.normalized != Vector3.zero)
                {
                    PlayerRigidBody.transform.rotation = Quaternion.Slerp(PlayerRigidBody.transform.rotation, Quaternion.LookRotation(playerAgent.velocity.normalized), d * AIRotationSpeed);
                    //only rotate on world z axis
                    var axis = Vector3.up;
                    PlayerRigidBody.transform.eulerAngles = new Vector3(PlayerRigidBody.transform.eulerAngles.x * axis.x, PlayerRigidBody.transform.eulerAngles.y * axis.y, PlayerRigidBody.transform.eulerAngles.z * axis.z);
                }

                var playerMovementOrientation = (playerAgent.nextPosition - PlayerRigidBody.transform.position).normalized;
                playerAgent.speed = SpeedMultiplicationFactor * normalizedSpeedMagnitude;
                distanceAttenuatedNormalizedSpeedMagnitude = normalizedSpeedMagnitude;

                if (speedFactorOverDistance != null)
                {
                    if (currentPathTotalDistance == 0f)
                    {
                        var pathCorners = playerAgent.path.corners;
                        for (var i = 1; i < pathCorners.Length; i++) currentPathTotalDistance += Vector3.Distance(pathCorners[i - 1], pathCorners[i]);
                    }
                    else
                    {
                        var distanceAttanuationFacotr = speedFactorOverDistance.Evaluate(Mathf.Clamp01(1 - playerAgent.remainingDistance / currentPathTotalDistance));
                        playerAgent.speed *= distanceAttanuationFacotr;
                        distanceAttenuatedNormalizedSpeedMagnitude *= distanceAttanuationFacotr;
                    }
                }

                PlayerRigidBody.transform.position = playerAgent.nextPosition;
            }
        }

        public float GetCurrentNormalizedSpeedMagnitude()
        {
            if (isDirectedByAi)
                return distanceAttenuatedNormalizedSpeedMagnitude;
            else
                return 0f;
        }

        public IEnumerator SetDestination(Transform destination, float normalizedSpeed, AnimationCurve speedFactorOverDistance)
        {
            currentPathTotalDistance = 0f;
            isDirectedByAi = true;
            normalizedSpeedMagnitude = normalizedSpeed;
            this.speedFactorOverDistance = speedFactorOverDistance;
            playerAgent.nextPosition = PlayerRigidBody.transform.position;
            playerAgent.SetDestination(destination.position);
            CutsceneControllerHelper.DisableRigidBodyForAnimation(PlayerRigidBody);
            //Let the AI move
            yield return Coroutiner.Instance.StartCoroutine(new WaitForNavAgentDestinationReached(playerAgent));
            CutsceneControllerHelper.EnableRigidBodyForAnimation(PlayerRigidBody);
            playerAgent.ResetPath();
            isDirectedByAi = false;
        }
    }

    internal class ObjectRotateManager
    {
        private bool isRotating;
        private Rigidbody rotatingRigidBody;
        private float speed;
        private Quaternion targetQuaternion;

        public ObjectRotateManager(Rigidbody rotatingRigidBody)
        {
            this.rotatingRigidBody = rotatingRigidBody;
        }

        public bool IsRotating => isRotating;

        public void AskRotation(Quaternion targetQuaternion, float speed)
        {
            this.targetQuaternion = targetQuaternion;
            this.speed = speed;
            isRotating = true;
        }

        public void Tick(float d)
        {
            if (isRotating)
            {
                rotatingRigidBody.transform.rotation = Quaternion.Slerp(rotatingRigidBody.rotation, targetQuaternion, speed * d);
                if (QuaterionHelper.ApproxEquals(rotatingRigidBody.transform.rotation, targetQuaternion))
                {
                    rotatingRigidBody.transform.eulerAngles = targetQuaternion.eulerAngles;
                    isRotating = false;
                }

                //     Debug.Log(this.rotatingRigidBody.transform.eulerAngles.ToString("F4"));
            }
        }
    }

    public static class CutsceneControllerHelper
    {
        public static void DisableRigidBodyForAnimation(Rigidbody rigidbody)
        {
            if (rigidbody != null) rigidbody.isKinematic = true;
        }

        public static void EnableRigidBodyForAnimation(Rigidbody rigidbody)
        {
            if (rigidbody != null) rigidbody.isKinematic = false;
        }
    }
}
*/