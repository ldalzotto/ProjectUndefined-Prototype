using CoreGame;
using UnityEngine;

namespace SelectableObjects_Interfaces
{
    public interface ISelectableObjectSystem
    {
        //TODO -> replace object with RTPPlayerAction ref
        object AssociatedPlayerAction { get; }
        ExtendedBounds GetAverageModelBoundLocalSpace();

        Transform GetTransform();
    }
}