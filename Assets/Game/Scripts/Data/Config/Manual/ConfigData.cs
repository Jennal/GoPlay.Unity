namespace GoPlay.Managers
{
    public partial class ConfigData
    {
        partial void InitUIAssets()
        {
            s_IUIAssets = UIAssetsConfs.ConvertAll(o => o as IUIAssetConf);
        }
    }
}