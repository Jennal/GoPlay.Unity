using UnityEngine;

namespace GoPlay.Utils
{
    /// <summary>
    /// 正态分布随机数生成器
    /// </summary>
    public class RandBoxMuller
    {
        private const float TOW_PI = Mathf.PI * 2f;
        private float z1;
        private bool generate;
	
        public float Rand(float mu, float sigma)
        {
            generate = !generate;

            if (!generate)
                return z1 * sigma + mu;

            float u1, u2;
            do
            {
                u1 = GoPlay.Utils.Rand.Range(0f, 1f);
                u2 = GoPlay.Utils.Rand.Range(0f, 1f);
            }
            while ( u1 <= float.Epsilon );

            float z0;
            z0 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(TOW_PI * u2);
            z1 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(TOW_PI * u2);
            return z0 * sigma + mu;
        }
    }
}