using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using GoPlay.Framework.Extensions;

namespace GoPlay.Affectables
{
    //TODO: FIX-ME: 序列化不友好，需改进
    [Serializable]
    public abstract class AffectableValue
    {
        public abstract AffectableValue Clone();
        public abstract void Unregister(string tag);
    }

    [Serializable]
    public class AffectableValue<T> : AffectableValue
    {
        public T OriginalValue;
        public T CurrentValue;

		protected T PrevValue;

        public UnityEvent<T> OnValueChange;

        /// <summary>
        /// (Original, Current) => Result
        /// </summary>
        protected List<Tuple<string, Func<T, T, T>>> affectFuncs = new List<Tuple<string, Func<T, T, T>>>();

        public AffectableValue() { }

        public AffectableValue(T val)
        {
            OriginalValue = val;
            CurrentValue = val;
			PrevValue = val;

		}

        public void Reset()
        {
            affectFuncs.Clear();
            
            CurrentValue = OriginalValue;
            PrevValue = OriginalValue;
        }
        
        //注册动态修改函数
        public void Register(string tag, Func<T, T, T> buffFunc)
        {
            affectFuncs.Add(new Tuple<string, Func<T, T, T>>(tag, buffFunc));
            calculate();
        }
        
        //注册动态修改函数
        public void Register(Func<T, T, T> buffFunc)
        {
            Register("", buffFunc);
        }

        //移除动态修改函数
        public override void Unregister(string tag)
        {
            var item = affectFuncs.Where(o => o.Item1 == tag).ToList();
            if (!item.Any()) return;
            
            affectFuncs.RemoveRange(item);
            calculate();
        }
        
        //移除动态修改函数
        public void Unregister(Func<T, T, T> buffFunc)
        {
            var item = affectFuncs.Where(o => o.Item2 == buffFunc).ToList();
            if (!item.Any()) return;
            
            affectFuncs.RemoveRange(item);
            calculate();
        }

        //Buff计算
        protected virtual void calculate()
        {
            CurrentValue = OriginalValue;
            foreach (var f in affectFuncs)
            {
                CurrentValue = f.Item2(OriginalValue, CurrentValue);
            }
			if (!CurrentValue.Equals(PrevValue) && OnValueChange != null)
			{
				OnValueChange.Invoke(CurrentValue);
			}
			PrevValue = CurrentValue;
		}

        //获取Buff计算后的值
        public virtual T Get()
        {
            return CurrentValue;
        }

        //获取原始值
        public virtual T GetOriginal()
        {
            return OriginalValue;
        }

        //只能对原始值做修改
        //public void Set(T val)
        //{
        //    OriginalValue = val;
        //    calculate();
        //}

        //只能对原始值做修改
        public virtual void Set(Func<T, T> func)
        {
            OriginalValue = func(OriginalValue);
            calculate();
        }

        public override string ToString()
        {
            return CurrentValue.ToString();
        }

        public override AffectableValue Clone()
        {
            return new AffectableValue<T>
            {
                OriginalValue = OriginalValue,
                CurrentValue = CurrentValue,
                affectFuncs = new List<Tuple<string, Func<T, T, T>>>(affectFuncs)
            };
        }
    }
}