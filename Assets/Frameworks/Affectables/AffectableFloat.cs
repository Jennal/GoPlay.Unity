using System;
using System.Collections.Generic;
using GoPlay.Events;

namespace GoPlay.Affectables
{
    /*
     * AffectableFloat 做了操作符重载，计算时可以当做普通float来使用
     * ！！注意：若需要保留Buff状态的赋值，请使用Set函数修改原始值
     */
    [Serializable]
    public class AffectableFloat : AffectableValue<float>
    {
        public AffectableFloat() : base()
        {
            OnValueChange = new FloatEvent();
        }

        public AffectableFloat(float v) : base(v)
        {
            OnValueChange = new FloatEvent();
        }

        public override AffectableValue Clone()
        {
            return new AffectableFloat
            {
                OriginalValue = OriginalValue,
                CurrentValue = CurrentValue,
                affectFuncs = new List<Tuple<string, Func<float, float, float>>>(affectFuncs)
            };
        }
        
        //public static implicit operator AffectableFloat(float i)
        //{
        //    return new AffectableFloat(i);
        //}

        public static implicit operator float(AffectableFloat i)
        {
            if (ReferenceEquals(i, null)) return 0f;
            return i.Get();
        }

        public static float operator +(AffectableFloat a, AffectableFloat b)
        {
            return a.Get() + b.Get();
        }

        public static float operator -(AffectableFloat a, AffectableFloat b)
        {
            return a.Get() - b.Get();
        }

        public static float operator *(AffectableFloat a, AffectableFloat b)
        {
            return a.Get() * b.Get();
        }

        public static float operator /(AffectableFloat a, AffectableFloat b)
        {
            return a.Get() / b.Get();
        }

        //OverLoading == operator
        public static bool operator ==(AffectableFloat x, AffectableFloat y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return Math.Abs(x.Get() - y.Get()) < float.Epsilon;
        }

        //OverLoading != operator
        public static bool operator !=(AffectableFloat x, AffectableFloat y)
        {
            return !(x == y); //This will call to operator == simple way to implement !=
        }

        //Always override GetHashCode(),Equals when overloading ==
        public override bool Equals(object obj)
        {
            return this == (AffectableFloat)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //OverLoading < operator
        public static bool operator <(AffectableFloat x, AffectableFloat y)
        {
            return x.Get() < y.Get();
        }

        //OverLoading > operator
        public static bool operator >(AffectableFloat x, AffectableFloat y)
        {
            return !(x < y) && x != y; //Calls to operator < and !=
        }

        public static bool operator >=(AffectableFloat x, AffectableFloat y)
        {
            return (x > y) || (x == y); //Calls to operator > and ==
        }

        public static bool operator <=(AffectableFloat x, AffectableFloat y)
        {
            return (x < y) || (x == y); //Calls to operator < and ==
        }
    }
}