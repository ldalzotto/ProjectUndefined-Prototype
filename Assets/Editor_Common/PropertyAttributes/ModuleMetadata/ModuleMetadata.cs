using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
public class ModuleMetadata : PropertyAttribute
{
    private string header;
    private string shortDescription;

    public ModuleMetadata(string header, string shortDescription = "")
    {
        this.header = header;
        this.shortDescription = shortDescription;
    }

    public string Header { get => header; }
    public string ShortDescription { get => shortDescription; }
}
