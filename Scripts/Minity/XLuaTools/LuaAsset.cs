#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minity.XLuaTools
{
    public class LuaAsset : ScriptableObject
    {
        private static readonly Dictionary<string, LuaAsset> pendingSubstitutions = new();
        
        [SerializeField, HideInInspector]
        private string _guid = Guid.NewGuid().ToString();
        
        [SerializeField, HideInInspector]
        private string _code;

        private string _substitution;
        
        internal bool Validated = true;
        
        public string Code => string.IsNullOrEmpty(_substitution) ? _code : _substitution;
        
        public override string ToString() => _code;

        private void OnEnable()
        {
            if (pendingSubstitutions.TryGetValue(_guid, out var substitution))
            {
                _substitution = substitution._code;
                Resources.UnloadAsset(substitution);
                pendingSubstitutions.Remove(_guid);
            }
        }

        private void OnValidate()
        {
            Validated = true;
        }

        public void ApplySubstitution()
        {
            pendingSubstitutions.Add(_guid, this);
        }
        
        public static LuaAsset Create(string code)
        {
            var asset = CreateInstance<LuaAsset>();
            asset._code = code;
            return asset;
        }
    }
}
#endif
