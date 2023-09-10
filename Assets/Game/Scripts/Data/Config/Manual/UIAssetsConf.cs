using GoPlay.Managers;

namespace GoPlay.Data.Config
{
    public partial struct UIAssetsConf : IUIAssetConf
    {
        public string UIClassName => ClassName;
        public string UIAssetbundle => Assetbundle;
        public string UIAsset => Asset;
    }
}