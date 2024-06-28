using AssetBundles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BundleManager : MonoBehaviour
{
	public static BundleManager instance;

	private string[] strURLList = new string[2]
	{
		"http://52.79.148.36/Publish/SweetRoad/AssetBundles/",
		"https://dev.cookapps.com/Builds/sweetroad_m/"
	};

	private string pathToBundles;

	private static readonly string LocalManifestFileName = "Manifest_V_5_5.dat";

	private Dictionary<string, AssetBundle> bundles;

	private Dictionary<string, string> bundleVariants;

	private AssetBundleManifest manifest;

	public UIDownloadEpisode uiDownloadEpsiode;

	public bool isReady => !object.ReferenceEquals(manifest, null);

	public AssetBundleManifest Manifest => manifest;

	public string GetBundleUrl()
	{
		return pathToBundles;
	}

	private void Awake()
	{
		if (object.ReferenceEquals(instance, null))
		{
			instance = this;
		}
		else if (!object.ReferenceEquals(instance, this))
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public IEnumerator LoadManifest()
	{
		string platform = Utility.GetPlatformName();
		if (string.IsNullOrEmpty(platform))
		{
			yield break;
		}
		pathToBundles = strURLList[(int)MonoSingleton<NetworkManager>.Instance.ConnectURL];
		if (MonoSingleton<NetworkManager>.Instance.ConnectURL == NetworkManager.URLList.Release && MonoSingleton<ServerDataTable>.Instance.m_dicTableOption.ContainsKey("RESOURCE_HOST_M") && !string.IsNullOrEmpty(MonoSingleton<ServerDataTable>.Instance.m_dicTableOption["RESOURCE_HOST_M"]))
		{
			pathToBundles = MonoSingleton<ServerDataTable>.Instance.m_dicTableOption["RESOURCE_HOST_M"];
		}
		pathToBundles = pathToBundles + platform + "/";
		bundles = new Dictionary<string, AssetBundle>();
		bundleVariants = new Dictionary<string, string>();
		if (MonoSingleton<IRVManager>.Instance.CurrentNetStatus == InternetReachabilityVerifier.Status.Offline)
		{
			string fileURL2 = Path.Combine(Application.persistentDataPath, LocalManifestFileName);
			if (File.Exists(fileURL2))
			{
				byte[] readBytes = File.ReadAllBytes(fileURL2);
				if (readBytes == null)
				{
					yield break;
				}
				AssetBundle acr = AssetBundle.LoadFromMemory(readBytes);
				yield return acr;
				if (acr == null)
				{
					yield break;
				}
				manifest = (AssetBundleManifest)acr.LoadAsset("AssetBundleManifest");
				yield return manifest;
				if (manifest == null)
				{
					yield break;
				}
			}
		}
		else
		{
			WWW www = new WWW(pathToBundles + platform);
			try
			{
				yield return www;
				if (!string.IsNullOrEmpty(www.error) || www.assetBundle == null)
				{
					yield break;
				}
				manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest));
				string fileURL = Path.Combine(Application.persistentDataPath, LocalManifestFileName);
				if (File.Exists(fileURL))
				{
					File.Delete(fileURL);
				}
				try
				{
					File.WriteAllBytes(fileURL, www.bytes);
				}
				catch (Exception)
				{
				}
				yield return null;
				www.assetBundle.Unload(unloadAllLoadedObjects: false);
			}
			finally
			{
			}
		}
		if (isReady)
		{
		}
	}

	public bool IsBundleLoaded(string bundleName)
	{
		if (!isReady)
		{
			return false;
		}
		return bundles.ContainsKey(bundleName);
	}

	public void RegisterVariant(string bundleName, string variantName)
	{
		if (!bundleVariants.ContainsValue(bundleName))
		{
			bundleVariants.Add(bundleName, variantName);
		}
	}

	public UnityEngine.Object GetAssetFromBundle(string bundleName, string assetName)
	{
		if (!IsBundleLoaded(bundleName))
		{
			return null;
		}
		return bundles[bundleName].LoadAsset(assetName);
	}

	public void OnlyDownloadBundle(string bundleName)
	{
		StartCoroutine(OnlyDownloadBundleCoroutine(bundleName));
	}

	private IEnumerator OnlyDownloadBundleCoroutine(string bundleName)
	{
		if (!(manifest == null) && !Caching.IsVersionCached(pathToBundles + bundleName, Manifest.GetAssetBundleHash(bundleName)))
		{
			string[] dependencies = manifest.GetAllDependencies(bundleName);
			for (int i = 0; i < dependencies.Length; i++)
			{
				yield return StartCoroutine(OnlyDownloadBundleCoroutine(dependencies[i]));
			}
			bundleName = RemapVariantName(bundleName);
			string url = pathToBundles + bundleName;
			WWW www = WWW.LoadFromCacheOrDownload(url, manifest.GetAssetBundleHash(bundleName));
			try
			{
				yield return www;
			}
			finally
			{
			}
		}
	}

	public IEnumerator LoadBundle(string bundleName)
	{
		if (!IsBundleLoaded(bundleName))
		{
			yield return StartCoroutine(LoadBundleCoroutine(bundleName));
		}
	}

	private IEnumerator LoadBundleCoroutine(string bundleName)
	{
		if (IsBundleLoaded(bundleName) || manifest == null)
		{
			yield break;
		}
		string[] dependencies = manifest.GetAllDependencies(bundleName);
		for (int i = 0; i < dependencies.Length; i++)
		{
			yield return StartCoroutine(LoadBundleCoroutine(dependencies[i]));
			if ((bool)uiDownloadEpsiode)
			{
				uiDownloadEpsiode.currentDownloadBundleIndex++;
			}
		}
		bundleName = RemapVariantName(bundleName);
		string url = pathToBundles + bundleName;
		WWW www = WWW.LoadFromCacheOrDownload(url, manifest.GetAssetBundleHash(bundleName));
		try
		{
			do
			{
				if (uiDownloadEpsiode != null)
				{
					uiDownloadEpsiode.SetProgress(www.progress);
				}
				yield return null;
			}
			while (!www.isDone);
			if (string.IsNullOrEmpty(www.error))
			{
				bundles.Add(bundleName, www.assetBundle);
			}
		}
		finally
		{
		}
	}

	private void OnDisable()
	{
		if (isReady && bundles != null)
		{
			foreach (KeyValuePair<string, AssetBundle> bundle in bundles)
			{
				bundle.Value.Unload(unloadAllLoadedObjects: false);
			}
			bundles.Clear();
		}
	}

	public void UnloadAssetBundle(string assetBundleName)
	{
		if (!isReady || bundles == null || !bundles.ContainsKey(assetBundleName))
		{
			return;
		}
		string[] allDependencies = manifest.GetAllDependencies(assetBundleName);
		for (int i = 0; i < allDependencies.Length; i++)
		{
			if (bundles.ContainsKey(allDependencies[i]))
			{
				bundles[allDependencies[i]].Unload(unloadAllLoadedObjects: false);
				bundles.Remove(allDependencies[i]);
			}
		}
	}

	private string RemapVariantName(string assetBundleName)
	{
		string[] array = assetBundleName.Split('.');
		if (!bundleVariants.TryGetValue(array[0], out string value))
		{
			return assetBundleName;
		}
		string[] allAssetBundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
		string text = array[0] + "." + value;
		if (Array.IndexOf(allAssetBundlesWithVariant, text) < 0)
		{
			return assetBundleName;
		}
		return text;
	}
}
