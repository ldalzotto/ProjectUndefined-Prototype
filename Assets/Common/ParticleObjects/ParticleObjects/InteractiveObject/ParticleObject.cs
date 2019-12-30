using UnityEngine;

namespace ParticleObjects
{
    public class ParticleObject
    {
        public bool IsAskingToBeDestroyed;

        [VE_Ignore] public bool IsUpdatedInMainManager;

        public ParticleGameObject ParticleGameObject { get; private set; }

        public ParticleObject(ParticleObjectDefinition ParticleObjectDefinition, ParticleGameObject particleGameObject, bool IsUpdatedInMainManager = true)
        {
            this.ParticleGameObject = particleGameObject;
            this.IsUpdatedInMainManager = IsUpdatedInMainManager;
            
            ParticleObjectsManager.Get().OnParticleObjectCreated(this);
        }


        public void Tick(float d)
        {
            if (this.ParticleGameObject.RootGameObject == null)
            {
                this.IsAskingToBeDestroyed = true;
            }
        }

        public void Destroy()
        {
            GameObject.Destroy(this.ParticleGameObject.RootGameObject);
        }
    }
}