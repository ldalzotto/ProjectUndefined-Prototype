namespace InteractiveObjects_Interfaces
{
    public abstract class AInteractiveObjectSystem
    {
        public virtual void Tick(float d)
        {
        }

        public virtual void AfterTicks()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}