using System;
using Minity.General;
using UnityEngine;

namespace Minity.SceneRouter
{
    public class SceneRouterNode
    {
        internal EnumIdentifier Identifier;
        internal string[] Path;
        internal string FullPath;
        internal string Scene;
        internal bool IsRoot;
        
        internal SceneRouterNode()
        {
            
        }
    }
}
