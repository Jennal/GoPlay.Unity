using System.Collections;
using UnityEngine;

namespace GoPlay.Utils
{
    public static class AnimatorExtensions
    {
        public static IEnumerator WaitForAnimationEnd(this Animator self, string name, int layer = 0)
        {
            if (!self) yield break;
            
            yield return self.WaitForEnterState(name, layer);
            yield return self.WaitForCurrentAnimationEnd(layer);
        }

        public static IEnumerator WaitForEnterState(this Animator self, string name, int layer=0)
        {
            if (!self) yield break;
            
            while (self && !self.GetCurrentAnimatorStateInfo(layer).IsName(name))
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator WaitForCurrentAnimationEnd(this Animator self, int layer=0)
        {
            if (!self) yield break;
            
            var enterStateInfo = self.GetCurrentAnimatorStateInfo(layer);
            var enterTime = enterStateInfo.normalizedTime;
            
            while (self && self.GetCurrentAnimatorStateInfo(layer).fullPathHash == enterStateInfo.fullPathHash && 
                   self.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f && 
                   enterTime <= self.GetCurrentAnimatorStateInfo(layer).normalizedTime)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}