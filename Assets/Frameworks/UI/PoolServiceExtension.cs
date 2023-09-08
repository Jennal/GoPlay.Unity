using GoPlay.Services;

namespace GoPlay.Framework.UI
{
    public static class PoolServiceExtension
    {
        public static T SpawnUI<T>(this PoolService service, params object[] data)
            where T : UIPanel, IPoolable
        {
            if (!service.IsRegisted<T>())
            {
                var prefab = UILoader.Load<T>();
                service.Register<T>(prefab);
            }

            return service.Spawn<T>(data);
        }
    }
}