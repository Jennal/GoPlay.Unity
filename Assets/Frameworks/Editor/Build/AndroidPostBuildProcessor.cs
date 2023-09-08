using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace GoPlay.Editor.Build
{
    public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 999;
        
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            // Debug.Log("Bulid path : " + path);
            // var gradlePropertiesFile = path + "/gradle.properties";
            // if (File.Exists(gradlePropertiesFile))
            // {
            //     File.Delete(gradlePropertiesFile);
            // }
            // var writer = File.CreateText(gradlePropertiesFile);
            // writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
            // writer.WriteLine("suppport_library_version=29.0.0");
            // writer.WriteLine("android.useDeprecatedNdk=true");
            // writer.WriteLine("android.useAndroidX=true");
            // writer.WriteLine("android.enableJetifier=true");
            // writer.Flush();
            // writer.Close();
        }
    }
}