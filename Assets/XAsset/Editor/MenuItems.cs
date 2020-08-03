//
// AssetsMenuItem.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2020 fjy
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

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace libx
{
    public static class MenuItems
    {
        #region hmf 清除标记
        private const string ClearAssetsTag = "Assets/Bundles/清除标记";

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

        #region hmf 标记
        public static void MarkAssetsWithFile()
        {
            GroupByFilename();
        }

        public static void MarkAssetsWithDir()
        {
            GroupByDirectory();
        }

        public static void ApplyBuildRules()
        {
            BuildRules();
        }

        public static void ApplyBuildAB()
        {
            BuildBundles();
        }
        #endregion

        [MenuItem("XASSET/Build Rules")]
        private static void BuildRules()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildRules(); 
            watch.Stop();
            Debug.Log("BuildRules " + watch.ElapsedMilliseconds + " ms.");
        } 
        
        [MenuItem("XASSET/Build Bundles")]
        private static void BuildBundles()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildRules();
            BuildScript.BuildAssetBundles();
            watch.Stop();
            Debug.Log("BuildBundles " + watch.ElapsedMilliseconds + " ms.");
        } 
        
        [MenuItem("XASSET/Build Player")]
        private static void BuildStandalonePlayer()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildStandalonePlayer();
            watch.Stop();
            Debug.Log("BuildPlayer " + watch.ElapsedMilliseconds + " ms.");
        }

        [MenuItem("XASSET/View/PersistentDataPath")]
        private static void ViewDataPath()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }

        [MenuItem("Assets/ToJson")]
        private static void ToJson()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var json = JsonUtility.ToJson(Selection.activeObject);
            File.WriteAllText(path.Replace(".asset", ".json"), json);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/GroupBy/Filename")]
        private static void GroupByFilename()
        {
            GroupAssets(GroupBy.Filename);
        } 

        [MenuItem("Assets/GroupBy/Directory")]
        private static void GroupByDirectory()
        {
            GroupAssets(GroupBy.Directory); 
        }
        
        [MenuItem("Assets/GroupBy/Explicit")]
        private static void GroupByExplicitLevel1()
        {
            GroupAssets(GroupBy.Explicit);  
        } 
        
        private static void GroupAssets(GroupBy nameBy)
        {
            var selection = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var rules = BuildScript.GetBuildRules();
            foreach (var o in selection)
            {
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path) || Directory.Exists(path))
                {
                    continue;
                }  
                rules.GroupAsset(path, nameBy);
            }

            EditorUtility.SetDirty(rules);
            AssetDatabase.SaveAssets();
        } 
        
        [MenuItem("Assets/PatchBy/Level0")]
        private static void PatchByLevel0()
        {
            PatchAssets(PatchBy.Level0);
        } 

        [MenuItem("Assets/PatchBy/Level1")]
        private static void PatchByLevel1()
        {
            PatchAssets(PatchBy.Level1); 
        }
        
        [MenuItem("Assets/PatchBy/Level2")]
        private static void PatchByLevel2()
        {
            PatchAssets(PatchBy.Level2);  
        } 
        
        [MenuItem("Assets/PatchBy/Level3")]
        private static void PatchByLevel3()
        {
            PatchAssets(PatchBy.Level3);  
        } 
        
        [MenuItem("Assets/PatchBy/Level4")]
        private static void PatchByLevel4()
        {
            PatchAssets(PatchBy.Level4);  
        } 
        
        private static void PatchAssets(PatchBy patch)
        {
            var selection = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var rules = BuildScript.GetBuildRules();
            foreach (var o in selection)
            {
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path) || Directory.Exists(path))
                {
                    continue;
                }  
                rules.PatchAsset(path, patch);
            }

            EditorUtility.SetDirty(rules);
            AssetDatabase.SaveAssets();
        } 

        #region Tools

        [MenuItem("XASSET/View/CRC")]
        private static void GetCRC()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path)) return;

            using (var fs = File.OpenRead(path))
            {
                var crc = Utility.GetCRC32Hash(fs);
                Debug.Log(crc);
            }
        }

        [MenuItem("XASSET/View/MD5")]
        private static void GetMD5()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path)) return;

            using (var fs = File.OpenRead(path))
            {
                var crc = Utility.GetMD5Hash(fs);
                Debug.Log(crc);
            }
        }

        [MenuItem("XASSET/Take a screenshot")]
        private static void Screenshot()
        {
            var path = EditorUtility.SaveFilePanel("截屏", null, "screenshot_", "png");
            if (string.IsNullOrEmpty(path)) return;

            ScreenCapture.CaptureScreenshot(path);
        }

        #endregion
    }
}