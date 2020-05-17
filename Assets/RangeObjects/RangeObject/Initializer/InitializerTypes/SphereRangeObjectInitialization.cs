namespace RangeObjects
{
    [SceneHandleDraw]
    [System.Serializable]
    public class SphereRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested] public SphereRangeTypeDefinition SphereRangeTypeDefinition;
    }
}