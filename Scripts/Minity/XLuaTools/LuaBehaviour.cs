#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Collections.Generic;
using Minity.XLuaTools.EventSupports;
using UnityEngine;
using XLua;
using Object = UnityEngine.Object;

namespace Minity.XLuaTools
{
    public class LuaBehaviour : MonoBehaviour
    {
        [Serializable]
        public class Injection
        {
            public string Name;
            public string Type = "GameObject";
            public GameObject Object;
        }

        [Flags]
        public enum EventCullFlag
        {
            None = 0,
            Basic = 1 << 0, Updating = 1 << 1, Enabling = 1 << 2, 
            Physics2D = 1 << 3, Physics3D = 1 << 4
        }

        private static readonly Dictionary<EventCullFlag, Type> EventSupportMap = new()
        {
            [EventCullFlag.Basic] = typeof(BasicEventSupport),
            [EventCullFlag.Updating] = typeof(UpdatingEventSupport),
            [EventCullFlag.Enabling] = typeof(EnablingEventSupport),
            [EventCullFlag.Physics2D] = typeof(Physics2DEventSupport),
            [EventCullFlag.Physics3D] = typeof(Physics3DEventSupport),
        };
        
        public LuaAsset Code;
        
        public EventCullFlag EventCulling = EventCullFlag.Basic | EventCullFlag.Updating;
        public Injection[] Injections;
        
        internal LuaTable ScriptScopeTable;

        private ILuaEventSupport[] registeredEvents;
        
        private void Awake()
        {
            _ = LuaEnvGuard.Instance;
            
            ScriptScopeTable = LuaEnvGuard.Environment.NewTable();
            
            using var meta = LuaEnvGuard.Environment.NewTable();
            meta.Set("__index", LuaEnvGuard.Environment.Global);
            ScriptScopeTable.SetMetaTable(meta);
            
            ScriptScopeTable.Set("self", this);
            ScriptScopeTable.Set("Global", LuaEnvGuard.Environment.Global);

            foreach (var injection in Injections)
            {
                if (injection.Type != "GameObject")
                {
                    ScriptScopeTable.Set(injection.Name, injection.Object.GetComponent(injection.Type));
                }
                else
                {
                    ScriptScopeTable.Set(injection.Name, injection.Object);
                }
            }
            
            LuaEnvGuard.Environment.DoString(Code.Code, Code.name, ScriptScopeTable);
            Code.Validated = false;

            var events = new List<ILuaEventSupport>();
            foreach (var flag in Enum.GetValues(typeof(EventCullFlag)))
            {
                if ((EventCulling & (EventCullFlag)flag) == 0)
                {
                    continue;
                }
                if (EventSupportMap.TryGetValue((EventCullFlag)flag, out var type))
                {
                    var eventSupport = (ILuaEventSupport)gameObject.AddComponent(type);
                    eventSupport.Initialize(ScriptScopeTable);
                    events.Add(eventSupport);
                }
            }

            registeredEvents = events.ToArray();
        }

        private void OnDestroy()
        {
            ScriptScopeTable.Dispose();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Code.Validated)
            {
                return;
            }

            Debug.Log($"Code '{Code.name}' has been updated and reloaded.");
            
            LuaEnvGuard.Environment.DoString(Code.Code, Code.name, ScriptScopeTable);
            foreach (var ev in registeredEvents)
            {
                ev.Reload(ScriptScopeTable);
            }
            
            Code.Validated = false;
        }
#endif
    }
}
#endif
