using System;

namespace GoPlay.AssetManagement
{
    [Serializable]
    public class AssetBundleRefCount
    {
        public string Value;
        public int Count = 1;
        
        public void Retain()
        {
            Count++;
        }

        public void Release(Action destroy)
        {
            Count--;
            if (Count <= 0) destroy();
        }
    }
}