using System;

namespace GoPlay.Events
{
    [Serializable]
    public class EventParams
    {
        public static readonly EventParams Empty = new EventParams();
        
        public int IntVal;
        public float FloatVal;
        public string StringVal;
        public bool BoolVal;

        public override string ToString()
        {
            return $"EventParams{{IntVal={IntVal}, FloatVal={FloatVal}, StringVal={StringVal}, BoolVal={BoolVal}}}";
        }
    }
}