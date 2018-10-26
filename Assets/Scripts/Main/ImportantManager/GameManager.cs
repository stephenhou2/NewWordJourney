using UnityEngine;
using System.Data;


namespace WordJourney
{

//	using UnityEngine.SceneManagement;
	using System.IO;

    /// <summary>
    /// 游戏控制器
    /// </summary>
	public class GameManager : MonoBehaviour
	{
        // 游戏控制器单例
		private static volatile GameManager instance;
		public static GameManager Instance
		{
			get
			{
				if (instance == null)
				{

					instance = TransformManager.FindTransform("GameManager").GetComponent<GameManager>();

					instance.gameDataCenter = new GameDataCenter();

					instance.persistDataManager = new PersistDataManager();

					DontDestroyOnLoad(instance);
				}
				return instance;
			}
		}

        // 音频控制器
		public SoundManager soundManager;
        // 数据中心
		public GameDataCenter gameDataCenter;
        // UI控制器
		public UIManager UIManager;
        // 数据存取类
		public PersistDataManager persistDataManager;
        // 发音控制器
		public PronounceManager pronounceManager;
        // 购买控制器
		public PurchaseManager purchaseManager;

		// 当前版本信息【格式：x.xx  例如：1.01 代表1.01版，版本更新时版本号需比上一版大】
		public float currentVersion;


	

#warning 如果决定使用scene来进行场景转换打开下面的代码
		//		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		//		static public void CallbackInitialization()
		//		{
		//			//register the callback to be called everytime the scene is loaded
		//			SceneManager.sceneLoaded += OnSceneLoaded;
		//		}
		//
		//		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		//		{
		//			switch (arg0.name) {
		//			case "GameScene":
		//				TransformManager.FindTransform ("GameLoader").GetComponent<GameLoader> ().SetUpHomeView ();
		//				break;
		//			case "ExploreScene":
		//				int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;
		//
		//				ResourceLoader exploreSceneLoader = ResourceLoader.CreateNewResourceLoader ();
		//
		//				ResourceManager.Instance.LoadAssetsWithBundlePath (exploreSceneLoader, "explore/scene", () => {
		//
		//					ExploreManager.Instance.GetComponent<ExploreManager> ().SetupExploreView (currentExploreLevel);
		//
		//				}, true);
		//				break;
		//
		//			}
		//
		//		}



#warning 打包ios时需要在xcode中更改以下内容
		/// <summary>
		/// 保存所有数据
		/// 该方法用于和ios交互
		/// xcode中UnityAppController.mm  修改如下
		/*
		- (void)applicationDidEnterBackground:(UIApplication*)application
        {
            UnitySendMessage("GameManager","SaveAllData",""); // 退回主界面时都进行保存操作
            ::printf("-> applicationDidEnterBackground()\n");
        }
        */
		/// </summary>
		public void SaveAllData(){
			Debug.Log("save data ");
			if(Camera.main.GetComponent<GameLoader>().dataReady){
				persistDataManager.SaveDataInExplore(null, true);
			}
            // 如果退出的时候游戏数据还没有ready，用备用数据存储一次，防止数据丢失
			else if(persistDataManager.dataBackUp != null) {
				
				if (persistDataManager.dataBackUp.playerData != null){
					DataHandler.SaveInstanceDataToFile<PlayerData>(persistDataManager.dataBackUp.playerData, CommonData.playerDataFilePath);
				}
				if(persistDataManager.dataBackUp.buyRecord != null){
					DataHandler.SaveInstanceDataToFile<BuyRecord>(persistDataManager.dataBackUp.buyRecord, CommonData.buyRecordFilePath);
				}
				if(persistDataManager.dataBackUp.chatRecords != null){
					DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(persistDataManager.dataBackUp.chatRecords, CommonData.chatRecordsFilePath);
				}
				if(persistDataManager.dataBackUp.currentMapEventsRecord != null){
					DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(persistDataManager.dataBackUp.currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
				}
				if(persistDataManager.dataBackUp.gameSettings != null){
					DataHandler.SaveInstanceDataToFile<GameSettings>(persistDataManager.dataBackUp.gameSettings, CommonData.gameSettingsDataFilePath);
				}
				if(persistDataManager.dataBackUp.mapEventsRecords != null){
					DataHandler.SaveInstanceListToFile<MapEventsRecord>(persistDataManager.dataBackUp.mapEventsRecords, CommonData.mapEventsRecordFilePath);
				}
				if(persistDataManager.dataBackUp.miniMapRecord != null){
					DataHandler.SaveInstanceDataToFile<MiniMapRecord>(persistDataManager.dataBackUp.miniMapRecord, CommonData.miniMapRecordFilePath);
				}
				if(persistDataManager.dataBackUp.playRecords != null){
					DataHandler.SaveInstanceListToFile<PlayRecord>(persistDataManager.dataBackUp.playRecords, CommonData.playRecordsFilePath);
				}

				persistDataManager.dataBackUp = null;
			}         
		}

        /// <summary>
        /// 退出程序时调用
        /// </summary>
		void OnApplicationQuit(){
			SaveAllData();
		}

        /// <summary>
        /// 程序失去焦点时调用
        /// </summary>
        /// <param name="focus">If set to <c>true</c> focus.</param>
		private void OnApplicationFocus(bool focus)
		{
			if(!focus){
				SaveAllData();
			}
            
		}

        /// <summary>
        /// 内存警告时调用
        /// </summary>
		void OnLowMemory()
		{
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
        
        


	}
}
