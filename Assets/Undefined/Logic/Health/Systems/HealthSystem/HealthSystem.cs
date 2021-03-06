﻿using System;
using InteractiveObjects;
using UnityEngine;

namespace Health
{
    public class HealthSystem
    {
        private FloatVariable CurrentHealth;
        private HealthSystemDefinition HealthSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;

        /// <summary>
        /// An event is created when <see cref="CurrentHealth"/> value change to allow multiple process to hook at value. 
        /// </summary>
        private event OnValueChangedDelegate OnHealthValueChangedEvent;

        public HealthSystem(CoreInteractiveObject AssociatedInteractiveObject, HealthSystemDefinition HealthSystemDefinition, OnValueChangedDelegate OnHealthValueChangedAction = null)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.HealthSystemDefinition = HealthSystemDefinition;

            /// By default, the constructor OnValueChangedDelegate is registered to OnValueChangedEvent
            if (OnHealthValueChangedAction != null)
            {
                this.OnHealthValueChangedEvent += OnHealthValueChangedAction;
            }

            this.CurrentHealth = new FloatVariable(this.HealthSystemDefinition.StartHealth, this.OnHealthValueChanged);
        }

        public void RegisterOnHealthValueChangedEventListener(OnValueChangedDelegate OnValueChangedDelegate)
        {
            this.OnHealthValueChangedEvent += OnValueChangedDelegate;
            /// Initialize the added event by manually calling it
            OnValueChangedDelegate.Invoke(this.CurrentHealth.GetValue(), this.CurrentHealth.GetValue());
        }

        private void OnHealthValueChanged(float OldValue, float newValue)
        {
            this.OnHealthValueChangedEvent?.Invoke(OldValue, newValue);
        }

        public void ChangeHealth(float HealthDelta)
        {
            this.CurrentHealth.SetValue(Mathf.Min(this.CurrentHealth.GetValue() + HealthDelta, this.GetMaxHealth()));
            if (this.HealthSystemDefinition.HealthRecoveryParticleEffect != null && HealthDelta > 0)
            {
                this.HealthSystemDefinition.HealthRecoveryParticleEffect.BuildParticleObject(string.Empty, this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform, Vector3.zero);
            }
        }

        #region DataRetrieval

        public float GetMaxHealth()
        {
            return this.HealthSystemDefinition.StartHealth;
        }

        public float GetHealthInPercent01()
        {
            return this.CurrentHealth.GetValue() / this.HealthSystemDefinition.StartHealth;
        }

        #endregion
    }
}