#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minity.XLuaTools
{
    public class LuaAsset : ScriptableObject
    {
        private static readonly Dictionary<string, LuaAsset> substitutions = new();
        private static readonly Dictionary<string, List<LuaAsset>> activeLuaAssets = new();
        
        [SerializeField]
        private string Guid;
        
        [SerializeField, HideInInspector]
        private string _code;

        private string _substitution;
        
        private bool _isSubstitution = false;

        internal event Action ReloadEvent;
        
        public string Code => string.IsNullOrEmpty(_substitution) ? _code : _substitution;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                return;
            }
            
            if (substitutions.TryGetValue(Guid, out var substitution))
            {
                _substitution = substitution._code;
            }

            if (!activeLuaAssets.ContainsKey(Guid))
            {
                activeLuaAssets.Add(Guid, new List<LuaAsset>());
            }
            activeLuaAssets[Guid].Add(this);
        }

        private void OnDestroy()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                return;
            }
            
            if (activeLuaAssets.TryGetValue(Guid, out var assetList))
            {
                assetList.Remove(this);
            }
        }

        private void OnValidate()
        {
            ReloadEvent?.Invoke();
        }

        public void ApplySubstitution()
        {
            _isSubstitution = true;
            substitutions.Add(Guid, this);
            if (activeLuaAssets.TryGetValue(Guid, out var assetList))
            {
                foreach (var asset in assetList)
                {
                    if (asset._isSubstitution)
                    {
                        continue;
                    }
                    
                    asset._substitution = _code;
                    asset.ReloadEvent?.Invoke();
                }
            }
        }
        
        public static LuaAsset Create(string code, string guid)
        {
            var asset = CreateInstance<LuaAsset>();
            asset._code = code;
            asset.Guid = guid;
            return asset;
        }
    }
}
#endif
