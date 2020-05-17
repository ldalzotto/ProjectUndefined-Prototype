using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace SelectionWheel
{
    public class SelectionWheelObjectAnimation
    {
        private PlayableGraph AnimationGraph;

        private bool DoesGraphMustbeUpdated;

        private AnimationClipPlayable EnterTransitionAnimationClipPlayable;
        private Dictionary<SelectionWheelObjectAnimationVarableType, BoolVariable> SelectionWheelObjectAnimationVariables;

        public SelectionWheelObjectAnimation(SelectionWheelGameObject SelectionWheelGameObject, AnimationClip enterTransitionAnimationClip, Action OnExitAnimationCompleted)
        {
            SelectionWheelObjectAnimationVariables = new Dictionary<SelectionWheelObjectAnimationVarableType, BoolVariable>()
            {
                {SelectionWheelObjectAnimationVarableType.OpenAnimation, new BoolVariable(false, OnOpenAnimationStarted)},
                {
                    SelectionWheelObjectAnimationVarableType.CloseAnimation, new BoolVariable(false, OnCloseAnimationStarted, () =>
                    {
                        OnCloseAnimationFinished();
                        OnExitAnimationCompleted.Invoke();
                    })
                }
            };

            AnimationGraph = PlayableGraph.Create(GetType().Name);
            AnimationGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            EnterTransitionAnimationClipPlayable = AnimationClipPlayable.Create(AnimationGraph, enterTransitionAnimationClip);
            EnterTransitionAnimationClipPlayable.SetApplyFootIK(false);
            EnterTransitionAnimationClipPlayable.Pause();
            var animationGraphPlayableOutput = AnimationPlayableOutput.Create(AnimationGraph, "Animation", SelectionWheelGameObject.Animator);
            animationGraphPlayableOutput.SetSourcePlayable(EnterTransitionAnimationClipPlayable);

            AnimationGraph.Play();
        }

        public void LateTick(float d)
        {
            if (DoesGraphMustbeUpdated == true)
            {
                if (SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.OpenAnimation].GetValue())
                {
                    if (EnterTransitionAnimationClipPlayable.GetTime() >= EnterTransitionAnimationClipPlayable.GetAnimationClip().length) SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.OpenAnimation].SetValue(false);
                }
                else if (SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.CloseAnimation].GetValue())
                {
                    if (EnterTransitionAnimationClipPlayable.GetTime() <= 0f) SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.CloseAnimation].SetValue(false);
                }
            }
        }

        public void PlayEnterAnimation()
        {
            DoesGraphMustbeUpdated = true;
            SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.CloseAnimation].SetValue(false);
            SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.OpenAnimation].SetValue(true);
        }

        public void PlayExitAnimation()
        {
            DoesGraphMustbeUpdated = true;
            SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.OpenAnimation].SetValue(false);
            SelectionWheelObjectAnimationVariables[SelectionWheelObjectAnimationVarableType.CloseAnimation].SetValue(true);
        }

        private void OnOpenAnimationStarted()
        {
            EnterTransitionAnimationClipPlayable.SetSpeed(1f);
            EnterTransitionAnimationClipPlayable.SetTime(0f);
        }

        private void OnCloseAnimationStarted()
        {
            EnterTransitionAnimationClipPlayable.SetSpeed(-1f);
            EnterTransitionAnimationClipPlayable.SetTime(EnterTransitionAnimationClipPlayable.GetAnimationClip().length);
        }

        private void OnCloseAnimationFinished()
        {
            DoesGraphMustbeUpdated = false;
        }
    }

    public enum SelectionWheelObjectAnimationVarableType
    {
        OpenAnimation,
        CloseAnimation
    }
}