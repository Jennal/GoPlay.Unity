using UnityEditor;
using UnityEngine;

namespace GoPlay.Editor
{
    public static class CopyGUID
    {
        [MenuItem("Assets/Copy GUID URI #&c")]
        private static void DoCopy()
        {
            var guid = Selection.assetGUIDs[0];
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GUIUtility.systemCopyBuffer = $"guid://{guid}/{path}";
        }
        
        [MenuItem("Assets/Copy GUID URI #&c", true)]
        private static bool Valid()
        {
            return Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1;
        }
        
        [MenuItem("Assets/Copy GUID &c")]
        private static void DoPureCopy()
        {
            var guid = Selection.assetGUIDs[0];
            GUIUtility.systemCopyBuffer = guid;
        }
        
        [MenuItem("Assets/Copy GUID &c", true)]
        private static bool ValidPure()
        {
            return Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1;
        }
    }
}
