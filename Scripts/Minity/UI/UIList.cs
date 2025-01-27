using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minity.UI
{
    [CreateAssetMenu(fileName = "UIList", menuName = "Minity/UI List")]
    public class UIList : ScriptableObject
    {
        [Serializable]
        public class UIConfig
        {
            public ManagedUI UI;
            public UIMode Mode;
        }
        public List<UIConfig> List = new();
    }
}
