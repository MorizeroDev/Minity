#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using XLua;

namespace Minity.XLuaTools.EventSupports
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(LuaBehaviour))]
    public class Physics2DEventSupport : MonoBehaviour, ILuaEventSupport
    {
        private Action<Collision2D> onCollisionEnter2D, onCollisionStay2D, onCollisionExit2D;
        private Action<Collider2D> onTriggerEnter2D, onTriggerStay2D, onTriggerExit2D;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetEvents(LuaTable table)
        {
            onCollisionEnter2D = table.Get<Action<Collision2D>>("onCollisionEnter2D");
            onCollisionStay2D = table.Get<Action<Collision2D>>("onCollisionStay2D");
            onCollisionExit2D = table.Get<Action<Collision2D>>("onCollisionExit2D");
            onTriggerEnter2D = table.Get<Action<Collider2D>>("onTriggerEnter2D");
            onTriggerStay2D = table.Get<Action<Collider2D>>("onTriggerStay2D");
            onTriggerExit2D = table.Get<Action<Collider2D>>("onTriggerExit2D");
        }

        private void CleanEvents()
        {
            onCollisionEnter2D = onCollisionStay2D = onCollisionExit2D = null;
            onTriggerEnter2D = onTriggerStay2D = onTriggerExit2D = null;
        }
        
        void ILuaEventSupport.Initialize(LuaTable table)
        {
            GetEvents(table);
        }

        void ILuaEventSupport.Reload(LuaTable table)
        {
            GetEvents(table);
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            onCollisionEnter2D?.Invoke(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            onCollisionStay2D?.Invoke(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            onCollisionExit2D?.Invoke(other);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            onTriggerEnter2D?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            onTriggerStay2D?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExit2D?.Invoke(other);
        }

        private void OnDestroy()
        {
            CleanEvents();
        }
    }
}
#endif