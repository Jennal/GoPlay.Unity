using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public enum Self { Exclude, Include }
    public enum Inactive { Exclude, Include }

    public static class ComponentExtensions
    {
        public static T GetComponentInParents<T>(this Component component, Self self, Inactive inactive)
            where T : class
        {
            return self switch
            {
                Self.Include => component.GetComponentInParent<T>(inactive == Inactive.Include),
                _ when component.transform.parent is Transform parent && parent != null => parent
                    .GetComponentInParent<T>(inactive == Inactive.Include),
                _ => null
            };
        }

        public static T GetComponentInParents<T>(this Component component, Self self) 
            where T : class
        {
            return component.GetComponentInParents<T>(self,
                component.gameObject.activeInHierarchy ? Inactive.Exclude : Inactive.Include);
        }
    }
}