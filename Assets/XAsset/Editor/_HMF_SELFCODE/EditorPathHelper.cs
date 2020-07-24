using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Hegametech.Framework.Common.Editor
{
    public class EditorPathHelper
    {
        /// <summary>
        /// 选中文件的Assets路径 (Assets/...)
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedDirAssetsPath()
        {
            string folderPath = string.Empty;
            var assets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            for (var i = 0; i < assets.Length; i++)
            {
                folderPath = AssetDatabase.GetAssetPath(assets[i]);
                if (!string.IsNullOrEmpty(folderPath) && File.Exists(folderPath))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                    break;
                }
            }
            return folderPath.Replace("\\", "/");
        }

        /// <summary>
        /// 全路径转换为Assets路径 (Assets/...)
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string FullPathToAssetsPath(string fullPath)
        {
            return fullPath.Substring(fullPath.IndexOf("Assets")).Replace("\\", "/");
        }

        /// <summary>
        /// Assets路径转换为相对路径 去掉"Assets/"
        /// </summary>
        /// <param name="assetsPath"></param>
        /// <returns></returns>
        public static string AssetsPathToRelevantPath(string assetsPath)
        {
            return assetsPath.Substring("Assets/".Length);
        }

        /// <summary>
        /// 相对路径转换为Assets路径, 加上"Assets/"
        /// </summary>
        /// <param name="assetsPath"></param>
        /// <returns></returns>
        public static string RelevantPathToAssetsPath(string assetsPath)
        {
            return "Assets/" + assetsPath;
        }
    }
}