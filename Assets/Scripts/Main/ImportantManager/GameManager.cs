using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


namespace WordJourney
{

//	using UnityEngine.SceneManagement;
	using System.IO;

	public class GameManager : MonoBehaviour
	{

		private static volatile GameManager instance;
		public static GameManager Instance
		{
			get
			{
				if (instance == null)
				{

					instance = TransformManager.FindTransform("GameManager").GetComponent<GameManager>();

					instance.gameDataCenter = new GameDataCenter();

					//					instance.UIManager = new UIManager ();

					instance.persistDataManager = new PersistDataManager();

					DontDestroyOnLoad(instance);
				}
				return instance;
			}
		}

		public SoundManager soundManager;

		public GameDataCenter gameDataCenter;

		public UIManager UIManager;

		public PersistDataManager persistDataManager;

		public PronounceManager pronounceManager;

		public PurchaseManager purchaseManager;

		// 当前版本信息【格式：x.xx  例如：1.01 代表1.01版，  版本更新时版本号需比上一版大】
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

		/// <summary>
		/// 退出程序时执行的逻辑【主要用于数据保存工作】
		/// </summary>
		void OnApplicationQuit()
		{
			//SaveDataOnApplicationQuit();
		}

#warning 打包ios时需要在xcode中更改以下内容
		/// <summary>
		/// 退出时保存数据的逻辑
		/// 该方法用于和ios交互
		/// xcode中UnityAppController.mm  修改如下
		/*
		- (void)applicationDidEnterBackground:(UIApplication*)application
        {
            UnitySendMessage("GameManager","SaveDataOnApplicationQuit",""); // 退回主界面时都进行保存操作
            ::printf("-> applicationDidEnterBackground()\n");
        }
        */
		/// </summary>
		public void SaveDataOnApplicationQuit(){
			//if (hasSavedDataOnQuit)
   //         {
   //             return;
   //         }
            
   //         if (ExploreManager.Instance != null)
   //         {
   //             ExploreManager.Instance.UpdateWordDataBase();
   //         }
   //         persistDataManager.SaveBuyRecord();
   //         persistDataManager.SaveGameSettings();
   //         persistDataManager.SaveMapEventsRecord();
   //         persistDataManager.SaveCompletePlayerData();
			//persistDataManager.SaveMiniMapRecords();

            //MySQLiteHelper.Instance.CloseAllConnections();

            //hasSavedDataOnQuit = true;
		}

		void OnLowMemory()
		{
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
        
        


	}
}
