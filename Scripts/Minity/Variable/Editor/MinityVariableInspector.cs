#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minity.Variable.Editor
{
    [InitializeOnLoad]
    public class MinityVariableReset
    {
        public static readonly Stack<MinityVariableBase> Variables = new Stack<MinityVariableBase>();
        
        static MinityVariableReset()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                while (Variables.Count > 0)
                {
                    Variables.Pop().ResetToInitial();
                }
            }
        }
    }
    
    [CustomEditor(typeof(MinityVariableBase), true)]
    public class MinityVariableInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var variable = (MinityVariableBase)target;
            
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InspectorValue"), new GUIContent("Value"));
            
            EditorGUILayout.Separator();
            
            EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.ToggleLeft("Changed since last update", variable.ChangedSinceLastUpdate);
            EditorGUILayout.ToggleLeft("Changed since last fixedUpdate", variable.ChangedSinceLastFixedUpdate);
            GUI.enabled = true;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
