namespace ConfigurationEditor
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class DictionaryEnumSearch : System.Attribute
    {
        private bool isObject;

        public DictionaryEnumSearch(bool isObject = false)
        {
            this.isObject = isObject;
        }

        public bool IsObject { get => isObject; }
    }
}
