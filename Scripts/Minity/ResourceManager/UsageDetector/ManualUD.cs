namespace Minity.ResourceManager.UsageDetector
{
    public class ManualUD : IUsageDetector
    {
        private bool _isUsing;
        
        public void Initialize(object? bind)
        {
            _isUsing = true;
        }

        public bool IsUsing()
        {
            return _isUsing;
        }

        public void Release()
        {
            _isUsing = false;
        }
        
        public IUsageDetector CombineDetector(IUsageDetector detector)
        {
            var compose = new ComposeUD();
            compose.CombineDetector(this);
            return compose;
        }
    }
}
