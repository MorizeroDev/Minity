using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TMPro;

namespace Minity.UI
{
    public delegate string FormatterDelegate<in T>(T data);
    public delegate void OnBindingChangedDelegate(BindingBase sender);
    
    public class Binding<T> : BindingBase
    {
        public FormatterDelegate<T> Formatter;
        
        private T data = default;
        public T Value
        {
            set
            {
                data = value;
                RaiseUpValueChangedEvent();
            }
            get => data;
        }

        [Obsolete("Empty binding does not need to be constructed by yourself.")]
        public Binding()
        {
            
        }

        public Binding(T initial)
        {
            data = initial;
        }
        
        public Binding(FormatterDelegate<T> formatter)
        {
            Formatter = formatter;
        }
        
        public Binding(T initial, FormatterDelegate<T> formatter)
        {
            data = initial;
            Formatter = formatter;
        }
        
        internal override object GetValue()
        {
            if (Formatter != null)
            {
                return Formatter(data);
            }
            return data;
        }
    }

    public abstract class BindingBase
    {
        internal readonly HashSet<TMP_Text> Components = new HashSet<TMP_Text>();
        internal event OnBindingChangedDelegate OnValueChanged;
        internal abstract object GetValue();

        internal void RaiseUpValueChangedEvent()
        {
            OnValueChanged?.Invoke(this);
        }

        public void Do(Action<TMP_Text> action)
        {
            foreach (var component in Components)
            {
                action.Invoke(component);
            }
        }
    }
}
