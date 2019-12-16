using System.Collections.Generic;

namespace InteractiveObject_Animation
{
    public enum BipedArmatureName
    {
        ItemHold_L = 0
    }
    
    public static class BipedArmatureConstants
    {
        static Dictionary<BipedArmatureName, string> BipedArmatureBoneNames = new Dictionary<BipedArmatureName, string>()
        {
            {BipedArmatureName.ItemHold_L, "ItemHolding.L"}
        };

        public static string GetBipedBoneName(BipedArmatureName BipedArmatureName)
        {
            return BipedArmatureBoneNames[BipedArmatureName];
        }
    }
}