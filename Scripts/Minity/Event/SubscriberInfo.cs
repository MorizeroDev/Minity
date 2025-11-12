namespace Minity.Event
{
    /// <summary>
    /// Lightweight DTO describing a subscriber (target instance, method and declaring type).
    /// Useful for runtime tracing and debugging.
    /// </summary>
    public class SubscriberInfo
    {
        public object Target { get; }
        public string MethodName { get; }
        public string DeclaringType { get; }

        public SubscriberInfo(object target, string methodName, string declaringType)
        {
            Target = target;
            MethodName = methodName;
            DeclaringType = declaringType;
        }

        public override string ToString() => $"{DeclaringType}.{MethodName} (target: {Target})";
    }
}
