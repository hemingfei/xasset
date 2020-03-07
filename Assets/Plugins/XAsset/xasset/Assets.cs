//
// Assets.cs
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

using Hegametech.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace xasset
{
	public delegate UnityEngine.Object LoadDelegate (string path, Type type);

	public delegate string GetPlatformDelegate ();

	[MonoSingletonAttribute("[HE GAME TECH]/Assets")]
	public class Assets : TMonoSingleton<Assets>
	{
		public static Dictionary<string, int> bundleAssets { get { return _bundleAssets; } }

		public const string AssetBundles = "AssetBundles";
		public const string AssetsManifestAsset = "Assets/Res/Manifest.asset";
		public static bool assetBundleMode = true;
		public static LoadDelegate loadDelegate = null;
		public static GetPlatformDelegate getPlatformDelegate = null;

		public static string dataPath { get; set; }

		public static string platform { get; private set; }

		public static string assetBundleDataPath { get; private set; }

		public static string downloadURL { get; set; }

		public static string assetBundleDataPathURL { get; private set; }

		public static string updatePath { get; private set; }

		private static string[] _bundles = new string[0];
		private static readonly Dictionary<string, int> _bundleAssets = new Dictionary<string, int> ();
		private static readonly List<Asset> _assets = new List<Asset> ();
		private static readonly List<Asset> _unusedAssets = new List<Asset> ();

		private static string GetPlatformForAssetBundles (RuntimePlatform platform)
		{
			switch (platform) {
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.WebGLPlayer:
				return "WebGL";
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				return "Windows";
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.OSXEditor:
				return "OSX";
			default:
				return null;
			}
		}

		public static string GetRelativeUpdatePath (string path)
		{
			return updatePath + path;
		}

		public static string GetDownloadURL (string filename)
		{
			return downloadURL + filename;
		}

		public static string GetAssetBundleDataPathURL (string filename)
		{
			return assetBundleDataPathURL + filename;
		}

		public static void Initialize (Action onSuccess, Action<string> onError)
		{
			var instance = FindObjectOfType<Assets> ();
			if (instance == null) {
				instance = Instance;
			}

			InitPaths (); 

			Versions.Load ();

			Log (Hegametech.Framework.Utility.Text.Format("Initialize->assetBundleMode {0}", Assets.assetBundleMode));

			if (Assets.assetBundleMode) {
				InitBundles (onSuccess, onError);
			} else {
				if (onSuccess != null) {
					onSuccess.Invoke ();
				}
			}
		}

		private static void InitBundles (Action onSuccess, Action<string> onError)
		{
			Bundles.OverrideBaseDownloadingUrl += Bundles_overrideBaseDownloadingURL;
			Bundles.Initialize (() => {
				var asset = LoadAsync (Assets.AssetsManifestAsset, typeof(AssetManifest));
				asset.completed += obj => {
					var manifest = obj.asset as AssetManifest;
					if (manifest == null) {
						if (onError != null) {
							onError.Invoke ("manifest == null");
						}
						return;
					}
					downloadURL = Path.Combine (manifest.downloadURL, platform) + Path.DirectorySeparatorChar;
					Bundles.activeVariants = manifest.activeVariants;
					_bundles = manifest.bundles;
					var dirs = manifest.dirs;
					_bundleAssets.Clear ();
					for (int i = 0, max = manifest.assets.Length; i < max; i++) {
						var item = manifest.assets [i];
						_bundleAssets [string.Format ("{0}/{1}", dirs [item.dir], item.name)] = item.bundle;
					}
					if (onSuccess != null) { 
						onSuccess.Invoke ();
					}
					obj.Release ();
				};
			}, onError);
		}

		private static void InitPaths ()
		{
			var protocal = string.Empty;

			if (Application.platform == RuntimePlatform.IPhonePlayer ||
			    Application.platform == RuntimePlatform.OSXEditor ||
			    Application.platform == RuntimePlatform.WindowsEditor) {
				protocal = "file://";
			} else if (Application.platform == RuntimePlatform.OSXPlayer ||
			           Application.platform == RuntimePlatform.WindowsPlayer) {
				protocal = "file:///";
			}

			if (string.IsNullOrEmpty (dataPath))
				dataPath = Application.streamingAssetsPath;

			platform = getPlatformDelegate != null ? getPlatformDelegate () : GetPlatformForAssetBundles (Application.platform);
			var platformAssetBundle = Path.Combine (AssetBundles, platform);
			assetBundleDataPath = Path.Combine (dataPath, platformAssetBundle) + Path.DirectorySeparatorChar;
			assetBundleDataPathURL = protocal + assetBundleDataPath;
			updatePath = Path.Combine (Application.persistentDataPath, platformAssetBundle) + Path.DirectorySeparatorChar;
		}

		public static string[] GetAllDependencies (string path)
		{
			string assetBundleName;
			return GetAssetBundleName (path, out assetBundleName) ? Bundles.GetAllDependencies (assetBundleName) : null;
		}

		public static SceneAsset LoadScene (string path, bool async, bool addictive)
		{
			var asset = async ? new SceneAssetAsync (path, addictive) : new SceneAsset (path, addictive);
			GetAssetBundleName (path, out asset.assetBundleName);
			asset.Load ();
			asset.Retain ();
			_assets.Add (asset);
			return asset;
		}

		public static void UnloadScene (string path)
		{
			for (int i = 0, max = _assets.Count; i < max; i++) {
				var item = _assets [i];
				if (!item.name.Equals (path))
					continue;
				Unload (item);
				break;
			}
		}

		public static Asset Load (string path, Type type)
		{
			return Load (path, type, false);
		}

		public static Asset LoadAsync (string path, Type type)
		{
			return Load (path, type, true);
		}

		public static void Unload (Asset asset)
		{
			asset.Release ();
			for (var i = 0; i < _unusedAssets.Count; i++) {
				var item = _unusedAssets [i];
				if (!item.name.Equals (asset.name))
					continue;
				item.Unload ();
				_unusedAssets.RemoveAt (i);
				return;
			}
		}

		private void Update ()
		{
			for (var i = 0; i < _assets.Count; i++) {
				var item = _assets [i];
				if (item.Update () || !item.IsUnused ())
					continue;
				_unusedAssets.Add (item);
				_assets.RemoveAt (i);
				i--;
			}

			for (var i = 0; i < _unusedAssets.Count; i++) {
				var item = _unusedAssets [i];
				item.Unload ();
				Log(Hegametech.Framework.Utility.Text.Format("Unload-> {0}", item.name));
			}

			_unusedAssets.Clear ();

			Bundles.Update ();
		}

		[Conditional ("LOG_ENABLE")]
		private static void Log (string s)
		{
			Hegametech.Framework.Log.Debug(Hegametech.Framework.Utility.Text.Format("[Assets] {0}", s));
		}

		private static Asset Load (string path, Type type, bool async)
		{
			if (string.IsNullOrEmpty (path)) {
				Debug.LogError ("invalid path");
				return null;
			}

			for (int i = 0, max = _assets.Count; i < max; i++) {
				var item = _assets [i];
				if (!item.name.Equals (path))
					continue;
				item.Retain ();
				return item;
			} 

			string assetBundleName;
			Asset asset;
			if (GetAssetBundleName (path, out assetBundleName)) {
				asset = async ? new BundleAssetAsync (assetBundleName) : new BundleAsset (assetBundleName);
			} else {
				if (path.StartsWith ("http://", StringComparison.Ordinal) ||
				    path.StartsWith ("https://", StringComparison.Ordinal) ||
				    path.StartsWith ("file://", StringComparison.Ordinal) ||
				    path.StartsWith ("ftp://", StringComparison.Ordinal) ||
				    path.StartsWith ("jar:file://", StringComparison.Ordinal))
					asset = new WebAsset ();
				else
					asset = new Asset ();
			}

			asset.name = path;
			asset.assetType = type;
			_assets.Add (asset);
			asset.Load ();
			asset.Retain ();

			Log (Hegametech.Framework.Utility.Text.Format("Load->{0}|{1}", path, assetBundleName));
			return asset;
		}

		private static bool GetAssetBundleName (string path, out string assetBundleName)
		{
			if (path.Equals (Assets.AssetsManifestAsset)) {
				assetBundleName = Path.GetFileNameWithoutExtension (path).ToLower ();
				return true;
			}

			assetBundleName = null;
			int bundle;
			if (!_bundleAssets.TryGetValue (path, out bundle))
				return false;
			assetBundleName = _bundles [bundle];
			return true;
		}

		private static string Bundles_overrideBaseDownloadingURL (string bundleName)
		{
			return !File.Exists (Assets.GetRelativeUpdatePath (bundleName)) ? null : Assets.updatePath;
		}
	}
}