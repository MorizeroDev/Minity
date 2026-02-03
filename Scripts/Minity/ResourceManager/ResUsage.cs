using Minity.ResourceManager.UsageDetector;
using UnityEngine;

namespace Minity.ResourceManager
{
    public static class ResUsage
    {
        /// <summary>
        /// Temporary use: release immediately after x frames once loaded
        /// </summary>
        public static IUsageDetector Temp(int validFrameCnt = 0)
        {
            var ret = new TransientAfterReadyUD();
            ret.Initialize(validFrameCnt);
            return ret;
        }
        
        /// <summary>
        /// Release after the current scene ends
        /// </summary>
        public static IUsageDetector Scene()
        {
            var ret = new SceneUD();
            ret.Initialize(null);
            return ret;
        }
        
        /// <summary>
        /// Release after the specified GameObject is destroyed
        /// </summary>
        public static IUsageDetector AsResUser(this GameObject gameObject)
        {
            var ret = new ObjectLinkUD();
            ret.Initialize(gameObject);
            return ret;
        }
        
        /// <summary>
        /// Release after the specified Component is destroyed
        /// </summary>
        public static IUsageDetector AsResUser(this Component component)
        {
            var ret = new ObjectLinkUD();
            ret.Initialize(component);
            return ret;
        }
        
        /// <summary>
        /// Bound to another Object, destroyed together with it
        /// </summary>
        public static IUsageDetector AsResUser(this Object obj)
        {
            var ret = new ObjectLinkUD();
            ret.Initialize(obj);
            return ret;
        }
        
        /// <summary>
        /// Manually release
        /// </summary>
        public static ManualUD Manual()
        {
            var ret = new ManualUD();
            ret.Initialize(null);
            return ret;
        }
    }
}
