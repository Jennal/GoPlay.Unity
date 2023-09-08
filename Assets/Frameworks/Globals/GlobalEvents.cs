using GoPlay.Events;
using UnityEngine.Events;

namespace GoPlay.Globals
{    
    
    public static partial class GlobalEvents
    {
        // App
        public static UnityEvent ApplicationQuit = new UnityEvent();    // app 退出
        public static UnityEvent ApplicationFocusIn = new UnityEvent(); // app 得到焦点
        public static UnityEvent ApplicationFocusOut = new UnityEvent();// app 失去焦点
        public static UnityEvent ApplicationPause = new UnityEvent();   // app 暂停 
        public static UnityEvent ApplicationResume = new UnityEvent();  // app 恢复
        
        //UI
        public static UIEvent UIOpened = new UIEvent();
        public static UIEvent UIClosed = new UIEvent();
        public static WindowEvent WindowOpened = new WindowEvent();
        public static WindowEvent WindowClosed = new WindowEvent();
    }
}