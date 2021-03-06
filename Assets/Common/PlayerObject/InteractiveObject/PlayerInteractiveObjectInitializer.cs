﻿using InteractiveObjects;

namespace PlayerObject
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [Inline()]
        [DrawNested]
        public PlayerInteractiveObjectDefinition PlayerInteractiveObjectDefinition;
        protected override CoreInteractiveObject InitializationLogic()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build_Allocate(gameObject), this.PlayerInteractiveObjectDefinition);
            return PlayerInteractiveObject;
        }
    }
}