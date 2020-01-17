using System;

namespace TimeManagement
{
    public class SimulatePhysicsTimeSteps
    {
        private float Timer;

        private Action<float, float> Callback;

        public SimulatePhysicsTimeSteps(Action<float, float> callback)
        {
            Callback = callback;
        }

        public bool Tick(float d, float unscaled)
        {
            this.Timer += unscaled;

            bool callbackCalled = false;
            
            while (this.Timer >= TimeManagementManager.Get().GetInitialPhysicsDeltaTime())
            {
                this.Timer -= TimeManagementManager.Get().GetInitialPhysicsDeltaTime();
                this.Callback.Invoke(d, unscaled);
                callbackCalled = true;
            }

            return callbackCalled;
        }

        public void Reset()
        {
            this.Timer = 0f;
        }
    }
}