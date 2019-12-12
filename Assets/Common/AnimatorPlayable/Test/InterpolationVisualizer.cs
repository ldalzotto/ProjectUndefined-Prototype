using System;
using System.Collections.Generic;
using AnimatorPlayable;
using UnityEngine;

namespace Test.PlayableGraphConnectionTest
{
    public class InterpolationVisualizer : MonoBehaviour
    {
        public Material mat;
        public TwoDAnimationInput TwoDAnimationInput;

        public Vector2 Value;

        private List<GameObject> Gos = new List<GameObject>();

        private void Start()
        {
            //FreeformDirectionalInterpolator
            this.TwoDAnimationInput.TwoDBlendTreeAnimationClipInputs.ConvertAll(i => i.TreePosition).ForEach(pos =>
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = pos;
                go.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                MonoBehaviour.Destroy(go.GetComponent<Rigidbody>());
                MonoBehaviour.Destroy(go.GetComponent<Collider>());
                var matarial = new Material(mat);
                go.GetComponent<Renderer>().material = matarial;
                Gos.Add(go);
            });
        }

        private void Update()
        {
            var weights = new float[this.TwoDAnimationInput.TwoDBlendTreeAnimationClipInputs.Count];
            FreeformDirectionalInterpolator.SampleWeightsPolar(this.Value, this.TwoDAnimationInput.TwoDBlendTreeAnimationClipInputs.ConvertAll(i => i.TreePosition).ToArray(), ref weights);
            for (var i = 0; i < weights.Length; i++)
            {
                this.Gos[i].GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(weights[i], weights[i], weights[i]));
            }
        }
    }
}