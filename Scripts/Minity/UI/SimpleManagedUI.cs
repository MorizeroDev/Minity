using System;

namespace Minity.UI
{
    public class SimpleManagedUI : ManagedUI
    {
        internal override string EditorName => $"void {name}();";

        protected override void Begin()
        {
            
        }

        protected override void AboutToClose()
        {
            
        }

        internal override void Open(object parameter)
        {
            
        }
        
        public void Close()
        {
            CloseInternalCallback = null;
            CloseInternal();
        }
    }
}
