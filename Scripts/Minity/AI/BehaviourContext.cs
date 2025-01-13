using UnityEngine;

namespace Minity.AI
{
    public abstract class BehaviourContext : MonoBehaviour
    {
        public BehaviourTree Tree { get; internal set; }

        public abstract void UpdateContext();
    }
}
