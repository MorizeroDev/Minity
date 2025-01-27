using System;
using Minity.Variable;
using UnityEngine;

namespace Minity.UI
{
    [Serializable]
    public class VarBinding<T> : BindingBase, ISerializationCallbackReceiver 
    {
        [SerializeField]
        private MinityVariable<T> Variable;
        
        public T Value
        {
            set => Variable.SetValue(value);
            get => (T)Variable.GetValue();
        }
        
        internal override object GetValue()
        {
            return Variable.GetValue();
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if (!Variable)
            {
                return;
            }
            Variable.InternalChangedEvent += RaiseUpValueChangedEvent;
        }
    }
}
