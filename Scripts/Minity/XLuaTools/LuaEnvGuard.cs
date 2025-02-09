#if MINITY_ENABLE_XLUA_TOOLS
using System;
using Minity.Infra;
using UnityEngine;
using XLua;

namespace Minity.XLuaTools
{
    public class LuaEnvGuard : GlobalSingleton<LuaEnvGuard>
    {
        public static readonly LuaEnv Environment = new();

        private const float GC_INTERVAL = 1f;

        private float lastGCTime;
        
        private void Update()
        {
            if (Time.unscaledTime - lastGCTime >= GC_INTERVAL)
            {
                lastGCTime = Time.unscaledTime;
                Environment.Tick();
            }
        }
    }
}
#endif