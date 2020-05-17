using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CoreGame
{
    public static class NavMeshConstants 
    {
        public static Dictionary<NavMeshLayer, int> NavMeshLayerIndex = new Dictionary<NavMeshLayer, int>() {
            {NavMeshLayer.WALKABLE, 1 }
        };
    }

    public enum NavMeshLayer
    {
        WALKABLE
    }
}
