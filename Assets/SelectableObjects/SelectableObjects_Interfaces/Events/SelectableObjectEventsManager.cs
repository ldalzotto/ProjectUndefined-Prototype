using System;
using CoreGame;

namespace SelectableObjects_Interfaces
{
    public class SelectableObjectEventsManager : GameSingleton<SelectableObjectEventsManager>
    {
        private event Action<ISelectableObjectSystem> OnSelectableObjectSelectedEvent;
        private event Action<ISelectableObjectSystem> OnSelectableObjectNoMoreSelectedEvent;

        public void RegisterOnSelectableObjectSelectedEventAction(Action<ISelectableObjectSystem> action)
        {
            OnSelectableObjectSelectedEvent += action;
        }

        public void RegisterOnSelectableObjectNoMoreSelectedEventAction(Action<ISelectableObjectSystem> action)
        {
            OnSelectableObjectNoMoreSelectedEvent += action;
        }

        public void OnSelectableObjectSelected(ISelectableObjectSystem selectableObject)
        {
            if (OnSelectableObjectSelectedEvent != null) OnSelectableObjectSelectedEvent.Invoke(selectableObject);
        }

        public void OnSelectableObjectNoMoreSelected(ISelectableObjectSystem selectableObject)
        {
            if (OnSelectableObjectNoMoreSelectedEvent != null) OnSelectableObjectNoMoreSelectedEvent.Invoke(selectableObject);
        }
    }
}