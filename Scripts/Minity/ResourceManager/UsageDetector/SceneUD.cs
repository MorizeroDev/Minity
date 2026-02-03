using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minity.ResourceManager.UsageDetector
{
    public struct SceneUD : IUsageDetector
    {
        private static uint curSceneChangeTick;
        private static bool initialized = false;

        private uint bindSceneChangeTick;

        public uint GetSceneDetectIdx() => bindSceneChangeTick;
        
        public void Initialize(object? bind)
        {
            if (!initialized)
            {
                initialized = true;
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                Application.quitting += UnRegisterSceneEvent;
            }

            bindSceneChangeTick = curSceneChangeTick;
        }

        public bool IsUsing()
        {
            return curSceneChangeTick == bindSceneChangeTick;
        }

        private static void UnRegisterSceneEvent()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            Application.quitting -= UnRegisterSceneEvent;
        }
        
        private static void OnSceneUnloaded(Scene scene)
        {
            curSceneChangeTick++;
        }
        
        public IUsageDetector CombineDetector(IUsageDetector detector)
        {
            var compose = new ComposeUD();
            compose.CombineDetector(this);
            return compose;
        }
    }
}
