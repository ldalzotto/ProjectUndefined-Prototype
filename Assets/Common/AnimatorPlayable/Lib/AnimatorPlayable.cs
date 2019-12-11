using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class AnimatorPlayableObject
    {
        public static AvatarMask DefaultAvatarMask = new AvatarMask();

        public PlayableGraph GlobalPlayableGraph { get; private set; }
        public AnimationLayerMixerPlayable AnimationLayerMixerPlayable { get; private set; }
        public Animator Animator { get; private set; }
        public Dictionary<int, MyAnimationLayer> AllAnimationLayersCurrentlyPlaying { get; private set; } = new Dictionary<int, MyAnimationLayer>();
        private List<MyAnimationLayer> OrderedByInputHandlerAnimationLayers = new List<MyAnimationLayer>();

        public AnimatorPlayableObject(string graphName, Animator animator)
        {
            this.Animator = animator;
            this.GlobalPlayableGraph = PlayableGraph.Create(graphName);
            this.GlobalPlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var playableOutput = AnimationPlayableOutput.Create(this.GlobalPlayableGraph, "Animation", animator);
            this.AnimationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(this.GlobalPlayableGraph);
            PlayableOutputExtensions.SetSourcePlayable(playableOutput, this.AnimationLayerMixerPlayable);
            this.GlobalPlayableGraph.Play();
        }

        public void PlayAnimation(int layerID, IAnimationInput animationInput, Action OnAnimationEnd = null, Func<float> InputWeightProvider = null)
        {
            if (animationInput.AnimationInputType == AnimationInputType.SEQUENCED)
            {
                this.PlaySequencedAnimation(layerID, animationInput as SequencedAnimationInput, OnAnimationEnd);
            }
            else if (animationInput.AnimationInputType == AnimationInputType.BLENDED)
            {
                this.PlayBlendedAnimation(layerID, animationInput as BlendedAnimationInput, InputWeightProvider);
            }
            else if (animationInput.AnimationInputType == AnimationInputType.TWODBLENDED)
            {
                this.PlayTwoDBlendedAnimation(layerID, animationInput as TwoDAnimationInput);
            }
        }

        public void Stop()
        {
            this.GlobalPlayableGraph.Stop();
        }

        public void Play()
        {
            this.GlobalPlayableGraph.Play();
        }

        private void PlayBlendedAnimation(int layerID, BlendedAnimationInput BlendedAnimationInput, Func<float> InputWeightProvider)
        {
            if (this.AllAnimationLayersCurrentlyPlaying.ContainsKey(layerID))
            {
                this.DestroyLayer(layerID);
            }

            BlendedAnimationLayer BlendedAnimationLayer = new BlendedAnimationLayer(this.GlobalPlayableGraph, this.AnimationLayerMixerPlayable, layerID,
                BlendedAnimationInput, InputWeightProvider);

            this.AllAnimationLayersCurrentlyPlaying[layerID] = BlendedAnimationLayer;
            this.OrderedByInputHandlerAnimationLayers.Add(BlendedAnimationLayer);

            this.SortLayers();

            PlayableExtensions.SetInputWeight(this.AnimationLayerMixerPlayable, this.AllAnimationLayersCurrentlyPlaying[layerID].Inputhandler, 1f);
        }

        private void PlayTwoDBlendedAnimation(int layerID, TwoDAnimationInput TwoDAnimationInput)
        {
            if (this.AllAnimationLayersCurrentlyPlaying.ContainsKey(layerID))
            {
                this.DestroyLayer(layerID);
            }

            TwoDBlendTree TwoDBlendTree = new TwoDBlendTree(layerID, this.GlobalPlayableGraph, this.AnimationLayerMixerPlayable, TwoDAnimationInput);

            this.AllAnimationLayersCurrentlyPlaying[layerID] = TwoDBlendTree;
            this.OrderedByInputHandlerAnimationLayers.Add(TwoDBlendTree);

            this.SortLayers();

            PlayableExtensions.SetInputWeight(this.AnimationLayerMixerPlayable, this.AllAnimationLayersCurrentlyPlaying[layerID].Inputhandler, 1f);
        }

        private void PlaySequencedAnimation(int layerID, SequencedAnimationInput SequencedAnimationInput, Action OnAnimationEnd)
        {
            if (this.AllAnimationLayersCurrentlyPlaying.ContainsKey(layerID))
            {
                this.DestroyLayer(layerID);
            }

            var SequencedAnimationLayer = new SequencedAnimationLayer(this.GlobalPlayableGraph, this.AnimationLayerMixerPlayable, layerID, SequencedAnimationInput, OnAnimationEnd);

            this.AllAnimationLayersCurrentlyPlaying[layerID] = SequencedAnimationLayer;
            this.OrderedByInputHandlerAnimationLayers.Add(SequencedAnimationLayer);

            this.SortLayers();

            PlayableExtensions.SetInputWeight(this.AnimationLayerMixerPlayable, SequencedAnimationLayer.Inputhandler, 1f);
        }

        public void Tick(float d)
        {
            List<int> animationLayersToDestroy = null;
            foreach (var blendedAnimationLayer in AllAnimationLayersCurrentlyPlaying)
            {
                blendedAnimationLayer.Value.Tick(d);
                if (blendedAnimationLayer.Value.AskedToBeDestoyed())
                {
                    if (animationLayersToDestroy == null)
                    {
                        animationLayersToDestroy = new List<int>();
                    }

                    animationLayersToDestroy.Add(blendedAnimationLayer.Key);
                }
            }

            if (animationLayersToDestroy != null)
            {
                foreach (var animationLayerToDestroy in animationLayersToDestroy)
                {
                    this.DestroyLayer(animationLayerToDestroy);
                }
            }
        }

        public void SetSpeed(float speed)
        {
            this.AnimationLayerMixerPlayable.SetSpeed(speed);
        }

        public void SetTwoDInputWeight(int layerId, Vector2 inputWeight)
        {
            this.AllAnimationLayersCurrentlyPlaying[layerId].SetTwoDInputWeight(inputWeight);
        }

        public void DestroyLayer(int layerID)
        {
            this.AllAnimationLayersCurrentlyPlaying.TryGetValue(layerID, out MyAnimationLayer animationLayer);
            if (animationLayer != null)
            {
                this.OrderedByInputHandlerAnimationLayers.Remove(this.AllAnimationLayersCurrentlyPlaying[layerID]);
                var animationLayerDestroyed = this.AllAnimationLayersCurrentlyPlaying[layerID];
                this.AllAnimationLayersCurrentlyPlaying.Remove(layerID);
                animationLayerDestroyed.Destroy(this.AnimationLayerMixerPlayable);

                /// We must update Input handler reference because one layer has been destroyed
                int destroyedInputHandler = animationLayerDestroyed.Inputhandler;

                foreach (var orderedLayer in OrderedByInputHandlerAnimationLayers)
                {
                    if (orderedLayer.Inputhandler > destroyedInputHandler)
                    {
                        var oldWeight = this.AnimationLayerMixerPlayable.GetInputWeight(orderedLayer.Inputhandler);
                        this.AnimationLayerMixerPlayable.DisconnectInput(orderedLayer.Inputhandler);
                        orderedLayer.Inputhandler -= 1;

                        this.AnimationLayerMixerPlayable.ConnectInput(orderedLayer.Inputhandler, orderedLayer.GetEntryPointMixerPlayable(), 0);
                        this.AnimationLayerMixerPlayable.SetLayerAdditive((uint) orderedLayer.Inputhandler, orderedLayer.IsLayerAdditive());
                        if (orderedLayer.GetLayerAvatarMask() != null)
                        {
                            this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) orderedLayer.Inputhandler, orderedLayer.GetLayerAvatarMask());
                        }
                        else
                        {
                            this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) orderedLayer.Inputhandler, DefaultAvatarMask);
                        }

                        this.AnimationLayerMixerPlayable.SetInputWeight(orderedLayer.Inputhandler, oldWeight);
                    }
                }
            }
        }

        private void SortLayers()
        {
            if (this.AllAnimationLayersCurrentlyPlaying.Count > 1)
            {
                //   bool layerRefreshNeeded = false;
                bool isSorted = false;
                while (!isSorted)
                {
                    isSorted = true;
                    for (var i = 0; i < this.OrderedByInputHandlerAnimationLayers.Count; i++)
                    {
                        if (i != this.OrderedByInputHandlerAnimationLayers.Count - 1)
                        {
                            if (this.OrderedByInputHandlerAnimationLayers[i].LayerID > this.OrderedByInputHandlerAnimationLayers[i + 1].LayerID)
                            {
                                this.SwitchLayers(this.OrderedByInputHandlerAnimationLayers[i].LayerID, this.OrderedByInputHandlerAnimationLayers[i + 1].LayerID);

                                var tmp = this.OrderedByInputHandlerAnimationLayers[i + 1];
                                this.OrderedByInputHandlerAnimationLayers[i + 1] = this.OrderedByInputHandlerAnimationLayers[i];
                                this.OrderedByInputHandlerAnimationLayers[i] = tmp;
                                isSorted = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SwitchLayers(int leftLayerID, int rightLayerID)
        {
            var leftLayerOnStart = this.AllAnimationLayersCurrentlyPlaying[leftLayerID];
            var rightLayerOnStart = this.AllAnimationLayersCurrentlyPlaying[rightLayerID];

            var isLeftLayerAdditive = leftLayerOnStart.IsLayerAdditive();
            var leftLayerMark = leftLayerOnStart.GetLayerAvatarMask();

            var isRightLiayerAdditive = rightLayerOnStart.IsLayerAdditive();
            var rightLayermask = rightLayerOnStart.GetLayerAvatarMask();

            var leftWeight = this.AnimationLayerMixerPlayable.GetInputWeight(leftLayerOnStart.Inputhandler);
            var rightWeight = this.AnimationLayerMixerPlayable.GetInputWeight(rightLayerOnStart.Inputhandler);

            this.AnimationLayerMixerPlayable.DisconnectInput(leftLayerOnStart.Inputhandler);
            this.AnimationLayerMixerPlayable.DisconnectInput(rightLayerOnStart.Inputhandler);

            this.AnimationLayerMixerPlayable.ConnectInput(rightLayerOnStart.Inputhandler, leftLayerOnStart.GetEntryPointMixerPlayable(), 0);
            this.AnimationLayerMixerPlayable.ConnectInput(leftLayerOnStart.Inputhandler, rightLayerOnStart.GetEntryPointMixerPlayable(), 0);

            this.AnimationLayerMixerPlayable.SetInputWeight(leftLayerOnStart.Inputhandler, rightWeight);
            this.AnimationLayerMixerPlayable.SetInputWeight(rightLayerOnStart.Inputhandler, leftWeight);

            this.AnimationLayerMixerPlayable.SetLayerAdditive((uint) leftLayerOnStart.Inputhandler, isRightLiayerAdditive);
            this.AnimationLayerMixerPlayable.SetLayerAdditive((uint) rightLayerOnStart.Inputhandler, isLeftLayerAdditive);

            if (rightLayermask != null)
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) leftLayerOnStart.Inputhandler, rightLayermask);
            }
            else
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) leftLayerOnStart.Inputhandler, DefaultAvatarMask);
            }

            if (leftLayerMark != null)
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) rightLayerOnStart.Inputhandler, leftLayerMark);
            }
            else
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) rightLayerOnStart.Inputhandler, DefaultAvatarMask);
            }

            var leftLayerInputHandler = leftLayerOnStart.Inputhandler;
            leftLayerOnStart.Inputhandler = rightLayerOnStart.Inputhandler;
            rightLayerOnStart.Inputhandler = leftLayerInputHandler;
        }

        public void Destroy()
        {
            this.GlobalPlayableGraph.Destroy();
        }
    }

    public abstract class MyAnimationLayer
    {
        public int Inputhandler;
        public int LayerID;
        public abstract void Tick(float d);
        public abstract bool AskedToBeDestoyed();

        protected AnimationLayerMixerPlayable ParentAnimationLayerMixerPlayable;

        public abstract AnimationMixerPlayable GetEntryPointMixerPlayable();

        public abstract AvatarMask GetLayerAvatarMask();
        public abstract bool IsLayerAdditive();

        protected MyAnimationLayer(int LayerID, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable)
        {
            this.LayerID = LayerID;
            ParentAnimationLayerMixerPlayable = parentAnimationLayerMixerPlayable;
        }

        public virtual void SetTwoDInputWeight(Vector2 inputWeihgt)
        {
        }

        public virtual void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
        {
            AnimationLayerMixerPlayable.DisconnectInput(this.Inputhandler);
        }
    }
}