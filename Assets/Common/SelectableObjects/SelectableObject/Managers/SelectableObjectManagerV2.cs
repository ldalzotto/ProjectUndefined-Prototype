using System.Collections.Generic;
using CoreGame;
using Input;
using SelectableObjects_Interfaces;

namespace SelectableObject
{
    public class SelectableObjectManagerV2 : GameSingleton<SelectableObjectManagerV2>
    {
        #region Internal Managers

        internal SelectableObjectRendererManager SelectableObjectRendererManager { get; private set; }

        #endregion

        #region Data Retrieval

        public ISelectableObjectSystem GetCurrentSelectedObject()
        {
            return CurrentSelectedObject;
        }

        #endregion

        public void Init(GameInputManager GameInputManager)
        {
            #region Exnternal Dependencies

            this.GameInputManager = GameInputManager;

            #endregion

            SelectableObjectRendererManager = new SelectableObjectRendererManager();
            interactableObjects = new List<ISelectableObjectSystem>();
        }

        public virtual void Tick(float d)
        {
            if (interactableObjects.Count > 0 && CurrentSelectedObject == null) SetCurrentSelectedObject(interactableObjects[0]);

            if (CurrentSelectedObject != null && GameInputManager.CurrentInput.SwitchSelectionButtonD()) SwitchSelection();

            if (interactableObjects.Count == 0) SetCurrentSelectedObject(default(ISelectableObjectSystem));

            SelectableObjectRendererManager.Tick(d, GetCurrentSelectedObject(), interactableObjects.Count > 1);
        }

        private void SwitchSelection()
        {
            var currentSelectedIndex = interactableObjects.IndexOf(CurrentSelectedObject);
            var nextSelectedIndex = currentSelectedIndex + 1;
            if (nextSelectedIndex == interactableObjects.Count) nextSelectedIndex = 0;

            SetCurrentSelectedObject(interactableObjects[nextSelectedIndex]);
        }

        private void SetCurrentSelectedObject(ISelectableObjectSystem SelectableObject)
        {
            if (CurrentSelectedObject != null)
            {
                if (!CurrentSelectedObject.Equals(SelectableObject))
                {
                    SelectableObjectEventsManager.OnSelectableObjectNoMoreSelected(CurrentSelectedObject);
                    if (SelectableObject != null) SelectableObjectEventsManager.OnSelectableObjectSelected(SelectableObject);
                }
            }
            else if (CurrentSelectedObject == null)
            {
                if (SelectableObject != null) SelectableObjectEventsManager.OnSelectableObjectSelected(SelectableObject);
            }

            CurrentSelectedObject = SelectableObject;
        }

        internal void RemoveInteractiveObjectFromSelectable(ISelectableObjectSystem selectableObject)
        {
            if (CurrentSelectedObject != null && interactableObjects.Contains(CurrentSelectedObject)) SetCurrentSelectedObject(default(ISelectableObjectSystem));

            interactableObjects.Remove(selectableObject);
        }

        internal void OnSelectableObjectEnter(ISelectableObjectSystem selectableObject)
        {
            if (!interactableObjects.Contains(selectableObject)) interactableObjects.Add(selectableObject);
        }

        #region External Dependencies

        private GameInputManager GameInputManager;
        private SelectableObjectEventsManager SelectableObjectEventsManager = SelectableObjectEventsManager.Get();

        #endregion

        #region Internal State

        private ISelectableObjectSystem CurrentSelectedObject;
        private List<ISelectableObjectSystem> interactableObjects;

        #endregion
    }
}