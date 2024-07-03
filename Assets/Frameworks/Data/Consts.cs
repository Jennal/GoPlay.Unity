using UnityEngine;

namespace GoPlay.Data
{
    public interface IDesignResolution
    {
        Vector2 Design { get; }
    }
    
    public static partial class Consts
    {
        public static IDesignResolution Resolution;
        
        public static class Language
        {
            public const string Default = "en_us";
        }
        
        static partial void Init();

        static Consts()
        {
            Init();
        }
    }
}