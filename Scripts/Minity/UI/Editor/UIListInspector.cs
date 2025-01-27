#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Minity.UI.Editor
{
    [CustomEditor(typeof(UIList))]
    public class UIListInspector : UnityEditor.Editor
    {
        private string[] modeNames;
        private int[] modeValues;
        private HashSet<UIList.UIConfig> foldoutSet;
        
        private void OnEnable()
        {
            foldoutSet = new HashSet<UIList.UIConfig>();
            modeNames = Enum.GetNames(typeof(UIMode));
            modeValues = Enum.GetValues(typeof(UIMode)).Cast<int>().ToArray();
        }

        public override void OnInspectorGUI()
        {
            var errorHeader = new GUIStyle(EditorStyles.foldoutHeader)
            {
                normal =
                {
                    textColor = Color.red
                },
                active = 
                {
                    textColor = Color.red
                },
                focused = 
                {
                    textColor = Color.red
                }
            };
            
            var set = new HashSet<Type>();
            
            var list = (UIList)target;
            var dirty = false;
            for (var i = 0; i < list.List.Count; i++)
            {
                if (i >= list.List.Count)
                {
                    break;
                }
                
                var ui = list.List[i];
                var duplicated = ui.UI && set.Contains(ui.GetType());
                
                if (ui.UI)
                {
                    set.Add(ui.GetType());
                }
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel = 1;
                    var foldout = foldoutSet.Contains(ui);
                    foldout = EditorGUILayout.Foldout(foldout, ui.UI?.EditorName ?? "<Empty UI>", ui.UI && !duplicated ? EditorStyles.foldoutHeader : errorHeader);
                    if (foldout != foldoutSet.Contains(ui))
                    {
                        if (foldout)
                        {
                            foldoutSet.Add(ui);
                        }
                        else
                        {
                            foldoutSet.Remove(ui);
                        }
                    }
                    if (EditorGUILayout.LinkButton(EditorGUIUtility.IconContent("Toolbar Minus")))
                    {
                        list.List.RemoveAt(i);
                        i--;
                        dirty = true;
                    }
                EditorGUI.indentLevel = 0;
                EditorGUILayout.EndHorizontal();

                if (duplicated)
                {
                    EditorGUILayout.HelpBox(" Duplicated UI", MessageType.Error);
                }
                
                if (!foldout)
                {
                    EditorGUILayout.EndVertical();
                    continue;
                }
                
                EditorGUI.indentLevel = 1;
                    var newUI = EditorGUILayout.ObjectField("Prefab Asset", ui.UI, typeof(ManagedUI), false);
                    if (newUI != ui.UI)
                    {
                        list.List[i].UI = (ManagedUI)newUI;
                        dirty = true;
                    }

                    var newMode = (UIMode)EditorGUILayout.IntPopup("Mode", (int)ui.Mode, modeNames, modeValues);
                    if (newMode != ui.Mode)
                    {
                        list.List[i].Mode = newMode;
                        dirty = true;
                    }
                    
                    EditorGUILayout.Space(5f);
                EditorGUI.indentLevel = 0;
                
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);
            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus")))
            {
                var item = new UIList.UIConfig();
                list.List.Add(item);
                foldoutSet.Add(item);
                dirty = true;
            }

            if (dirty)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif
