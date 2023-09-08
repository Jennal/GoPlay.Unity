using System.Collections.Generic;
using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// 绝对值
        /// </summary>
        /// <returns></returns>
        public static float ToAbs(this float value)
        {
            return Mathf.Abs(value);
        }

        public static List<float> Separate(float startValue, float endValue, int num)
        {
            if (startValue.Equals(endValue))
            {
                return new List<float>(){startValue};
            }
            if (num <= 1)
            {
                return new List<float>(){startValue};
            }
            List<float> result = new List<float>();
            var delta = (endValue - startValue) / (num - 1);
            result.Add(startValue);
            for (int i = 0; i < num - 1; i++)
            {
                result.Add(startValue + delta * (i+1));
            }
            return result;
        }

        /// <summary>
        /// 将一个数从0开始等比例分割
        /// </summary>
        /// <param name="num">分割数</param>
        /// <returns></returns>
        public static List<float> Separate(this float value, int num)
        {
            return Separate(0, value, num);
        }

        /// <summary>
        /// 是否在范围内
        /// </summary>
        /// <param name="v1">值1</param>
        /// <param name="v2">值2</param>
        /// <param name="boundaryValue">是否包含边界值</param>
        /// <returns></returns>
        public static bool InRange(this float value, float v1 = 0, float v2 = 1, bool boundaryValue = true)
        {
            var min = Mathf.Min(v1, v2);
            var max = Mathf.Max(v1, v2);
            if (boundaryValue)
            {
                return value >= min && value <= max;
            }
            return value > min && value < max;
        }

        public static float ClampWith(this float value, float v1 = 0, float v2 = 1)
        {
            var min = Mathf.Min(v1, v2);
            var max = Mathf.Max(v1, v2);
            return Mathf.Clamp(value, min, max);
        }
    }
}