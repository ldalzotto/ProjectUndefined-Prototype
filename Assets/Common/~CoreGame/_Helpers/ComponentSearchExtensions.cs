using System.Collections.Generic;
using UnityEngine;

public static class ComponentSearchExtensions
{
    public static GameObject FindChildObjectRecursively(this GameObject gameObject, string name)
    {
        foreach (Transform childTransform in gameObject.transform)
        {
            if (childTransform.name == name)
            {
                return childTransform.gameObject;
            }
            else
            {
                if (childTransform.childCount > 0)
                {
                    var recursiveResult = childTransform.gameObject.FindChildObjectRecursively(name);
                    if (recursiveResult != null)
                    {
                        return recursiveResult;
                    }
                }
            }
        }
        return null;
    }

    public static GameObject FindChildObjectWithLevelLimit(this GameObject gameObject, string name, int maxLevelDownIncluded)
    {
        return FindChildObjectWithLevelLimit(gameObject, name, maxLevelDownIncluded, 0);
    }

    private static GameObject FindChildObjectWithLevelLimit(GameObject gameObject, string name, int maxLevelDownIncluded, int currentLevel)
    {
        if (currentLevel > maxLevelDownIncluded)
        {
            return null;
        }

        foreach (Transform childTransform in gameObject.transform)
        {
            if (childTransform.name == name)
            {
                return childTransform.gameObject;
            }
            else
            {
                if (childTransform.childCount > 0)
                {
                    var newCurrentLevel = currentLevel + 1;
                    var recursiveResult = FindChildObjectWithLevelLimit(childTransform.gameObject, name, maxLevelDownIncluded, newCurrentLevel);
                    if (recursiveResult != null)
                    {
                        return recursiveResult;
                    }
                }
            }
        }
        return null;
    }

    public static List<GameObject> FindOneLevelDownChilds(this GameObject gameObject)
    {
        List<GameObject> returnValue = new List<GameObject>();
        foreach (Transform childTransform in gameObject.transform)
        {
            returnValue.Add(childTransform.gameObject);
        }
        return returnValue;
    }

    public static List<T> GetComponentsInCurrentAndChild<T>(this Component component) where T : Component
    {
        List<T> foundComponents = new List<T>();
        var sameLevelComponent = component.GetComponent<T>();
        if (sameLevelComponent != null) { foundComponents.Add(sameLevelComponent); }
        var childComponents = component.GetComponentsInChildren<T>();
        if(childComponents != null) { foundComponents.AddRange(childComponents); }
        return foundComponents;
    }

}
