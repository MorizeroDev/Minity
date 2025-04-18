﻿using System;
using System.Collections.Generic;

namespace Minity.Behaviour.Nodes
{
    public interface IBehaviourNode
    {
        internal BehaviourState State { get; set; }
        internal IBehaviourNode Previous { get; set; }
        internal BehaviourState Run(BehaviourContext context);
        internal BehaviourState Resume(BehaviourContext context, BehaviourState innerState);
        internal void Reset();
    }
}
