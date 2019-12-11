using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableGraphConnectionTest : MonoBehaviour
{
    public AnimationClip FakeClip;

    private PlayableGraph PlayableGraph;
    private AnimationLayerMixerPlayable AnimationLayerMixerPlayable;

    public bool Switch;

    public bool Add;
    public bool Destroy;
    public int Index;

    private List<IPlayable> Layers = new List<IPlayable>();

    void Start()
    {
        var animator = this.GetComponent<Animator>();

        this.PlayableGraph = PlayableGraph.Create("Test");

        this.AnimationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(this.PlayableGraph, 0);
        this.AnimationLayerMixerPlayable.SetInputCount(0);
        var playableOutput = AnimationPlayableOutput.Create(this.PlayableGraph, "Animation", animator);
        playableOutput.SetSourcePlayable(this.AnimationLayerMixerPlayable);

        this.Layers.Add(AnimationMixerPlayable.Create(this.PlayableGraph));
        ((AnimationMixerPlayable) this.Layers[0]).AddInput(AnimationClipPlayable.Create(this.PlayableGraph, this.FakeClip), 0);
        ((AnimationMixerPlayable) this.Layers[0]).AddInput(AnimationClipPlayable.Create(this.PlayableGraph, this.FakeClip), 0);

        this.Layers.Add(AnimationClipPlayable.Create(this.PlayableGraph, this.FakeClip));


        this.Layers.Add(AnimationMixerPlayable.Create(this.PlayableGraph));
        ((AnimationMixerPlayable) this.Layers[2]).AddInput(AnimationClipPlayable.Create(this.PlayableGraph, this.FakeClip), 0);

        this.AnimationLayerMixerPlayable.AddInput((Playable) this.Layers[0], 0);
        this.AnimationLayerMixerPlayable.AddInput((Playable) this.Layers[1], 0);
        this.AnimationLayerMixerPlayable.AddInput((Playable) this.Layers[2], 0);

        this.PlayableGraph.Play();
    }

    void Update()
    {
        if (this.Switch)
        {
            this.Switch = false;

            this.AnimationLayerMixerPlayable.DisconnectInput(0);
            this.AnimationLayerMixerPlayable.DisconnectInput(1);

            this.AnimationLayerMixerPlayable.ConnectInput(0, (Playable) this.Layers[1], 0);
            this.AnimationLayerMixerPlayable.ConnectInput(1, (Playable) this.Layers[0], 0);
        }

        if (this.Destroy)
        {
            this.Destroy = false;
            var inputCount = this.AnimationLayerMixerPlayable.GetInputCount();
            this.AnimationLayerMixerPlayable.GetGraph().Disconnect(this.AnimationLayerMixerPlayable, this.Index);
            this.AnimationLayerMixerPlayable.SetInputCount(inputCount - 1);
        }

        /*
        if (this.Add)
        {
            this.Add = false;

            var animationClipPlayable = AnimationClipPlayable.Create(this.PlayableGraph, this.FakeClip);
            var inputCount = this.AnimationLayerMixerPlayable.GetInputCount();
            this.AnimationLayerMixerPlayable.SetInputCount(inputCount + 1);
            Debug.Log("Set to input : " + inputCount);
            this.PlayableGraph.Connect(animationClipPlayable, 0, this.AnimationLayerMixerPlayable, inputCount);
        }

        if (this.Destroy)
        {
            this.Destroy = false;

            //  var inputCount = this.AnimationLayerMixerPlayable.GetInputCount();
            this.AnimationLayerMixerPlayable.GetGraph().Disconnect(this.AnimationLayerMixerPlayable, this.Index);
            // this.AnimationLayerMixerPlayable.SetInputCount(inputCount - 1);
        }
        */
    }

    /*
     *     public static int AddInput<U, V>(
      this U playable,
      V sourcePlayable,
      int sourceOutputIndex,
      float weight = 0.0f)
      where U : struct, IPlayable
      where V : struct, IPlayable
    {
      int inputCount = playable.GetInputCount<U>();
      playable.SetInputCount<U>(inputCount + 1);
      playable.ConnectInput<U, V>(inputCount, sourcePlayable, sourceOutputIndex, weight);
      return inputCount;
    }
     */

    private void OnDestroy()
    {
        this.PlayableGraph.Destroy();
    }
}