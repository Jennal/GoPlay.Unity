using System;

namespace GoPlay.Framework.ValueWatcher
{
    public class WatchClassAttribute : Attribute
    {
        public bool ShowEvent;

        public WatchClassAttribute(bool showEvent = false)
        {
            ShowEvent = showEvent;
        }
    }

    public class WatchFieldAttribute : Attribute
    {
    }
}