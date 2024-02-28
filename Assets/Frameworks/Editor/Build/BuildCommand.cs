using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace GoPlay.Editor.Build
{
    public struct ABBuildInfo
    {
        public string outputDirectory;
        public BuildAssetBundleOptions options;
        public BuildTarget buildTarget;
    }

    /// 
    /// Put me inside an Editor folder
    /// 
    /// Add a Build menu on the toolbar to automate multiple build for different platform
    /// 
    /// Use #define BUILD in your code if you have build specification 
    /// Specify all your Target to build All
    /// 
    /// Install to Android device using adb install -r "pathofApk"
    /// 
    public class BuidCommand : MonoBehaviour
    {
        static readonly string androidKeystoreName = Path.Combine(Application.dataPath, "../Keys/spck.jks");
        const string androidKeystorePass = "GoPlay904";
        const string androidKeyaliasName = "GoPlay spck";
        const string androidKeyaliasPass = "GoPlay904";

        // const string scriptDefineSymbols = "ODIN_INSPECTOR;TextMeshPro;AMPLIFY_SHADER_EDITOR";

        static readonly string OutputDir = Path.Combine(Application.dataPath, "../Build");

        static BuildTarget[] targetToBuildAll =
        {
            BuildTarget.Android,
            BuildTarget.StandaloneWindows64
        };

        private static string BuildPathRoot
        {
            get
            {
                string path = OutputDir;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        static int AndroidLastBuildVersionCode
        {
            get { return PlayerPrefs.GetInt("LastVersionCode", -1); }
            set { PlayerPrefs.SetInt("LastVersionCode", value); }
        }

        static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.XboxOne:
                    return BuildTargetGroup.XboxOne;
                case BuildTarget.tvOS:
                    return BuildTargetGroup.tvOS;
                case BuildTarget.Switch:
                    return BuildTargetGroup.Switch;
                case BuildTarget.NoTarget:
                default:
                    return BuildTargetGroup.Standalone;
            }
        }

        static string GetExtension(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    return ".app";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.iOS:
                    return ".ipa";
                case BuildTarget.Android:
                    return ".apk";
            }

            return "";
        }

        private static string _buildPathRoot;
        private static string _productName;
        private static string _scriptDefineSymbols;
        private static string _packageName;
        private static string _packageSuffix;
        private static string _suffix;
        private static bool _isDebug = false;
        private static bool _isIL2Cpp = false;

        private static void CustomizedCommandLine()
        {
            //args
            var cmdActions = new Dictionary<string, Delegate>
            {
                {
                    "-buildPathRoot", new Action<string>(argument => { _buildPathRoot = argument; })
                },
                {
                    "-productName", new Action<string>(argument => { _productName = argument; })
                },
                {
                    "-define", new Action<string>(argument => { _scriptDefineSymbols = argument; })
                },
                {
                    "-packageName", new Action<string>(argument => { _packageName = argument; })
                },
                {
                    "-debug", new Action(() => { _isDebug = true; })
                },
                {
                    "-il2cpp", new Action(() => { _isIL2Cpp = true; })
                },
                {
                    "-suffix", new Action<string>(argument => { _suffix = argument;})
                },
            };

            Delegate actionCache;
            string[] cmdArguments = Environment.GetCommandLineArgs();

            for (int count = 0; count < cmdArguments.Length; count++)
            {
                if (cmdActions.ContainsKey(cmdArguments[count]))
                {
                    actionCache = cmdActions[cmdArguments[count]];
                    if (actionCache.Method.GetParameters().Length > 0)
                    {
                        actionCache.DynamicInvoke(cmdArguments[count + 1]);
                        count++;
                    }
                    else
                    {
                        actionCache.DynamicInvoke();
                    }
                }
            }

            //Defaults
            if (string.IsNullOrEmpty(_productName))
            {
                _productName = PlayerSettings.productName;
            }

            if (string.IsNullOrEmpty(_buildPathRoot))
            {
                _buildPathRoot = BuildPathRoot;
            }

            if (!string.IsNullOrEmpty(_scriptDefineSymbols) && !_scriptDefineSymbols.StartsWith(";"))
            {
                _scriptDefineSymbols = ";" + _scriptDefineSymbols;
            }

            if (string.IsNullOrEmpty(_packageName))
            {
                _packageName = PlayerSettings.applicationIdentifier;
            }
            
            if (!string.IsNullOrEmpty(_suffix) && !_suffix.StartsWith("_"))
            {
                _suffix = "_" + _suffix;
            }
        }

        static BuildPlayerOptions GetDefaultPlayerOptions(BuildTargetGroup targetGroup)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            var listScenes = new List<string>();
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.enabled)
                    listScenes.Add(s.path);
            }

            buildPlayerOptions.scenes = listScenes.ToArray();
            buildPlayerOptions.options = BuildOptions.None;

            if (_isDebug)
            {
                buildPlayerOptions.options |= BuildOptions.AllowDebugging | BuildOptions.Development |
                                              BuildOptions.ConnectWithProfiler;
            }

            if (_isIL2Cpp)
            {
                PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
            }

            // To define
            // buildPlayerOptions.locationPathName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\LightGunBuild\\Android\\LightGunMouseArcadeRoom.apk";
            // buildPlayerOptions.target = BuildTarget.Android;

            return buildPlayerOptions;
        }

        //TODO: 增加自动添加Sprite Packing Tag
        static void DefaultBuild(BuildTarget buildTarget, string suffix="", string packageSuffix="")
        {
            _suffix = suffix;
            _packageSuffix = packageSuffix;
            
            CustomizedCommandLine();
            BuildAssetBundle(buildTarget);

            LogFormat("Building on platform {0}...", buildTarget.ToString());

            var now = DateTime.Now;
            BuildTargetGroup targetGroup = ConvertBuildTarget(buildTarget);

            //Save Old Value
            var oldProductName = PlayerSettings.productName;
            var oldPackageName = PlayerSettings.applicationIdentifier;
            var oldVersion = PlayerSettings.bundleVersion;
            // var oldDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            var idx = IndexOfNth(oldVersion, ".", 2);
            if (idx >= 0) oldVersion = oldVersion.Substring(0, idx);

            _packageName = oldPackageName + packageSuffix;
            
            PlayerSettings.productName = _productName;
            PlayerSettings.applicationIdentifier = _packageName;
            // PlayerSettings.Android.bundleVersionCode++; //PostProcessor已经自增
            PlayerSettings.bundleVersion =
                $"{oldVersion}.{now:yyyyMMdd}{_suffix}.{PlayerSettings.Android.bundleVersionCode}";

            string path = Path.Combine(_buildPathRoot, targetGroup.ToString());
            string name = _productName.Replace(" ", "_")
                                      .Replace(":", "")
                                        + "_" + PlayerSettings.bundleVersion
                                        + $"_{now:HHmmss}"
                                        + GetExtension(buildTarget);

            // string defineSymbole =
            //     scriptDefineSymbols +
            //     _scriptDefineSymbols; //PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            // PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole);

            PlayerSettings.Android.keystoreName = androidKeystoreName;
            PlayerSettings.Android.keystorePass = androidKeystorePass;
            PlayerSettings.Android.keyaliasName = androidKeyaliasName;
            PlayerSettings.Android.keyaliasPass = androidKeyaliasPass;

            BuildPlayerOptions buildPlayerOptions = GetDefaultPlayerOptions(targetGroup);

            buildPlayerOptions.locationPathName = Path.Combine(path, name);
            buildPlayerOptions.target = buildTarget;

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);

            Log($"Start building: {buildPlayerOptions.locationPathName}");
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            LogReport(report);

            if (buildTarget == BuildTarget.Android)
                AndroidLastBuildVersionCode = PlayerSettings.Android.bundleVersionCode;

            //Reset Old Value
            PlayerSettings.productName = oldProductName;
            PlayerSettings.applicationIdentifier = oldPackageName;
            PlayerSettings.bundleVersion = oldVersion;
            // PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, oldDefineSymbols);

            if (!Application.isBatchMode)
            {
                EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);
            }

            LogFormat("Done!");
        }

        private static void LogReport(BuildReport report)
        {
            if (report.summary.result != BuildResult.Succeeded)
            {
                Log("Build Result: Failed!");
                return;
            }

            var output = @$"Build Result: Success
Output: {report.summary.outputPath}
Start Time: {report.summary.buildStartedAt}
End Time: {report.summary.buildEndedAt}
Used Time: {report.summary.buildEndedAt.Subtract(report.summary.buildStartedAt)}";
            Log(output);
        }

        static void BuildAssetBundle(BuildTarget buildTarget)
        {
            LogFormat("Building Assetbundles... {0}", buildTarget);

            ABBuildInfo buildInfo = new ABBuildInfo();

            buildInfo.outputDirectory = Path.Combine(Path.GetDirectoryName(Application.dataPath), "AssetBundles");
            buildInfo.outputDirectory = Path.Combine(buildInfo.outputDirectory, buildTarget.ToString());
            buildInfo.options = BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression;
            buildInfo.buildTarget = buildTarget;

            if (Directory.Exists(buildInfo.outputDirectory)) Directory.Delete(buildInfo.outputDirectory, true);
            Directory.CreateDirectory(buildInfo.outputDirectory);

            BuildAssetBundles(buildInfo);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            if (Directory.Exists(Application.streamingAssetsPath)) Directory.Delete(Application.streamingAssetsPath, true);
            DirectoryCopy(buildInfo.outputDirectory, Application.streamingAssetsPath);

            LogFormat("Done");

            // CopyDbFile();
            ClearCache();
        }

        public static bool BuildAssetBundles(ABBuildInfo info)
        {
            var builds = new List<AssetBundleBuild>();
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
            {
                var assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(name);
                //打包的时候，不显示声明dependency，BuildPipeline会自动生成依赖关系
                //var dependency = new List<string>();
                //foreach (var assetName in assetNames)
                //{
                //    var dependList = AssetDatabase.GetDependencies(assetName, true);
                //    dependency.AddRange(dependList);
                //}

                builds.Add(new AssetBundleBuild()
                {
                    assetBundleName = name,
                    assetNames = assetNames,
                    //assetNames = assetNames
                    //                .Union(dependency)
                    //                .Distinct()
                    //                .Where(o => !o.EndsWith(".cs"))
                    //                .ToArray(),
                });
                //LogFormat("{0} => {1}", name, string.Join(", ", assetNames));
                //LogFormat("Dependancy: {0}", string.Join(", ", dependency.ToArray()));
            }

            var manifest = BuildPipeline.BuildAssetBundles(info.outputDirectory, builds.ToArray(), info.options,
                info.buildTarget);
            //取消version文件的生成
            //if ((info.options & BuildAssetBundleOptions.DryRunBuild) != BuildAssetBundleOptions.DryRunBuild)
            //{
            //    BuildVersionConfig(manifest, info.outputDirectory);
            //}

            return true;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                //LogFormat("Copying {0} => {1}", file.FullName, temppath);
                file.CopyTo(temppath, true);
            }


            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        public static int IndexOfNth(string str, string value, int nth = 1)
        {
            if (nth <= 0)
                throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
            int offset = str.IndexOf(value);
            for (int i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }

            return offset;
        }

        // [MenuItem("Build/Db/Copy Db to StreamingAssets")]
        // static void CopyDbFile()
        // {
        //     LogFormat("Copying Data File...");
        //
        //     var srcPath = Path.Combine(Application.dataPath, "Res/Data/DB/ff19db.bytes");
        //     var dstPath = Path.Combine(Application.streamingAssetsPath, "ff19db.bytes");
        //     File.Copy(srcPath, dstPath, true);
        //
        //     LogFormat("Done");
        //     EditorUtility.DisplayDialog("", "Copy Data File Done!", "OK");
        // }

        [MenuItem("Build/Build Specific/Android")]
        static void BuildAndroid()
        {
            PlayerSettings.Android.useAPKExpansionFiles = false;
            DefaultBuild(BuildTarget.Android);
        }
        
        [MenuItem("Build/Build Specific/Android Review")]
        static void BuildAndroidReview()
        {
            PlayerSettings.Android.useAPKExpansionFiles = false;
            DefaultBuild(BuildTarget.Android, "_review", ".review");
        }

        [MenuItem("Build/Build Specific/Android Google Play")]
        static void BuildAndroidGooglePlay()
        {
            PlayerSettings.Android.useAPKExpansionFiles = true;
            DefaultBuild(BuildTarget.Android, "_google_play");
        }

        [MenuItem("Build/Assetbundle/Android")]
        static void BuildAssetbundleAndroid()
        {
            BuildAssetBundle(BuildTarget.Android);
        }

        [MenuItem("Build/Assetbundle/Windows")]
        static void BuildAssetbundleWindows()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows);
        }

        //[MenuItem("Build/Build Specific/Build Win32")]
        //static void BuildWin32()
        //{
        //    DefaultBuild(BuildTarget.StandaloneWindows);
        //}

        //[MenuItem("Build/Build Specific/Build Win64")]
        //static void BuildWin64()
        //{
        //    DefaultBuild(BuildTarget.StandaloneWindows64);
        //}

        // [MenuItem("Build/Test")]
        // static void Test()
        // {
            //PlayerSettings.applicationIdentifier = "com.yd.ghost";
            // EditorUtility.RevealInFinder(@"C:\Users\FYXMzhengle\Desktop\见鬼场景测试\Android\见鬼场景测试_1.0.20180712.3.apk");
            // Log(PlayerSettings.applicationIdentifier);
        // }

        //[MenuItem("Build/Get Build Number")]
        //static void BuildNumber()
        //{
        //    Log("Current/Last: " + PlayerSettings.Android.bundleVersionCode + "/" + AndroidLastBuildVersionCode);
        //}

        //[MenuItem("Build/Build Number/Up Build Number")]
        //static void BuildNumberUp()
        //{
        //    PlayerSettings.Android.bundleVersionCode++;
        //    BuildNumber();
        //}

        //[MenuItem("Build/Build Number/Down Build Number")]
        //static void BuildNumberDown()
        //{
        //    PlayerSettings.Android.bundleVersionCode--;
        //    BuildNumber();
        //}

        //[MenuItem("Build/Build All")]
        //static void BuildAll()
        //{
        //    var buildTargetLeft = new List<BuildTarget>(targetToBuildAll);

        //    if (buildTargetLeft.Contains(EditorUserBuildSettings.activeBuildTarget))
        //    {
        //        DefaultBuild(EditorUserBuildSettings.activeBuildTarget);
        //        buildTargetLeft.Remove(EditorUserBuildSettings.activeBuildTarget);
        //    }

        //    foreach (var b in buildTargetLeft)
        //    {
        //        DefaultBuild(b);
        //    }
        //}

        [MenuItem("Build/Clear Cache")]
        static void ClearCache()
        {
            if (Caching.ClearCache())
            {
                LogFormat("Clear Cache Done!");
            }
            else
            {
                LogFormat("Clear Cache Failed!");
            }
        }

        static void Log(string msg)
        {
            if (Application.isBatchMode) Console.WriteLine(msg);
            else Debug.Log(msg);
        }

        static void LogFormat(string fmt, params object[] args)
        {
            if (Application.isBatchMode) Console.WriteLine(fmt, args);
            else Debug.LogFormat(fmt, args);
        }
    }
}