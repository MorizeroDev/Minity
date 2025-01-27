using System;
#if UNITY_EDITOR
using Minity.Variable.Editor;
#endif
using UnityEngine;

namespace Minity.Variable
{
    public delegate void VariableChangedEvent<in T>(T lastValue, T newValue);
    
    public abstract class MinityVariableBase : ScriptableObject
    {
        public bool ChangedSinceLastUpdate { get; internal set; }
        public bool ChangedSinceLastFixedUpdate { get; internal set; }
        
        protected object _value;

        internal event Action InternalChangedEvent;

        internal object GetValue()
        {
            return _value;
        }

        internal abstract void SetValue(object value);
        internal abstract void SetInspectorValue(object value);
        internal abstract void RegisterOnChangedListener(object listener);
        internal abstract void ResetToInitial();

        internal void RaiseInternalChangedEvent()
        {
            InternalChangedEvent?.Invoke();
        }
    }
    
    public abstract class MinityVariable<T> : MinityVariableBase
    {
        public T Value
        {
            get => (T)_value;
            set
            {
                OnChanged?.Invoke((T)_value, value);
                RaiseInternalChangedEvent();
                
                _value = value;
                
                muteValidate = true;
                InspectorValue = value;
                muteValidate = false;
                
                ChangedSinceLastUpdate = true;
                ChangedSinceLastFixedUpdate = true;
                
                MinityVariableGuard.ChangedSinceLastUpdate.Push(this);
                MinityVariableGuard.ChangedSinceLastFixedUpdate.Push(this);
            }
        }

        [SerializeField]
        internal T InspectorValue;

        private T initialValue;
        
        public event VariableChangedEvent<T> OnChanged;
        
        private bool muteValidate = false;

        internal override void SetInspectorValue(object value)
        {
            muteValidate = true;
            InspectorValue = (T)value;
            muteValidate = false;
        }

        internal override void RegisterOnChangedListener(object listener)
        {
            OnChanged += (VariableChangedEvent<T>)listener;
        }

        internal override void SetValue(object value)
        {
            Value = (T)value;
        }

        private void OnEnable()
        {
            _value = InspectorValue;
            initialValue = InspectorValue;
#if UNITY_EDITOR
            MinityVariableReset.Variables.Push(this);
#endif
        }

        internal override void ResetToInitial()
        {
            InspectorValue = initialValue;
        }

        private void OnValidate()
        {
            if (muteValidate)
            {
                return;
            }
            
            Value = InspectorValue;
        }
    }
}
