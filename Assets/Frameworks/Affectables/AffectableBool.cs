using System;
using System.Collections.Generic;
using GoPlay.Events;
using UnityEngine;

namespace GoPlay.Affectables
{
    /*
     * AffectableBool 做了操作符重载，计算时可以当做普通bool来使用
     * ！！注意：若需要保留Buff状态的赋值，请使用Set函数修改原始值
     */
    [Serializable]
    public class AffectableBool : AffectableValue<bool>
    {
        public AffectableBool() : base()
        {
            OnValueChange = new BoolEvent();
        }

        public AffectableBool(bool v) : base(v)
        {
            OnValueChange = new BoolEvent();
        }

        public override AffectableValue Clone()
        {
            return new AffectableBool
            {
                OriginalValue = OriginalValue,
                CurrentValue = CurrentValue,
                affectFuncs = new List<Tuple<string, Func<bool, bool, bool>>>(affectFuncs)
            };
        }
        
        //public static implicit operator AffectableBool(bool i)
        //{
        //    return new AffectableBool(i);
        //}

        public static implicit operator bool(AffectableBool i)
        {
            if (ReferenceEquals(i, null)) return false;
            return i.Get();
        }
        
        //OverLoading == operator
        public static bool operator ==(AffectableBool x, AffectableBool y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Get() == y.Get();
        }

        //OverLoading != operator
        public static bool operator !=(AffectableBool x, AffectableBool y)
        {
            return !(x == y); //This will call to operator == simple way to implement !=
        }

        //Always override GetHashCode(),Equals when overloading ==
        public override bool Equals(object obj)
        {
            return this == (AffectableBool)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Logical negation operator. Returns True if the operand is False, Null
        // if the operand is Null, or False if the operand is True.
        public static AffectableBool operator !(AffectableBool x)
        {
            return new AffectableBool(!x.Get());
        }

        // Definitely true operator. Returns true if the operand is True, false
        // otherwise.
        public static bool operator true(AffectableBool x)
        {
            return x.Get();
        }

        // Definitely false operator. Returns true if the operand is False, false
        // otherwise.
        public static bool operator false(AffectableBool x)
        {
            return !x.Get();
        }

    }
}