namespace GoPlay.Framework.UI
{
    public class UIPoolableWindow : UIPoolablePageChild
    {
        protected override void Start()
        {
            UIUtils.InitCanvas(gameObject);
        }
    }
}