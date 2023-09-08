using System;

namespace GoPlay.Attributes
{
    public class PrefabName : Attribute
    {
        string m_name;
        public string Name => m_name;

        public PrefabName(string name)
        {
            m_name = name;
        }
    }
}