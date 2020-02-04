//
// AssetsMenuItem.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2019 fjy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Plugins.XAsset.Editor
{
    public static class AssetsMenuItem
    {
        #region hmf edit
        private const string ClearAssetsTag = "Assets/AssetBundles/清除标记";
        #endregion
        private const string KMarkAssetsWithDir = "Assets/AssetBundles/按目录标记";
        private const string KMarkAssetsWithFile = "Assets/AssetBundles/按文件标记";
        private const string KMarkAssetsWithName = "Assets/AssetBundles/按名称标记";
        private const string KBuildManifest = "Assets/AssetBundles/生成配置";
        private const string KBuildAssetBundles = "Assets/AssetBundles/生成资源包";
        private const string KBuildPlayer = "Assets/AssetBundles/生成播放器";
        private const string KCopyPath = "Assets/复制路径";
        private const string KMarkAssets = "标记资源";
        private const string KCopyToStreamingAssets = "Assets/AssetBundles/拷贝到StreamingAssets";
        public static string assetRootPath;

        [InitializeOnLoadMethod]
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
				Utility.dataPath = string.Empty;
            }
            else
            {
                bool isRunning = LaunchLocalServer.IsRunning();
                if (isRunning)
                {
                    LaunchLocalServer.KillRunningAssetBundleServer();
                }
				Utility.dataPath = System.Environment.CurrentDirectory; 	
            } 
            Utility.downloadURL = BuildScript.GetManifest().downloadURL;
            Utility.assetBundleMode = settings.runtimeMode;
            Utility.getPlatformDelegate = BuildScript.GetPlatformName;
            Utility.loadDelegate = AssetDatabase.LoadAssetAtPath;
        }

        public static string TrimedAssetBundleName(string assetBundleName)
        {
            if(string.IsNullOrEmpty(assetRootPath))
                return assetBundleName;
            return assetBundleName.Replace(assetRootPath, "");
        }

        [MenuItem(KCopyToStreamingAssets)]
        private static void CopyAssetBundles()
        {
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundles));
            AssetDatabase.Refresh();
        }

        [MenuItem(KMarkAssetsWithDir)]
        public static void MarkAssetsWithDir()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath.Replace("\\", "/"); 
            var assetsManifest = BuildScript.GetManifest();
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset).Replace("\\", "/");
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = TrimedAssetBundleName(Path.GetDirectoryName(path).Replace("\\", "/")) + "_g";
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KMarkAssetsWithFile)]
        public static void MarkAssetsWithFile()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath.Replace("\\", "/"); 
            var assetsManifest = BuildScript.GetManifest();
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset).Replace("\\", "/");
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;

                var dir = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                if (dir == null)
                    continue;
                dir = dir.Replace("\\", "/") + "/";
                if (name == null)
                    continue;
                var assetBundleNameFull = Path.Combine(dir, name).Replace("\\", "/");
                var assetBundleName = TrimedAssetBundleName(assetBundleNameFull);
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KMarkAssetsWithName)]
        public static void MarkAssetsWithName()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath.Replace("\\", "/"); 
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var assetsManifest = BuildScript.GetManifest();
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset).Replace("\\", "/");
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = Path.GetFileNameWithoutExtension(path);
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KBuildManifest)]
        public static void BuildManifest()
        {
            BuildScript.BuildManifest();
        }

        [MenuItem(KBuildAssetBundles)]
        public static void BuildAssetBundles()
        {
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
        }

        [MenuItem(KBuildPlayer)]
        private static void BuildStandalonePlayer()
        {
            BuildScript.BuildStandalonePlayer();
        }

        [MenuItem(KCopyPath)]
        private static void CopyPath()
        {
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = assetPath;
            Debug.Log(assetPath);
        }

        #region hmf edit
        [MenuItem(ClearAssetsTag)]
        public static void ClearAssetNameAsFolderName()
        {
            string selectPath = Hegametech.Framework.Common.Editor.EditorPathHelper.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                return;
            }
            AutoClearAssetNameInFolder(selectPath);
            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log("Finish Clear AB Name.");
        }

        static bool IsAsset(string fileName)
        {
            if (fileName.EndsWith(".meta") || fileName.EndsWith(".gaf") || fileName.EndsWith(".DS_Store") || fileName.EndsWith(".cs"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        // 递归清理文件夹下所有Asset 文件
        /// </summary>
        /// <param name="folderPath">Asset目录下文件夹</param>
        private static void AutoClearAssetNameInFolder(string folderPath)
        {
            if (folderPath == null)
            {
                UnityEngine.Debug.LogWarning("Folder Path Is Null!");
                return;
            }
            // 清除文件夹的标记
            {
                AssetImporter folderAi = AssetImporter.GetAtPath(folderPath);
                if (folderAi == null)
                {
                    UnityEngine.Debug.LogError("Not Find Folder Asset:" + folderPath);
                }
                else
                {
                    folderAi.assetBundleName = null;
                }
            }

            string workPath = Hegametech.Framework.Common.Editor.EditorPathHelper.FullPathToAssetsPath(folderPath);
            string assetBundleName = null;
            //处理文件
            var filePaths = Directory.GetFiles(workPath);
            for (int i = 0; i < filePaths.Length; ++i)
            {
                if (!IsAsset(filePaths[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePaths[i]);

                string fullFileName = string.Format("{0}/{1}", folderPath, fileName);

                AssetImporter ai = AssetImporter.GetAtPath(fullFileName);
                if (ai == null)
                {
                    UnityEngine.Debug.LogError("Not Find Asset:" + fullFileName);
                    continue;
                }
                else
                {
                    ai.assetBundleName = assetBundleName;
                }
            }

            //递归处理文件夹
            var dirs = Directory.GetDirectories(workPath);
            for (int i = 0; i < dirs.Length; ++i)
            {
                string fileName = Path.GetFileName(dirs[i]);

                fileName = string.Format("{0}/{1}", folderPath, fileName);
                AutoClearAssetNameInFolder(fileName);
            }
        }
        #endregion
    }
}