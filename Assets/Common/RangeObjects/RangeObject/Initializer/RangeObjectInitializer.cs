using UnityEngine;
using UnityEngine.Profiling;

namespace RangeObjects
{
    [SceneHandleDraw]
    public class RangeObjectInitializer : MonoBehaviour
    {
       [DrawNested] [Inline()] public RangeObjectInitialization RangeObjectInitialization;

        public void Init()
        {
            FromRangeObjectInitialization(RangeObjectInitialization, this.gameObject);
            MonoBehaviour.Destroy(this);
        }

        public static RangeObjectV2 FromRangeObjectInitialization(RangeObjectInitialization RangeObjectInitialization, GameObject parent)
        {
            RangeObjectV2 rangeObjectV2 = null;
            if (RangeObjectInitialization.GetType() == typeof(SphereRangeObjectInitialization))
            {
                rangeObjectV2 = new SphereRangeObjectV2(parent, (SphereRangeObjectInitialization) RangeObjectInitialization, null);
            }
            else if (RangeObjectInitialization.GetType() == typeof(BoxRangeObjectInitialization))
            {
                rangeObjectV2 = new BoxRangeObjectV2(parent, (BoxRangeObjectInitialization) RangeObjectInitialization, null);
            }
            else if (RangeObjectInitialization.GetType() == typeof(FrustumRangeObjectInitialization))
            {
                rangeObjectV2 = new FrustumRangeObjectV2(parent, (FrustumRangeObjectInitialization) RangeObjectInitialization, null);
            }
            else if (RangeObjectInitialization.GetType() == typeof(RoundedFrustumRangeObjectInitialization))
            {
                rangeObjectV2 = new RoundedFrustumRangeObjectV2(parent, (RoundedFrustumRangeObjectInitialization) RangeObjectInitialization, null);
            }

            return rangeObjectV2;
        }
    }
}