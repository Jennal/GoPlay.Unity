using UnityEngine;
using GoPlay.Globals;
using GoPlay.Utils;

namespace GoPlay.Framework.UI
{
    public class UIWindow : UIPageChild
    {
        protected override void Start()
        {
            UIUtils.InitCanvas(gameObject);
        }
        
        public override void Open(params object[] data)
        {
            transform.GetOrAddComponent<Canvas>().sortingOrder = transform.GetSiblingIndex() + 2;
            base.Open(data);
            GlobalEvents.WindowOpened.Invoke(this);
        }

        public override void Close()
        {
            base.Close();
            GlobalEvents.WindowClosed.Invoke(this);
        }

        public virtual void OnInputEscape()
        {
            
        }
    }
}