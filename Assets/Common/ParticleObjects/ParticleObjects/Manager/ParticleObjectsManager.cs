using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace ParticleObjects
{
    public class ParticleObjectsManager : GameSingleton<ParticleObjectsManager>
    {
        private List<ParticleObject> ParticleObjects = new List<ParticleObject>();

        public void Tick(float d)
        {
            for (var i = 0; i < this.ParticleObjects.Count; i++)
                if (this.ParticleObjects[i].IsUpdatedInMainManager)
                    this.ParticleObjects[i].Tick(d);

            this.DestroyObjectsIfTagged();
        }

        private void DestroyObjectsIfTagged()
        {
            List<ParticleObject> particlesObjectsToDestroy = null;
            foreach (var particleObject in ParticleObjects)
            {
                if (particleObject.IsAskingToBeDestroyed)
                {
                    if (particlesObjectsToDestroy == null)
                    {
                        particlesObjectsToDestroy = new List<ParticleObject>();
                    }

                    particlesObjectsToDestroy.Add(particleObject);
                }
            }

            if (particlesObjectsToDestroy != null)
            {
                foreach (var particleObjectToDestroy in particlesObjectsToDestroy)
                {
                    particleObjectToDestroy.Destroy();
                    this.ParticleObjects.Remove(particleObjectToDestroy);
                }
            }
        }

        public override void OnDestroy()
        {
            foreach (var particleObject in ParticleObjects)
            {
                particleObject.Destroy();
            }

            this.ParticleObjects.Clear();
        }

        public void OnParticleObjectCreated(ParticleObject ParticleObject)
        {
            this.ParticleObjects.Add(ParticleObject);
        }
    }
}