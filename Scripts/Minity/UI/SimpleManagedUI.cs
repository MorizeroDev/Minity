﻿using System;

namespace Minity.Milutools.UI
{
    public class SimpleManagedUI : ManagedUI
    {
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