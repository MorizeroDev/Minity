#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using XLua;

namespace Minity.XLuaTools.EventSupports
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(LuaBehaviour))]
    public class Physics3DEventSupport : MonoBehaviour, ILuaEventSupport
    {
        private Action<Collision> onCollisionEnter, onCollisionStay, onCollisionExit;
        private Action<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetEvents(LuaTable table)
        {
            onCollisionEnter = table.Get<Action<Collision>>("onCollisionEnter");
            onCollisionStay = table.Get<Action<Collision>>("onCollisionStay");
            onCollisionExit = table.Get<Action<Collision>>("onCollisionExit");
            onTriggerEnter = table.Get<Action<Collider>>("onTriggerEnter");
            onTriggerStay = table.Get<Action<Collider>>("onTriggerStay");
            onTriggerExit = table.Get<Action<Collider>>("onTriggerExit");
        }

        private void CleanEvents()
        {
            onCollisionEnter = onCollisionStay = onCollisionExit = null;
            onTriggerEnter = onTriggerStay = onTriggerExit = null;
        }
        
        void ILuaEventSupport.Initialize(LuaTable table)
        {
            GetEvents(table);
        }
        
        void ILuaEventSupport.Reload(LuaTable table)
        {
            GetEvents(table);
        }

        private void OnCollisionEnter(Collision other)
        {
            onCollisionEnter?.Invoke(other);
        }

        private void OnCollisionStay(Collision other)
        {
            onCollisionStay?.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            onCollisionExit?.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }
        
        private void OnDestroy()
        {
            CleanEvents();
        }
    }
}
#endif