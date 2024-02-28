using UnityEditor;
using GoPlay.Editor.ExcelModifiers;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class Excel2Unity
    {
        [MenuItem("GoPlay/Excel/Generate All In Once", false, 1)]
        public static void Execute()
        {
            FileUtil.DeleteFileOrDirectory(ExporterConsts.cacheFile);
            VFXAssets2Excel.Execute();
            if (!Excel2CSharp.DoExecute()) return;
            Excel2Enum.DoExecute();
            Excel2ScriptableObjectsWithoutCompile.Execute();
        }
    }
}