#if UNITY_2018_1_OR_NEWER && MINITY_ENABLE_XLUA_TOOLS
using UnityEngine;
using UnityEditor;

using System.IO;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Minity.XLuaTools.Editor
{
    [ScriptedImporter(2, new[] { "lua" })]
    public class LuaImporter : ScriptedImporter
    {
        private const string IconGUID = "e6b6558eba4f8d249b8ac24be9d6985e";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var id = AssetDatabase.AssetPathToGUID(ctx.assetPath);
            var asset = LuaAsset.Create(File.ReadAllText(ctx.assetPath), id);
            ctx.AddObjectToAsset("LuaCode", asset, LoadIconTexture());
            ctx.SetMainObject(asset);
        }

        private static Texture2D LoadIconTexture()
        {
            return AssetDatabase.LoadAssetAtPath(
                        AssetDatabase.GUIDToAssetPath(IconGUID), 
                        typeof(Texture2D)
                    ) as Texture2D;
        }
    }
}

#endif
