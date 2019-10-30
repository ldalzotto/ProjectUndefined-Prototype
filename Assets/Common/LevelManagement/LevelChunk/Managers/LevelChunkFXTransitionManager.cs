using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace LevelManagement
{
    public class LevelChunkFXTransitionManager : GameSingleton<LevelChunkFXTransitionManager>
    {
        private CurrentTransitionableLevelFXTypeManager CurrentTransitionableLevelFXTypeManager;
        private FXTransitionAnimationManager FXTransitionAnimationManager;

        private List<LevelChunkInteractiveObject> currentInsideTracker = new List<LevelChunkInteractiveObject>();

        public void Init()
        {
            this.CurrentTransitionableLevelFXTypeManager = new CurrentTransitionableLevelFXTypeManager();
        }

        #region External Events

        public void OnChunkLevelEnter(LevelChunkInteractiveObject nextLevelChunkTracker)
        {
            if (!this.currentInsideTracker.Contains(nextLevelChunkTracker))
            {
                this.currentInsideTracker.Add(nextLevelChunkTracker);
                this.CurrentTransitionableLevelFXTypeManager.OnChunkLevelEnter(nextLevelChunkTracker);
            }
        }

        public void OnChunkLevelExit(LevelChunkInteractiveObject levelChunkTracker)
        {
            this.currentInsideTracker.Remove(levelChunkTracker);
            if (this.currentInsideTracker.Count == 1)
            {
                //If when there is only one chunk tracker, the current tracker considered is not the last one
                if (!this.CurrentTransitionableLevelFXTypeManager.IsCurrentChunkTrackerEqualsTo(this.currentInsideTracker[0]))
                {
                    //we transition
                    this.CurrentTransitionableLevelFXTypeManager.OnChunkLevelEnter(this.currentInsideTracker[0]);
                }
            }
        }

        #endregion

        public void Tick(float d)
        {
            this.CurrentTransitionableLevelFXTypeManager.Tick(d);
        }
    }

    class CurrentTransitionableLevelFXTypeManager
    {
        public CurrentTransitionableLevelFXTypeManager()
        {
            this.AnimationManagers = new List<FXTransitionAnimationManager>()
            {
                new FXTransitionAnimationManager(new PostProcessingTransitionAnimationManager()),
                new FXTransitionAnimationManager(new MainDirectionalLightTransitionAnimationManager())
            };
        }

        private TransitionableLevelFXType current;
        private TransitionableLevelFXType old;

        private List<FXTransitionAnimationManager> AnimationManagers;

        public void OnChunkLevelEnter(LevelChunkInteractiveObject nextLevelChunkInteractiveObject)
        {
            if (old == null)
            {
                this.old = nextLevelChunkInteractiveObject.GetTransitionableLevelFXType();
                Debug.Log(MyLog.Format("SAME"));
                this.OnNewChunkLevel(this.old, this.current, forceInstantTransition: true);
            }
            else if (current == null)
            {
                if (this.old != nextLevelChunkInteractiveObject.GetTransitionableLevelFXType())
                {
                    this.current = nextLevelChunkInteractiveObject.GetTransitionableLevelFXType();
                    this.OnNewChunkLevel(this.old, this.current);
                }
            }
            else
            {
                if (this.current != nextLevelChunkInteractiveObject.GetTransitionableLevelFXType())
                {
                    this.old = this.current;
                    this.current = nextLevelChunkInteractiveObject.GetTransitionableLevelFXType();
                    this.OnNewChunkLevel(this.old, this.current);
                }
            }
        }

        public void Tick(float d)
        {
            foreach (var AnimationManager in this.AnimationManagers)
            {
                AnimationManager.Tick(d);
            }
        }

        #region Logical conditions

        public bool IsCurrentChunkTrackerEqualsTo(LevelChunkInteractiveObject compareChunkInteractiveObject)
        {
            return this.current != null && this.current == compareChunkInteractiveObject.GetTransitionableLevelFXType();
        }

        #endregion

        private void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current, bool forceInstantTransition = false)
        {
            foreach (var AnimationManager in this.AnimationManagers)
            {
                bool isSmooth = !forceInstantTransition && AnimationManager.IsNextDifferent(old, current);
                AnimationManager.OnNewChunkLevel(old, current, isSmooth ? ChunkFXTransitionType.SMOOTH : ChunkFXTransitionType.INSTANT);
            }
        }
    }

    class FXTransitionAnimationManager
    {
        #region State

        private bool isTransitioning;

        #endregion

        private float elapsedTime;
        private const float MAX_TIME = 1f;

        private TransitionableLevelFXType old;
        private TransitionableLevelFXType current;

        private ITransitionAnimationManager associatedAnimationManager;

        public FXTransitionAnimationManager(ITransitionAnimationManager associatedAnimationManager)
        {
            this.associatedAnimationManager = associatedAnimationManager;
        }

        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current, ChunkFXTransitionType TransitionType)
        {
            this.old = old;
            this.current = current;

            this.associatedAnimationManager.OnNewChunkLevel(old, current);

            if (current == null)
            {
                this.isTransitioning = false;
            }
            else
            {
                if (TransitionType == ChunkFXTransitionType.SMOOTH)
                {
                    if (this.isTransitioning)
                    {
                        this.OnTimeElapsingReverse();
                    }
                    else
                    {
                        this.isTransitioning = true;
                        this.ResetState();
                    }
                }
                else
                {
                    this.OnTransitionEnd();
                }
            }
        }

        private void ResetState()
        {
            this.elapsedTime = 0f;
            this.associatedAnimationManager.ResetState();
        }

        public void Tick(float d)
        {
            if (this.isTransitioning)
            {
                this.elapsedTime += d;
                float completionPercent = this.elapsedTime / MAX_TIME;
                if (IsTransitionFinished(completionPercent))
                {
                    this.OnTransitionEnd();
                }
                else
                {
                    this.associatedAnimationManager.UpdateAnimatedData(completionPercent, this.old, this.current);
                }
            }
        }

        private bool IsTransitionFinished(float completionPercent)
        {
            return completionPercent >= 1;
        }

        private void OnTransitionEnd()
        {
            this.associatedAnimationManager.OnTransitionEnd(this.old, this.current);
            this.isTransitioning = false;
        }

        private void OnTimeElapsingReverse()
        {
            this.associatedAnimationManager.OnTimeElapsingReverse(this.old, this.current);
            this.elapsedTime = 0f;
        }

        public bool IsNextDifferent(TransitionableLevelFXType current, TransitionableLevelFXType next)
        {
            return this.associatedAnimationManager.IsNextDifferent(current, next);
        }
    }

    interface ITransitionAnimationManager
    {
        void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current);
        void OnTimeElapsingReverse(TransitionableLevelFXType old, TransitionableLevelFXType current);
        void UpdateAnimatedData(float completionPercent, TransitionableLevelFXType old, TransitionableLevelFXType current);
        void ResetState();
        void OnTransitionEnd(TransitionableLevelFXType old, TransitionableLevelFXType current);
        bool IsNextDifferent(TransitionableLevelFXType current, TransitionableLevelFXType next);
    }

    class PostProcessingTransitionAnimationManager : ITransitionAnimationManager
    {
        private float oldPostProcessingStartingWeight = 1f;
        private float currentPostProcessingStartingWeight = 0f;

        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            old.PostProcessVolume.gameObject.SetActive(true);
            if (current != null)
            {
                current.PostProcessVolume.gameObject.SetActive(true);
            }
        }

        public void OnTimeElapsingReverse(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            //we reverse PP weight to have continuity in weight
            this.oldPostProcessingStartingWeight = old.PostProcessVolume.weight;
            this.currentPostProcessingStartingWeight = current.PostProcessVolume.weight;
        }

        public void UpdateAnimatedData(float completionPercent, TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            //the completionPercent - 0.3f is for delaying the old postprecessing transition -> causing artifacts
            old.PostProcessVolume.weight = Mathf.SmoothStep(this.oldPostProcessingStartingWeight, 0, completionPercent - 0.3f);
            current.PostProcessVolume.weight = Mathf.SmoothStep(this.currentPostProcessingStartingWeight, 1, completionPercent);
        }

        public void ResetState()
        {
            this.oldPostProcessingStartingWeight = 1f;
            this.currentPostProcessingStartingWeight = 0f;
        }

        public void OnTransitionEnd(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            old.PostProcessVolume.gameObject.SetActive(false);
            current.PostProcessVolume.weight = 1f;
            old.PostProcessVolume.weight = 0f;
        }

        public bool IsNextDifferent(TransitionableLevelFXType current, TransitionableLevelFXType next)
        {
            return current.PostProcessVolume.sharedProfile.name != next.PostProcessVolume.sharedProfile.name;
        }
    }

    class MainDirectionalLightTransitionAnimationManager : ITransitionAnimationManager
    {
        private float oldLightIntensity;
        private float currentLightIntensity;

        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            old.MainDirectionalLight.gameObject.SetActive(true);
            old.MainDirectionalLight.enabled = true;

            if (current != null)
            {
                current.MainDirectionalLight.gameObject.SetActive(true);
                current.MainDirectionalLight.enabled = true;
            }
        }

        public void OnTimeElapsingReverse(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            this.oldLightIntensity = old.MainDirectionalLight.intensity;
            this.currentLightIntensity = current.MainDirectionalLight.intensity;
        }

        public void UpdateAnimatedData(float completionPercent, TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            old.MainDirectionalLight.intensity = Mathf.SmoothStep(this.oldLightIntensity, 0, completionPercent);
            current.MainDirectionalLight.intensity = Mathf.SmoothStep(this.currentLightIntensity, 1, completionPercent);
        }

        public void ResetState()
        {
            this.oldLightIntensity = 1f;
            this.currentLightIntensity = 0f;
        }

        public void OnTransitionEnd(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            old.MainDirectionalLight.gameObject.SetActive(false);
            old.MainDirectionalLight.enabled = false;

            current.MainDirectionalLight.intensity = 1f;
            old.MainDirectionalLight.intensity = 0f;
        }

        public bool IsNextDifferent(TransitionableLevelFXType current, TransitionableLevelFXType next)
        {
            return (current.MainDirectionalLight.intensity != next.MainDirectionalLight.intensity)
                   || (current.MainDirectionalLight.color != next.MainDirectionalLight.color);
        }
    }

    public enum ChunkFXTransitionType
    {
        INSTANT,
        SMOOTH
    }
}