using GoPlay.Managers;
using GoPlay.Modules.Translate;
using UnityEngine;

namespace GoPlay.Translate
{
    public class LangGameObject : LangObjectBase<GameObject>
    {
        public override void SetData()
        {
            var data = GetData();
            var idx = transform.GetSiblingIndex();
            var pos = transform.position;
            
            var go = Instantiate(data, transform.parent);
            go.transform.position = pos;
            go.transform.SetSiblingIndex(idx);
            var lgo = go.GetComponent<LangGameObject>();
            if (!lgo) lgo = go.AddComponent<LangGameObject>();
            lgo.Id = Id;
            lgo.Lang = ModuleManager.Get<LangModule>().CurrentLang;
            
            Destroy(gameObject);
        }

        public override GameObject GetData()
        {
            return ModuleManager.Get<LangModule>().GetGameObject(Id);
        }
    }
}