using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;

namespace GoPlay.Editor
{
    public class AssetProcessor : AssetPostprocessor
    {
        private const string PRESET_DIR = "Assets/Game/Res/Presets/";
        
        private void OnPreprocessTexture()
        {
            if (assetPath.StartsWith("Assets/Game/Res/SpineAni/"))
            {
                OnPreprocessSpineTexture();
            }
        }

        void OnPreprocessAsset()
        {
            var ext = Path.GetExtension(assetPath);
            switch (ext)
            {
                case ".mat":
                    OnPreprocessMaterial();
                    break;
            }
        }

        private void OnPreprocessMaterial()
        {
            if (assetPath.StartsWith("Assets/Game/Res/SpineAni/"))
            {
                // OnPreprocessSpineMaterial();
            }
        }

        private void OnPreprocessSpineTexture()
        {
            ImportWithPreset("SpineTextureImporter");
        }

        private void OnPreprocessSpineMaterial()
        {
            ImportWithPreset("SpineMaterialImporter");
        }

        private void ImportWithPreset(string presetName)
        {
            var path = Path.Combine(PRESET_DIR, presetName) + ".preset";
            var preset = AssetDatabase.LoadAssetAtPath<Preset>(path);
            preset.ApplyTo(assetImporter);
            assetImporter.SaveAndReimport();
        }
    }
}