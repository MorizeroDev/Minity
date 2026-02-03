using System;
using System.Threading.Tasks;
using Minity.ResourceManager.UsageDetector;
using Object = UnityEngine.Object;

namespace Minity.ResourceManager.Handlers
{
    public interface IResHandler
    {
        public void Initialize(Uri uri);

        public Task<Object> LoadAsync<T>() where T : Object;
        
        public Object Load<T>() where T : Object;
        
        public void Release();
    }
}
