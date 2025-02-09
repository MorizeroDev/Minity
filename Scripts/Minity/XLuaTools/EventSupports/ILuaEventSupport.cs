#if MINITY_ENABLE_XLUA_TOOLS
using XLua;

namespace Minity.XLuaTools.EventSupports
{
    internal interface ILuaEventSupport
    {
        internal void Initialize(LuaTable table);

        internal void Reload(LuaTable table);
    }
}
#endif