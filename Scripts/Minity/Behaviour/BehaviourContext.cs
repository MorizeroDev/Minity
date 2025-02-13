using UnityEngine;

namespace Minity.Behaviour
{
    public abstract class BehaviourContext : MonoBehaviour
    {
        public BehaviourTree Tree { get; internal set; }

        public abstract void UpdateContext();
    }
}
