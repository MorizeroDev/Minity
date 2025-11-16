#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace Minity.Event.Editor
{
    public class EventArgsPublishWindow : EditorWindow
    {
        private Type selectedType;
        private IEventArgs argsInstance;
        private Vector2 scroll;
        private List<Type> registeredTypes = new();

        public static void Open()
        {
            GetWindow<EventArgsPublishWindow>("Publish Event").Show();
        }

        private void OnEnable()
        {
            RefreshRegisteredTypes();
        }

        private void RefreshRegisteredTypes()
        {
            registeredTypes.Clear();
            foreach (var t in EventBus.Instance.GetRegisteredTypes())
                registeredTypes.Add(t);
        }

        private void OnGUI()
        {
            // Top: show total number and provide a refresh button
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField($"Registered events: {registeredTypes.Count}", GUILayout.Width(160));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                RefreshRegisteredTypes();
                selectedType = null;
                argsInstance = null;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Left: event type list (scrollable)
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(220));
            EditorGUILayout.LabelField("Event Types", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(220),
                GUILayout.Height(position.height - 70));
            foreach (var type in registeredTypes)
            {
                if (GUILayout.Button(type.Name))
                {
                    selectedType = type;
                    try
                    {
                        argsInstance = (IEventArgs)Activator.CreateInstance(type);
                    }
                    catch
                    {
                        argsInstance = (IEventArgs)FormatterServices.GetUninitializedObject(type);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // Right: parameter editor and publish controls
            EditorGUILayout.BeginVertical();
            if (argsInstance != null && selectedType != null)
            {
                EditorGUILayout.LabelField($"Edit parameters: {selectedType.Name}", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                var fields =
                    selectedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var f in fields)
                {
                    // Only show serializable fields: public or marked with SerializeField
                    if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) continue;

                    object value = f.GetValue(argsInstance);

                    // Primitive type support
                    if (f.FieldType == typeof(int))
                    {
                        f.SetValue(argsInstance,
                            EditorGUILayout.IntField(ObjectNames.NicifyVariableName(f.Name), (int)(value ?? 0)));
                    }
                    else if (f.FieldType == typeof(float))
                    {
                        f.SetValue(argsInstance,
                            EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(f.Name), (float)(value ?? 0f)));
                    }
                    else if (f.FieldType == typeof(string))
                    {
                        f.SetValue(argsInstance,
                            EditorGUILayout.TextField(ObjectNames.NicifyVariableName(f.Name), (string)(value ?? "")));
                    }
                    else if (f.FieldType == typeof(bool))
                    {
                        f.SetValue(argsInstance,
                            EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(f.Name), (bool)(value ?? false)));
                    }
                    // Support dragging for UnityEngine.Object and derived types (including GameObject, Component)
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType))
                    {
                        // allowSceneObjects = true so scene GameObjects/Components can be dragged in
                        var obj = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(f.Name),
                            (UnityEngine.Object)value, f.FieldType, true);
                        f.SetValue(argsInstance, obj);
                    }
                    // Enum supports dropdown selection
                    else if (f.FieldType.IsEnum)
                    {
                        var enumVal = (Enum)(value ?? Enum.GetValues(f.FieldType).GetValue(0));
                        var newEnumVal = EditorGUILayout.EnumPopup(ObjectNames.NicifyVariableName(f.Name), enumVal);
                        f.SetValue(argsInstance, newEnumVal);
                    }
                    // Simple arrays (1D primitive types only) are shown as read-only strings (can be extended)
                    else if (f.FieldType.IsArray)
                    {
                        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(f.Name),
                            "(array) " + (value == null ? "null" : value.ToString()));
                    }
                    else
                    {
                        // Other unsupported types are displayed as read-only text to avoid exceptions
                        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(f.Name),
                            (value != null) ? value.ToString() : "null");
                    }
                }

                EditorGUILayout.Space();

                // Show runtime subscribers for the selected event type
                try
                {
                    var subscribers = EventBus.Instance.GetSubscribers(selectedType).ToList();
                    EditorGUILayout.LabelField($"Subscribers: {subscribers.Count}", EditorStyles.boldLabel);
                    foreach (var s in subscribers)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{s.DeclaringType}.{s.MethodName}", GUILayout.ExpandWidth(true));
                        if (s.Target is UnityEngine.Object uobj)
                        {
                            EditorGUILayout.ObjectField(uobj, typeof(UnityEngine.Object), true, GUILayout.Width(200));
                            if (GUILayout.Button("Select", GUILayout.Width(60)))
                            {
                                Selection.activeObject = uobj;
                                EditorGUIUtility.PingObject(uobj);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField(s.Target != null ? s.Target.ToString() : "<static>", GUILayout.Width(200));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                catch (Exception)
                {
                    // ignore subscriber introspection errors in editor window
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Publish", GUILayout.Height(28)))
                {
                    try
                    {
                        var method = typeof(EventBus).GetMethod(nameof(EventBus.Publish))
                            .MakeGenericMethod(selectedType);
                        method.Invoke(EventBus.Instance, new object[] { argsInstance });
                        Debug.Log($"[Tools] Published {selectedType.Name}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                if (GUILayout.Button("Recreate Instance", GUILayout.Height(28)))
                {
                    try
                    {
                        argsInstance = (IEventArgs)Activator.CreateInstance(selectedType);
                    }
                    catch
                    {
                        argsInstance = (IEventArgs)FormatterServices.GetUninitializedObject(selectedType);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Select an event type to edit its arguments.",
                    EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
