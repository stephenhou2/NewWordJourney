using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.Networking;

	public class MyResourceManager:SingletonMono<MyResourceManager>{



		private enum AssetBundleErrorCode{
			None = 0,
			ManifestNull = 1,
			AssetBundleNull = 2,
			AssetNull = 3

		}

		/// <summary>
		/// 空构造函数
		/// </summary>
		private MyResourceManager(){
			
		}

	

		/// <summary>
		/// 已加载的bundle缓存字典
		/// </summary>
		private Dictionary<string,AssetBundle> bundleCacheDic;


		private Dictionary<string,string[]> dependencyDic;

		/// <summary>
		/// 正在进行中的网络请求
		/// </summary>
		private Dictionary<string,UnityWebRequest> inloadingWebRequestDic;


		/// <summary>
		/// manifest主信息
		/// </summary>
		/// <value>The main manifest.</value>
		public AssetBundleManifest mainManifest{ get; private set;}



		/// <summary>
		/// manifest主信息是否已准备好
		/// </summary>
		public bool isManifestReady{get; private set;}

		private AssetBundleErrorCode errorCode;

		void Awake(){
			DontDestroyOnLoad (this);
			Initilize ();
		}


		public void Initilize(){

			if (bundleCacheDic == null) {
				bundleCacheDic = new Dictionary<string, AssetBundle> ();
			}

			if (dependencyDic == null) {
				dependencyDic = new Dictionary<string, string[]> ();
			}

			if (inloadingWebRequestDic == null) {
				inloadingWebRequestDic = new Dictionary<string, UnityWebRequest> ();
			}

			isManifestReady = false;

			InitilizeManifest ();

		}

		/// <summary>
		/// 初始化manifest信息文件
		/// </summary>
		private void InitilizeManifest(){

			Debug.Log ("initialize manifest!!!");

			string manifestName = CommonData.assetBundleRootName;

			string manifestBundlePath = GetBundleFilePath (manifestName);

			AssetBundle ab = AssetBundle.LoadFromFile(manifestBundlePath);

			if (ab == null) {
				errorCode = AssetBundleErrorCode.ManifestNull;
			}

			mainManifest = ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");

			isManifestReady = mainManifest != null;

//			Debug.Log (mainManifest);

			return;

		}

		/// <summary>
		/// 根据不同平台获取AssetBundle的本地绝对路径
		/// </summary>
		/// <returns>The bundle file path.</returns>
		public string GetBundleFilePath(string bundleName){
			return string.Format("{0}/{1}/{2}",Application.streamingAssetsPath, CommonData.assetBundleRootName, bundleName);
		}
			

		/// <summary>
		/// 获取缓存中的bundle数据【如果缓存中没有指定名称的bundle数据，或者指定名称的bundle的依赖数据没有完全加载，都返回null】
		/// </summary>
		/// <returns>The loaded asset bundle.</returns>
		/// <param name="bundleName">Bundle name.</param>
		private AssetBundle GetLoadedAssetBundle(string bundleName){

			AssetBundle loadedBundle = null;

			// 如果缓存中没有找到，直接返回null
			if (!bundleCacheDic.TryGetValue (bundleName, out loadedBundle)) {
				return null;
			}
				
			string[] dependecyArray = null;
			if (!dependencyDic.TryGetValue (bundleName,out dependecyArray)) {
				// 如果缓存中没有查询到依赖文件，则获取依赖文件
				dependecyArray = GetAndRecordDependecies (bundleName);

			}

			// 如果bundle没有依赖关系，则直接返回bundle
			if (dependecyArray.Length == 0) {
				return loadedBundle;
			}

			// 查询依赖的bundle是否已经完全加载
			for (int i = 0; i < dependecyArray.Length; i++) {

				AssetBundle dependencyBundle = GetLoadedAssetBundle (dependecyArray [i]);

				// 如果依赖的bundle还有没有加载的，也返回null
				if (dependencyBundle == null) {
					return null;
				}

			}

			return loadedBundle;

		}

		private string[] GetAndRecordDependecies(string bundleName){

			string[] dependencies = mainManifest.GetAllDependencies (bundleName);

			dependencyDic.Add (bundleName, dependencies);

			return dependencies;

		}


		/// <summary>
		/// 同步加载资源
		/// </summary>
		/// <returns>The assets.</returns>
		/// <param name="bundleName">Bundle name.</param>
		/// <param name="fileName">File name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] LoadAssets<T>(string bundleName,string assetName = null)
			where T:Object
		{
			AssetBundle ab = GetLoadedAssetBundle (bundleName);

			if (ab == null) {

				LoadAllDependencies (bundleName);

				ab = LoadAssetBundle (bundleName);

			}

			if (assetName != null) {
				return new T[]{ab.LoadAsset<T> (assetName)};
			}

			return ab.LoadAllAssets<T> ();

		}

		/// <summary>
		/// 异步加载资源
		/// </summary>
		/// <returns>The asset async.</returns>
		/// <param name="bundleName">Bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public AssetBundleRequest LoadAssetAsync<T>(string bundleName, string assetName = null)
			where T:Object
		{

			AssetBundle ab = GetLoadedAssetBundle (bundleName);

			if (ab == null) {

				LoadAllDependencies (bundleName);

				ab = LoadAssetBundle (bundleName);

			}

			if (ab == null) {
				errorCode = AssetBundleErrorCode.AssetBundleNull;
			}

			if (assetName != null) {
				return ab.LoadAssetAsync<T> (assetName);
			}

			return ab.LoadAllAssetsAsync<T> ();

		}

		public WWW LoadAssetsUsingWWW(string filePath)
		{
			//路径前缀
			#if UNITY_EDITOR || UNITY_IOS
			string pathHead = "file://";
			#elif UNITY_ANDROID
			string pathHead = "";
			#endif

			string absolutePath = string.Format ("{0}{1}{2}", pathHead, Application.streamingAssetsPath, filePath);

			WWW www = new WWW (absolutePath);

			return www;

		}

		/// <summary>
		/// 使用www方式从网络加载资源
		/// </summary>
		/// <returns>The assets from web.</returns>
		/// <param name="url">URL.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public UnityWebRequest LoadAssetsFromWeb(string url){
			uint version = 0;

			UnityWebRequest request = UnityWebRequest.GetAssetBundle (url, version);

			return request;

		}


		private void LoadAllDependencies(string bundleName){

			string[] dependencies = null;

			if(!dependencyDic.TryGetValue(bundleName,out dependencies)){
				dependencies = GetAndRecordDependecies (bundleName);
			}

			if (dependencies == null) {
				return;
			}

			for (int i = 0; i < dependencies.Length; i++) {

				AssetBundle ab = GetLoadedAssetBundle (dependencies [i]);

				if (ab == null) {

					LoadAllDependencies (dependencies [i]);

					LoadAssetBundle (dependencies [i]);

				}

			}

		}


		/// <summary>
		/// 如果缓存中有指定名称的bundle，则直接返回该bundle，否则重新加载
		/// </summary>
		/// <returns>The asset bundle.</returns>
		/// <param name="bundleName">Bundle name.</param>
		private AssetBundle LoadAssetBundle(string bundleName){

			AssetBundle loadedBundle = GetLoadedAssetBundle (bundleName);

			if (loadedBundle == null) {
				
				string abFullPath = GetBundleFilePath (bundleName);

				loadedBundle = AssetBundle.LoadFromFile (abFullPath);

				bundleCacheDic.Add (bundleName, loadedBundle);

				Debug.LogFormat ("asset:{0}--加载进内存", bundleName);

			}

			if (loadedBundle == null) {
				errorCode = AssetBundleErrorCode.AssetBundleNull;
			}

			return loadedBundle;

		}



		public void UnloadAssetBundle(string bundleName,bool unloadAllLoadedObjects){

			if (!bundleCacheDic.ContainsKey (bundleName)) {
				return;
			}

			Debug.LogFormat ("卸载assetbundle{0}---完全卸载：{1}", bundleName, unloadAllLoadedObjects);

			bundleCacheDic [bundleName].Unload (unloadAllLoadedObjects);

			bundleCacheDic.Remove (bundleName);

		}



	}
}
