using System;
using Object = UnityEngine.Object;

namespace Minity.ResourceManager.UsageDetector
{
    public struct ObjectLinkUD : IUsageDetector
    {
        private WeakReference<Object> _refer;
        
        public bool TryGetLinkObject(out Object obj) => _refer.TryGetTarget(out obj);
        
        public void Initialize(object? bind)
        {
            if (bind is not Object obj)
            {
                throw new Exception("Must bind a object");
            }
            _refer = new WeakReference<Object>(obj);
        }

        public bool IsUsing()
        {
            return _refer.TryGetTarget(out var obj) && obj;
        }
        
        public IUsageDetector CombineDetector(IUsageDetector detector)
        {
            var compose = new ComposeUD();
            compose.CombineDetector(this);
            return compose;
        }
    }
}
