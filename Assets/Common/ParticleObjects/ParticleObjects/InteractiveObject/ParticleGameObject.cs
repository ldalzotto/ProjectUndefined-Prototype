using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using UnityEngine;

namespace ParticleObjects
{
    public class ParticleGameObject
    {
        public GameObject RootGameObject { get; private set; }
        public ParticleSystemObject RootParticleSystemObject { get; private set; }

        public static ParticleGameObject BuildFromGameObject(GameObject SourceGameObject)
        {
            var particleGameObject = new ParticleGameObject();

            particleGameObject.RootGameObject = SourceGameObject;

            GameObject rootParticleGameObject = null;

            var rootParticleSystem = SourceGameObject.GetComponent<ParticleSystem>();
            if (rootParticleSystem == null)
            {
                SourceGameObject.GetComponentInChildren<ParticleSystem>();
            }
            else
            {
                rootParticleGameObject = rootParticleSystem.gameObject;
            }

            particleGameObject.RootParticleSystemObject = ParticleSystemObject.BuildFromGameObject(rootParticleGameObject);
            return particleGameObject;
        }
    }

    public class ParticleSystemObject
    {
        private ParticleSystem AssociatedParticleSystem;

        public ParticleSystemObject(ParticleSystem associatedParticleSystem)
        {
            AssociatedParticleSystem = associatedParticleSystem;
            var mainModule = AssociatedParticleSystem.main;
            mainModule.stopAction = ParticleSystemStopAction.Destroy;
        }

        public static ParticleSystemObject BuildFromGameObject(GameObject SourceGameObject)
        {
            return new ParticleSystemObject(SourceGameObject.GetComponent<ParticleSystem>());
        }

        public bool IsOver()
        {
            return (!this.AssociatedParticleSystem.main.loop && this.AssociatedParticleSystem.time >= this.AssociatedParticleSystem.main.duration);
        }
    }
}