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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace libx
{
    public static class MenuItems
    {
        #region hmf 清除标记
        private const string ClearAssetsTag = "Assets/AssetBundles/清除标记";
        #endregion
        private const string KApplyBuildRules = "XAsset/Build Rules";
        private const string KBuildPlayer = "XAsset/Build Player";
        private const string KBuildManifest = "XAsset/Build Manifest";
        private const string KBuildAssetBundles = "XAsset/Build AssetBundles";
        private const string KClearAssetBundles = "XAsset/Clear AssetBundles";
        private const string KCopyAssetBundles = "XAsset/Copy AssetBundles";
        private const string KCopyPath = "Assets/Copy the Path";

        //[MenuItem("Assets/Regroup")]
        //private static void Regroup()
        //{
        //    foreach (var item in Selection.GetFiltered<Object>(SelectionMode.DeepAssets))
        //    {
        //        var path = AssetDatabase.GetAssetPath(item);
        //        var s = Path.GetFileNameWithoutExtension(path);
        //        if (s.Contains("-"))
        //        {
        //            var group = s.Substring(0, s.LastIndexOf('-')).Replace("-", "/");
        //            var dir = Path.GetDirectoryName(path);
        //            var newdir = dir + "/" + group;
        //            if (!Directory.Exists(newdir))
        //            {
        //                Directory.CreateDirectory(newdir);
        //            }
        //            FileUtil.MoveFileOrDirectory(path, newdir + "/" + Path.GetFileName(path));
        //        }
        //    }
        //}

        [MenuItem("Assets/Apply Rule/Text", false, 1)]
        private static void ApplyRuleText()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternText);
        }

        [MenuItem("Assets/Apply Rule/Prefab", false, 1)]
        private static void ApplyRulePrefab()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternPrefab);
        }

        [MenuItem("Assets/Apply Rule/PNG", false, 1)]
        private static void ApplyRulePNG()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternPNG);
        }

        [MenuItem("Assets/Apply Rule/Material", false, 1)]
        private static void ApplyRuleMaterail()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternMaterial);
        }

        [MenuItem("Assets/Apply Rule/Controller", false, 1)]
        private static void ApplyRuleController()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternController);
        }

        [MenuItem("Assets/Apply Rule/Asset", false, 1)]
        private static void ApplyRuleAsset()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternAsset);
        }

        [MenuItem("Assets/Apply Rule/Scene", false, 1)]
        private static void ApplyRuleScene()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternScene);
        }

        [MenuItem("Assets/Apply Rule/Dir", false, 1)]
        private static void ApplyRuleDir()
        {
            var rules = BuildScript.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternDir);
        }

        private static void AddRulesForSelection(BuildRules rules, string searchPattern)
        {
            var isDir = rules.searchPatternDir.Equals(searchPattern);
            foreach (var item in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(item);
                var rule = new BuildRule();
                rule.searchPath = path;
                rule.searchPattern = searchPattern;
                rule.searchOption = SearchOption.AllDirectories;
                rule.unshared = false;
                rule.searchDirOnly = isDir;
                ArrayUtility.Add<BuildRule>(ref rules.rules, rule);
            }
            EditorUtility.SetDirty(rules);
            AssetDatabase.SaveAssets();
            Selection.activeObject = rules;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem(KApplyBuildRules)]
        private static void ApplyBuildRules()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.ApplyBuildRules();
            watch.Stop();
            Debug.Log("ApplyBuildRules " + watch.ElapsedMilliseconds + " ms.");
        }

        [MenuItem(KBuildPlayer)]
        private static void BuildStandalonePlayer()
        {
            BuildScript.BuildStandalonePlayer();
        }

        [MenuItem(KBuildManifest)]
        private static void BuildManifest()
        {
            BuildScript.BuildManifest();
        }

        [MenuItem(KBuildAssetBundles)]
        private static void BuildAssetBundles()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
            watch.Stop();
            Debug.Log("BuildAssetBundles " + watch.ElapsedMilliseconds + " ms.");
        }

        [MenuItem(KClearAssetBundles)]
        private static void ClearAssetBundles()
        {
            BuildScript.ClearAssetBundles();
        }

        [MenuItem(KCopyAssetBundles)]
        private static void CopyAssetBundles()
        {
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Assets.AssetBundles));
            AssetDatabase.Refresh();
        }

        [MenuItem(KCopyPath)]
        private static void CopyPath()
        {
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = assetPath;
            Debug.Log(assetPath);
        }

        #region hmf 清除标记
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
