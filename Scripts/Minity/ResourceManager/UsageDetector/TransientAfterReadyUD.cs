using System;
using UnityEngine;

namespace Minity.ResourceManager.UsageDetector
{
    public struct TransientAfterReadyUD : IUsageDetector
    {
        private int _frameIdx, _frameCnt;
        
        public int GetRemainingFrames() => _frameCnt - (Time.frameCount - _frameIdx);
        
        public void Initialize(object? bind)
        {
            if (bind is not int frameCnt)
            {
                throw new Exception("Must specify how much frame count is valid after the resource is loaded.");
            }
            _frameCnt = frameCnt;
            _frameIdx = Time.frameCount;
        }

        public bool IsUsing()
        {
            return Time.frameCount - _frameIdx < _frameCnt;
        }
        
        public IUsageDetector CombineDetector(IUsageDetector detector)
        {
            var compose = new ComposeUD();
            compose.CombineDetector(this);
            return compose;
        }
    }
}
