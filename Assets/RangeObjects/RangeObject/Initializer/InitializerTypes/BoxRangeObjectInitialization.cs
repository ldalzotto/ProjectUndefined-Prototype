namespace RangeObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    public class BoxRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested] public BoxRangeTypeDefinition BoxRangeTypeDefinition;
    }
}