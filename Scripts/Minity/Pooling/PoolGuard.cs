using System;
using Minity.Logger;
using UnityEngine;
using Random = System.Random;

namespace Minity.Pooling
{
    [AddComponentMenu("")]
    internal class PoolGuard : MonoBehaviour
    {
        internal const int usageTrackCount = 10;
        private float tick = 0f;
        
        private void FixedUpdate()
        {
            if (!ObjectPool.AutoReleaseUnusedObjects)
            {
                return;
            }

            tick += Time.fixedDeltaTime;
            if (tick >= 1f)
            {
                tick -= 1f;
                foreach (var context in ObjectPool.contexts.Values)
                {
                    context.UsageRecords.Enqueue(context.CurrentUsage);
                    context.PeriodUsage += context.CurrentUsage;
                    if (context.UsageRecords.Count > usageTrackCount)
                    {
                        context.PeriodUsage -= context.UsageRecords.Dequeue();
                    }

                    if (context.PeriodUsage == 0)
                    {
                        context.IdleTick++;
                    }
                    else
                    {
                        context.IdleTick = 0;
                    }
                }
            }

            foreach (var context in ObjectPool.contexts.Values)
            {
                var cnt = Math.Max(context.CurrentUsage, context.PeriodUsage / usageTrackCount) 
                                                    + context.MinimumObjectCount 
                                                    - Math.Max(context.IdleTick - 10, 0);
                if (context.GetObjectCount() > cnt - context.CurrentUsage)
                {
                    var collection = context.Request();
                    collection.PoolableController.ReadyToDestroy = true;
                    Destroy(collection.GameObject);
                    context.Objects.Remove(collection);
                }
            }
        }
    }
}
