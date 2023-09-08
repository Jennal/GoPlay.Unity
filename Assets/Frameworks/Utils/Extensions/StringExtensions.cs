using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace GoPlay.Utils
{
    public static class StringExtensions
    {
        public static (T1, T2) SplitTo<T1, T2>(this string str, string separates = ",|")
        {
            var arr = str.Split(separates.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(arr.Length >= 2);

            return (
                arr[0].Convert<T1>(), 
                arr[1].Convert<T2>()
            );
        }
        
        public static (T1, T2, T3) SplitTo<T1, T2, T3>(this string str, string separates = ",|")
        {
            var arr = str.Split(separates.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(arr.Length >= 3);

            return (
                arr[0].Convert<T1>(), 
                arr[1].Convert<T2>(),
                arr[2].Convert<T3>()
            );
        }
        
        public static (T1, T2, T3, T4) SplitTo<T1, T2, T3, T4>(this string str, string separates = ",|")
        {
            var arr = str.Split(separates.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(arr.Length >= 4);

            return (
                arr[0].Convert<T1>(), 
                arr[1].Convert<T2>(),
                arr[2].Convert<T3>(),
                arr[3].Convert<T4>()
            );
        }

        public static List<T> SplitToList<T>(this string str, string separates = ",|")
        {
            var arr = str.Split(separates.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return arr.Select(o => o.Convert<T>()).ToList();
        }

        public static T Convert<T>(this string val)
        {
            if (typeof(T) == typeof(string)) return (T)(object)val;
            if (typeof(T) == typeof(int)) return (T)(object)int.Parse(val);
            if (typeof(T) == typeof(long)) return (T)(object)long.Parse(val);
            if (typeof(T) == typeof(float)) return (T)(object)float.Parse(val);
            if (typeof(T) == typeof(double)) return (T)(object)double.Parse(val);
            if (typeof(T) == typeof(bool)) return (T)(object)bool.Parse(val);

            throw new Exception($"Type {typeof(T).Name} not recognized!");
        }
    }
}