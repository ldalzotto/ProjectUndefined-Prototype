using System.Collections.Generic;
using AnimatorPlayable;
using UnityEngine;

#if UNITY_EDITOR
namespace InteractiveObject_Animation
{
    public class TwoDAnimationTest : MonoBehaviour
    {
        public A_AnimationPlayableDefinition BlendTree;
        private AnimatorPlayableObject AnimatorPlayableObject;
        private TwoDObjectAnimationPlayableSystem TwoDObjectAnimationPlayableSystem;

        public Vector2 Speed;
        private List<WeightSphere> WeightSpheres = new List<WeightSphere>();
        public Transform SamplePoint;

        private float[] Weights;

        private void Start()
        {
            var animator = this.GetComponent<Animator>();
            this.AnimatorPlayableObject = new AnimatorPlayableObject("Test", animator);
            this.TwoDObjectAnimationPlayableSystem = new TwoDObjectAnimationPlayableSystem(this.AnimatorPlayableObject, this.BlendTree);
        }

        private void Update()
        {
            this.TwoDObjectAnimationPlayableSystem.SetCurrentSpeed(this.Speed);
            this.AnimatorPlayableObject.Tick(Time.deltaTime);
        }
    }

    class WeightSphere
    {
        public Vector2 Pos;
        private GameObject obj;

        public WeightSphere(Vector2 Pos)
        {
            this.Pos = Pos;
            this.obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = this.Pos;
            MonoBehaviour.Destroy(obj.GetComponent<Rigidbody>());
        }

        public void Tick(float Weight)
        {
            this.obj.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white * Weight);
        }
    }
}
#endif