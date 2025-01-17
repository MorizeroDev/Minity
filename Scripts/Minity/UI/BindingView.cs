using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Minity.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class BindingView : MonoBehaviour
    {
        protected abstract CultureInfo Formatter { get; }
        
        private static readonly Regex regex = new (@"{{(.*?)}}", RegexOptions.Compiled);
        
        private class TextSegment
        {
            public string Content;
            public string Formatter;
            public BindingBase Binding;
            public bool IsDynamicText;
        }
        
        private class Link
        {
            public TMP_Text Component;
            public CultureInfo Formatter;
            public List<TextSegment> Segments = new();

            public void UpdateText(BindingBase sender)
            {
                if (!Component)
                {
                    if (sender != null)
                    {
                        sender.Components.Remove(Component);
                        sender.OnValueChanged -= UpdateText;
                    }
                    return;
                }
                var sb = new StringBuilder();
                foreach (var segment in Segments)
                {
                    if (segment.IsDynamicText)
                    {
                        var value = segment.Binding.GetValue();
                        if (value is IFormattable formattable)
                        {
                            sb.Append(formattable.ToString(segment.Formatter, Formatter));
                        }
                        else
                        {
                            sb.Append(value);
                        }
                    }
                    else
                    {
                        sb.Append(segment.Content);
                    }
                }

                Component.text = sb.ToString();
            }
        }
        
        private Dictionary<string, BindingBase> bindings = new();

        private BindingBase GetBinding(string path)
        {
            if (bindings.TryGetValue(path, out var binding))
            {
                return binding;
            }
            
            var items = path.Split('.');
            var type = this.GetType();
            object current = this;
            object parent = this;
            FieldInfo field = null;
            
            foreach (var item in items)
            {
                field = type.GetField(item, 
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                {
                    Debug.LogWarning($"Could not find the binding '{path}'");
                    return null;
                }

                parent = current;
                current = field.GetValue(current);
                
                type = field.FieldType;
            }

            if (type.BaseType != typeof(BindingBase))
            {
                Debug.LogWarning($"the type of binding '{path}' must be Binding<T>");
                return null;
            }

            BindingBase ret;
            if (current == null)
            {
                ret = (BindingBase)Activator.CreateInstance(type);
                field.SetValue(parent, ret);
            }
            else
            {
                ret = current as BindingBase;
            }
            
            bindings.Add(path, ret);

            return ret;
        }

        public void CreateLink(TMP_Text component)
        {
            var text = component.text;
            var link = new Link()
            {
                Component = component,
                Formatter = Formatter
            };
            var lastIndex = 0;
            var matches = regex.Matches(text);
            
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    if (match.Index > lastIndex)
                    {
                        link.Segments.Add(new TextSegment()
                        {
                            Content = text.Substring(lastIndex, match.Index - lastIndex)
                        });
                    }

                    var content = match.Groups[1].Value.Trim();
                    var index = content.IndexOf(':');
                    string formatter = null;

                    if (index != -1)
                    {
                        formatter = content.Substring(index + 1);
                        content = content.Substring(0, index);
                    }
                    
                    var binding = GetBinding(content);
                    if (binding == null)
                    {
                        link.Segments.Add(new TextSegment());
                    }
                    else
                    {
                        link.Segments.Add(new TextSegment()
                        {
                            Binding = binding,
                            Formatter = formatter,
                            IsDynamicText = true
                        });
                        binding.OnValueChanged += link.UpdateText;
                        binding.Components.Add(component);
                    }
                    
                    lastIndex = match.Index + match.Length;
                }
            }
                
            if (lastIndex < text.Length)
            {
                link.Segments.Add(new TextSegment()
                {
                    Content = text.Substring(lastIndex)
                });
            }
            
            link.UpdateText(null);
        }
        
        public void Awake()
        {
            var components = GetComponentsInChildren<TMP_Text>(true);
            foreach (var component in components)
            {
                CreateLink(component);
            }

            Initialize();
        }

        protected virtual void Initialize()
        {
            
        }
    }
}
