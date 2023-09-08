//using UnityEditor;
//using UnityEditor.AddressableAssets;
//using UnityEditor.AddressableAssets.Settings;
//
//namespace Veewo.Editor.Excel2ScriptableObject
//{
//    public static class AddressableHelper
//    {
//        public static AddressableAssetSettings Settings => AddressableAssetSettingsDefaultObject.Settings;
//        
//        public static AddressableAssetEntry GetOrCreateEntry(string path, string groupName="")
//        {
//            var guid = AssetDatabase.AssetPathToGUID(path);
//            var group = string.IsNullOrEmpty(groupName) ? Settings.DefaultGroup : GetOrCreateGroup(groupName);
//            var entry = Settings.CreateOrMoveEntry(guid, group);
//            return entry;
//        }
//
//        public static AddressableAssetGroup GetOrCreateGroup(string name = "Default")
//        {
//            var group = Settings.FindGroup(name);
//            if (group == null)
//            {
//                group = Settings.CreateGroup(name, false, false, false, Settings.DefaultGroup.Schemas);
//            }
//
//            return group;
//        }
//    }
//}