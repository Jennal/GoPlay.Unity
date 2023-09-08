using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void DoParallel(this MonoBehaviour self, params IEnumerator[] coroutines)
        {
            self.StartCoroutine(Parallel(self, coroutines));
        }

        public static IEnumerator Parallel(this MonoBehaviour self, params Coroutine[] coroutines)
        {
            foreach (var item in coroutines)
            {
                yield return item;
            }
        }
        
        public static IEnumerator Parallel(this MonoBehaviour self, IEnumerable<Coroutine> coroutines)
        {
            foreach (var item in coroutines)
            {
                yield return item;
            }
        }
        
        public static IEnumerator Parallel(this MonoBehaviour self, params IEnumerator[] coroutines)
        {
            return Parallel(self, coroutines as IEnumerable<IEnumerator>);
        }
        
        public static IEnumerator Parallel(this MonoBehaviour self, IEnumerable<IEnumerator> coroutines)
        {
            var list = new List<Coroutine>();
            foreach (var coroutine in coroutines)
            {
                list.Add(self.StartCoroutine(coroutine));
            }

            return Parallel(self, list);
        }

        public static T EnsureComponent<T>(this MonoBehaviour self)
            where T : MonoBehaviour
        {
            var result = self.GetComponent<T>();
            if (!result) self.gameObject.AddComponent<T>();
            return result;
        }
    }
}