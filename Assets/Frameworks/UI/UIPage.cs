using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace GoPlay.Framework.UI
{
    public class UIPage : UIPanel
    {
        public UIPageChild[] Children => GetComponentsInChildren<UIPageChild>(true);

        public IEnumerable<UIWindow> ChildrenWindows => Children.Where(o => o is UIWindow).Cast<UIWindow>();
        public IEnumerable<UIWidget> ChildrenWidgets => Children.Where(o => o is UIWidget).Cast<UIWidget>();

        public override void Open(params object[] data)
        {
            UIManager.Instance.PageStack.Add(this);
            base.Open(data);
        }

        public override void Close()
        {
            Assert.IsTrue(UIManager.Instance.PageStack.Count > 1);
            
            foreach (var child in Children)
            {
                if(child) child.Close();
            }
            
            base.Close();
            UIManager.Instance.PageStack.Remove(this);
        }
    }
}