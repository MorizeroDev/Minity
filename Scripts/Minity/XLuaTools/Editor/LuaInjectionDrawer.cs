#if UNITY_EDITOR && MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Minity.XLuaTools.Editor
{
    [CustomPropertyDrawer(typeof(LuaBehaviour.Injection))]
    public class LuaInjectionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                }
            };



            var objField = new PropertyField(property.FindPropertyRelative("Object"), "")
            {
                style =
                {
                    width = new StyleLength(Length.Percent(30f))
                }
            };
            container.Add(objField);
            
            
            var obj = (GameObject)property.FindPropertyRelative("Object").objectReferenceValue;

            if (obj)
            {
                var components = new List<Component>();
                obj.GetComponents(components);
                var types = components.Select(x => x.GetType().Name).Distinct().ToList();
                types.Insert(0, "GameObject");
                var typeProp = property.FindPropertyRelative("Type");
                
                var nameField = new PropertyField(property.FindPropertyRelative("Name"), "")            
                {
                    style =
                    {
                        width = new StyleLength(Length.Percent(30f))
                    }
                };
                var typeField = new DropdownField(types, 
                    Math.Max(0, types.FindIndex(x => x == typeProp.stringValue)))
                {
                    style =
                    {
                        width = new StyleLength(Length.Percent(40f))
                    }
                };
                typeField.BindProperty(typeProp);
            
                container.Add(nameField);
                container.Add(typeField);
            }
            
            return container;
        }
    }
}
#endif