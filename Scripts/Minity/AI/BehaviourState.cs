namespace Minity.AI
{
    public delegate BehaviourState BehaviourFunction<in T>(T context);
    public enum BehaviourState
    {
        Succeed, Failed, Running
    }
}
