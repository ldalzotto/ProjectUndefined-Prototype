namespace RangeObjects
{
    [SceneHandleDraw]
    [System.Serializable]
    public class RoundedFrustumRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested] public RoundedFrustumRangeTypeDefinition RoundedFrustumRangeTypeDefinition;
    }
}