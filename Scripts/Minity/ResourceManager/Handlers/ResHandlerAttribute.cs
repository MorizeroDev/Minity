using System;

namespace Minity.ResourceManager.Handlers
{
    public class ResHandlerAttribute : Attribute
    {
        public string Scheme;

        public ResHandlerAttribute(string scheme)
        {
            Scheme = scheme;
        }
    }
}
