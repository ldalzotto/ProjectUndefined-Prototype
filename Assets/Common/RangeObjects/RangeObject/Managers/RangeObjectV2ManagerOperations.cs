using InteractiveObjects;

namespace RangeObjects
{
    public static class RangeObjectV2ManagerOperations
    {
        public static void ClearAllReferencesOfInteractiveObject(CoreInteractiveObject InteractiveObject)
        {
            foreach (var rangeObject in RangeObjectV2Manager.Get().RangeObjects)
            {
                if (rangeObject.RangeIntersectionV2System != null)
                {
                    var RangeIntersectionListeners = rangeObject.RangeIntersectionV2System.RangeIntersectionListeners;
                    if (RangeIntersectionListeners != null)
                    {
                        foreach (var RangeIntersectionListener in RangeIntersectionListeners)
                        {
                            RangeIntersectionListener.RemoveReferencesToInteractiveObject(InteractiveObject);
                        }
                    }
                }
            }
        }
    }
}