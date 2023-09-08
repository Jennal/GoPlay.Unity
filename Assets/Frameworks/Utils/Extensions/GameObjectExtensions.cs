using UnityEngine;

namespace GoPlay.Utils
{
    public static class GameObjectExtensions
    {
        public static void RemoveComponent<T>(this GameObject gameObject)
            where T : Object
        {
            var comp = gameObject.GetComponent<T>();
            if (comp) Object.Destroy(comp);
        }

        //子节点少的物体
        public static int ActiveChildCount(this GameObject gameObject)
        {
            int i = 0;
            foreach (var item in gameObject.transform.GetAllChildren())
            {
                if (item.gameObject.activeSelf)
                {
                    i++;
                }
            }

            return i;
        }

        public static int GetFristNoActiveChild(this GameObject gameObject)
        {
            int i = 0;
            foreach (var item in gameObject.transform.GetAllChildren())
            {
                if (!item.gameObject.activeSelf)
                {
                    break;
                }

                i++;
            }

            return i;
        }
    }
}