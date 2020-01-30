using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PointerGeneration
{
    public static class PointerGenerationEditor
    { 
        public static string ClassPointerTemplateFilePath = "/Editor/PointerGeneration/ClassPointerTemplate.txt";
        public static string ClassPointerExtensionTemplatePath = "/Editor/PointerGeneration/ClassPointerExtensionTemplate.txt";
        public static string StructPointerTemplatePath =  "/Editor/PointerGeneration/StructPointerTemplate.txt";

        public static string[] JsonDefintionPaths = new string[]
        {
            "/Common/~CoreGame/PointerDefinitions.json",
            "/Undefined/Logic/SightVisualFeedback/PointerDefinitions.json",
            "/Common/AnimatorPlayable/PointerDefinitions.json",
            "/Common/InteractiveObjects/PointerDefinitions.json"
        };

        [MenuItem("Tools/PointerGenerationEditor")]
        public static void Generate()
        {
            foreach (var jsonDefintionPath in JsonDefintionPaths)
            {
                PointerGenerationBuffer PointerGenerationBuffer = new PointerGenerationBuffer();
                var PointerGenerationRoot = JsonUtility.FromJson<PointerGenerationRoot>(File.ReadAllText(Application.dataPath + jsonDefintionPath));

                if (!string.IsNullOrEmpty(PointerGenerationRoot.Namespace))
                {
                    if (PointerGenerationRoot.Classes != null && PointerGenerationRoot.Classes.Count > 0)
                    {
                        PointerGenerationBuffer.ExtensionBuffer = "";
                        PointerGenerationBuffer.ClassPointerBuffer = "";
                        foreach (var pointerGenerationDataClass in PointerGenerationRoot.Classes)
                        {
                            PointerGenerationBuffer.ExtensionBuffer += BuildExtension(RemoveNamespaceFromName(pointerGenerationDataClass.Name) + "Pointer", pointerGenerationDataClass.Name);
                            PointerGenerationBuffer.ClassPointerBuffer += BuildClassPointer(RemoveNamespaceFromName(pointerGenerationDataClass.Name) + "Pointer", pointerGenerationDataClass.Name);
                        }

                        PointerGenerationBuffer.ExtensionBuffer = ExtensionClass(PointerGenerationBuffer.ExtensionBuffer);
                    }

                    if (PointerGenerationRoot.Structs != null && PointerGenerationRoot.Structs.Count > 0)
                    {
                        PointerGenerationBuffer.StructPointerBuffer = "";
                        foreach (var pointerGenerationDataStruct in PointerGenerationRoot.Structs)
                        {
                            PointerGenerationBuffer.StructPointerBuffer += BuildStructPointer(RemoveNamespaceFromName(pointerGenerationDataStruct.Name) + "Pointer", pointerGenerationDataStruct.Name);
                        }
                    }

                    File.WriteAllText((Application.dataPath + jsonDefintionPath).Replace(".json", ".cs"),
                        BuildFinalClass(PointerGenerationRoot.Namespace,
                            PointerGenerationBuffer.ExtensionBuffer + PointerGenerationBuffer.ClassPointerBuffer + PointerGenerationBuffer.StructPointerBuffer));
                }
            }
        }

        private static string RemoveNamespaceFromName(string initialClassName)
        {
            var splittedName = initialClassName.Split('.');
            if (splittedName != null)
            {
                return splittedName[splittedName.Length - 1];
            }
            else
            {
                return initialClassName;
            }
        }


        [Serializable]
        public struct PointerGenerationRoot
        {
            public string Namespace;
            public List<PointerGenerationData> Structs;
            public List<PointerGenerationData> Classes;
        }

        [Serializable]
        public struct PointerGenerationData
        {
            public string Name;
        }


        public struct PointerGenerationBuffer
        {
            public string ExtensionBuffer;
            public string ClassPointerBuffer;
            public string StructPointerBuffer;
        }

        #region Extension

        public static string ExtensionClass(string GeneratedExtensions)
        {
            return "public static class ClassPointerExtension" +
                   "\n{" +
                   "\n" + GeneratedExtensions +
                   "\n}";
        }

        public static string BuildExtension(string ClassPointerName, string InitialClassName)
        {
            return File.ReadAllText(Application.dataPath + ClassPointerExtensionTemplatePath)
                .Replace("${PointerClassName}", ClassPointerName)
                .Replace("${TypeName}", InitialClassName);
        }

        #endregion

        #region Class Pointers

        public static string BuildClassPointer(string ClassPointerName, string InitialClassName)
        {
            return File.ReadAllText(Application.dataPath + ClassPointerTemplateFilePath)
                .Replace("${PointerClassName}", ClassPointerName)
                .Replace("${TypeName}", InitialClassName);
        }

        #endregion

        #region Struct Pointers

        public static string BuildStructPointer(string StructPointerName, string InitialClassName)
        {
            return File.ReadAllText(Application.dataPath + StructPointerTemplatePath)
                .Replace("${PointerClassName}", StructPointerName)
                .Replace("${TypeName}", InitialClassName);
        }

        #endregion

        #region Namespace

        public static string BuildFinalClass(string nmsp, string content)
        {
            return "using System;"
                   + "\nusing System.Runtime.InteropServices;"
                   + "\nnamespace " + nmsp + ""
                   + "\n{"
                   + "\n " + content
                   + "\n}";
        }

        #endregion
    }
}