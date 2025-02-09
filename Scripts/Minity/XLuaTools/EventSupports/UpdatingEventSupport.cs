#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using XLua;

namespace Minity.XLuaTools.EventSupports
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(LuaBehaviour))]
    public class UpdatingEventSupport : MonoBehaviour, ILuaEventSupport
    {
        private Action fixedUpdate, update, lateUpdate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetEvents(LuaTable table)
        {
            fixedUpdate = table.Get<Action>("fixedUpdate");
            update = table.Get<Action>("update");
            lateUpdate = table.Get<Action>("lateUpdate");
        }

        private void CleanEvents()
        {
            fixedUpdate = update = lateUpdate = null;
        }
        
        void ILuaEventSupport.Initialize(LuaTable table)
        {
            GetEvents(table);
        }
        
        void ILuaEventSupport.Reload(LuaTable table)
        {
            GetEvents(table);
        }

        private void FixedUpdate()
        {
            fixedUpdate?.Invoke();
        }

        private void Update()
        {
            update?.Invoke();
        }

        private void LateUpdate()
        {
            lateUpdate?.Invoke();
        }
        
        private void OnDestroy()
        {
            CleanEvents();
        }
    }
}
#endif