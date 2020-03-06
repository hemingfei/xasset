using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
    public class EditorRuntimeInitializeOnLoad
    {
        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize()
        {
            EditorUtility.ClearProgressBar();
            var settings = BuildScript.GetSettings();
            if (settings.localServer)
            {
                bool isRunning = LaunchLocalServer.IsRunning();
                if (!isRunning)
                {
                    LaunchLocalServer.Run();
                }
                Assets.dataPath = string.Empty;
                var manifest = BuildScript.GetManifest();
                if (string.IsNullOrEmpty(manifest.downloadURL))
                {
                    manifest.downloadURL = "http://127.0.0.1:7888/";
                    EditorUtility.SetDirty(manifest);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                bool isRunning = LaunchLocalServer.IsRunning();
                if (isRunning)
                {
                    LaunchLocalServer.KillRunningAssetBundleServer();
                }
                Assets.dataPath = System.Environment.CurrentDirectory;
            }
            Assets.assetBundleMode = settings.runtimeMode;
            Assets.getPlatformDelegate = BuildScript.GetPlatformName;
            Assets.loadDelegate = AssetDatabase.LoadAssetAtPath;
            Debug.Log(Assets.dataPath);
        }

        [InitializeOnLoadMethod]
        private static void OnEditorInitialize()
        {
            BuildScript.GetManifest();
            BuildScript.GetSettings();
            BuildScript.GetBuildRules();
        }
    }
}
