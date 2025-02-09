#if MINITY_ENABLE_XLUA_TOOLS
using System;
using UnityEngine;

namespace Minity.XLuaTools
{
    public class LuaAsset : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string _code;
        
        internal bool Validated = true;
        
        public string Code => _code;
        
        public override string ToString() => _code;

        private void OnValidate()
        {
            Validated = true;
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