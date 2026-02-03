namespace Minity.ResourceManager.UsageDetector
{
    public interface IUsageDetector
    {
        public void Initialize(object? bind);
        public bool IsUsing();
        public IUsageDetector CombineDetector(IUsageDetector detector);
    }
}
