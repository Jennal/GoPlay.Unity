using UnityEditor;
using UnityEngine;

namespace GoPlay.Editor
{
    public static class SaveData
    {
        [MenuItem("GoPlay/Clear PlayerPrefs", false)]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            EditorUtility.DisplayDialog("", "PlayerPrefs Cleared!", "OK");
        }
    }
}