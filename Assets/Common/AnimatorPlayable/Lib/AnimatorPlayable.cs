using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class AnimatorPlayableObject
    {
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

                /*
                /// We must update Input handler reference because one layer has been destroyed
                int destroyedInputHandler = animationLayerDestroyed.Inputhandler;
                for (var i = 0; i < this.OrderedByInputHandlerAnimationLayers.Count; i++)
                {
                    if (this.OrderedByInputHandlerAnimationLayers[i].Inputhandler > destroyedInputHandler)
                    {
                        this.OrderedByInputHandlerAnimationLayers[i].Inputhandler -= 1;
                    }
                }
                */
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

                /*
                if (layerRefreshNeeded)
                {
                    foreach (var animationLayer in OrderedByInputHandlerAnimationLayers)
                    {
                        Debug.Log("Disconnect : " + animationLayer.Inputhandler);
                        this.AnimationLayerMixerPlayable.DisconnectInput(animationLayer.Inputhandler);
                    }

                    foreach (var orderedByInputHandlerAnimationLayer in this.OrderedByInputHandlerAnimationLayers)
                    {
                        this.AnimationLayerMixerPlayable.ConnectInput(orderedByInputHandlerAnimationLayer.Inputhandler, orderedByInputHandlerAnimationLayer.GetEntryPointMixerPlayable(), 0);
                    }
                }
                */
            }
        }

        private void SwitchLayers(int leftLayerID, int rightLayerID)
        {
            var isLeftLayerAdditive = this.AllAnimationLayersCurrentlyPlaying[leftLayerID].IsLayerAdditive();
            var leftLayerMark = this.AllAnimationLayersCurrentlyPlaying[leftLayerID].GetLayerAvatarMask();

            var isRightLiayerAdditive = this.AllAnimationLayersCurrentlyPlaying[rightLayerID].IsLayerAdditive();
            var rightLayermask = this.AllAnimationLayersCurrentlyPlaying[rightLayerID].GetLayerAvatarMask();

            var leftWeight = this.AnimationLayerMixerPlayable.GetInputWeight(this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler);
            var rightWeight = this.AnimationLayerMixerPlayable.GetInputWeight(this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler);

            this.AnimationLayerMixerPlayable.DisconnectInput(this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler);
            this.AnimationLayerMixerPlayable.DisconnectInput(this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler);

            this.AnimationLayerMixerPlayable.ConnectInput(this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler, AllAnimationLayersCurrentlyPlaying[leftLayerID].GetEntryPointMixerPlayable(), 0);
            this.AnimationLayerMixerPlayable.ConnectInput(this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler, AllAnimationLayersCurrentlyPlaying[rightLayerID].GetEntryPointMixerPlayable(), 0);

            this.AnimationLayerMixerPlayable.SetInputWeight(this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler, rightWeight);
            this.AnimationLayerMixerPlayable.SetInputWeight(this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler, leftWeight);
            
            this.AnimationLayerMixerPlayable.SetLayerAdditive((uint) this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler, isRightLiayerAdditive);
            this.AnimationLayerMixerPlayable.SetLayerAdditive((uint) this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler, isLeftLayerAdditive);

            if (rightLayermask != null)
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler, rightLayermask);
            }
            else
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler, new AvatarMask());
            }

            if (leftLayerMark != null)
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler, leftLayerMark);
            }
            else
            {
                this.AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint) this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler, new AvatarMask());
            }

            var leftLayerInputHandler = AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler;
            AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler = this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler;
            this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler = leftLayerInputHandler;
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
        
        public virtual void SetTwoDInputWeight(Vector2 inputWeihgt){}

        public virtual void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
        {
            AnimationLayerMixerPlayable.DisconnectInput(this.Inputhandler);
        }
    }
}