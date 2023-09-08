using System;
using System.Collections.Generic;
using GoPlay.Events;
using UnityEngine;

namespace GoPlay.Affectables
{
    /*
     * AffectableInt 做了操作符重载，计算时可以当做普通int来使用
     * ！！注意：若需要保留Buff状态的赋值，请使用Set函数修改原始值
     */
    [Serializable]
    public class AffectableInt : AffectableValue<int>
    {
        public AffectableInt() : base()
        {
            OnValueChange = new IntEvent();
        }

        public AffectableInt(int v) : base(v)
        {
            OnValueChange = new IntEvent();
        }

        public override AffectableValue Clone()
        {
            return new AffectableInt
            {
                OriginalValue = OriginalValue,
                CurrentValue = CurrentValue,
                affectFuncs = new List<Tuple<string, Func<int, int, int>>>(affectFuncs)
            };
        }
        
        //public static implicit operator AffectableInt(int i)
        //{
        //    return new AffectableInt(i);
        //}

        public static implicit operator int(AffectableInt i)
        {
            if (ReferenceEquals(i, null)) return 0;
            return i.Get();
		}

		public static int operator +(AffectableInt a, AffectableInt b)
        {
            return a.Get() + b.Get();
        }

        public static int operator -(AffectableInt a, AffectableInt b)
        {
            return a.Get() - b.Get();
        }

        public static int operator *(AffectableInt a, AffectableInt b)
        {
            return a.Get() * b.Get();
        }

        public static int operator /(AffectableInt a, AffectableInt b)
        {
            return a.Get() / b.Get();
        }

        //OverLoading == operator
        public static bool operator ==(AffectableInt x, AffectableInt y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Get() == y.Get();
        }

        //OverLoading != operator
        public static bool operator !=(AffectableInt x, AffectableInt y)
        {
            return !(x == y); //This will call to operator == simple way to implement !=
        }

        //Always override GetHashCode(),Equals when overloading ==
        public override bool Equals(object obj)
        {
            return this == (AffectableInt)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //OverLoading < operator
        public static bool operator <(AffectableInt x, AffectableInt y)
        {
            return x.Get() < y.Get();
        }

        //OverLoading > operator
        public static bool operator >(AffectableInt x, AffectableInt y)
        {
            return !(x < y) && x != y; //Calls to operator < and !=
        }

        public static bool operator >=(AffectableInt x, AffectableInt y)
        {
            return (x > y) || (x == y); //Calls to operator > and ==
        }

        public static bool operator <=(AffectableInt x, AffectableInt y)
        {
            return (x < y) || (x == y); //Calls to operator < and ==
        }
    }
}