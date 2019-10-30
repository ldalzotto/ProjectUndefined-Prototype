using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public static class RendererRetrievableHelper
    {
        public static List<Renderer> GetAllRederers(GameObject rootObject, bool particleRenderers)
        {
            List<Renderer> returnList = new List<Renderer>();
            var foundRenderers = rootObject.GetComponentsInChildren<Renderer>();
            foreach (var foundRenderer in foundRenderers)
            {
                if (!particleRenderers)
                {
                    if (foundRenderer.GetType() == typeof(ParticleSystemRenderer))
                    {
                        continue;
                    }
                }

                returnList.Add(foundRenderer);
            }
            return returnList;
        }
    }
}
