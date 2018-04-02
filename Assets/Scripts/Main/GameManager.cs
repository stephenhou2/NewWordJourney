using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


namespace WordJourney
{

//	using UnityEngine.SceneManagement;
	using System.IO;

	public class GameManager : MonoBehaviour {

		private static volatile GameManager instance;  
		public static  GameManager Instance {  
			get {  
				if (instance == null) {  

					instance = TransformManager.FindTransform ("GameManager").GetComponent<GameManager>();

					instance.gameDataCenter = new GameDataCenter ();

//					instance.UIManager = new UIManager ();

					instance.persistDataManager = new PersistDataManager ();

					DontDestroyOnLoad (instance);
				}  
				return instance;  
			}
		}

		public SoundManager soundManager;

		public GameDataCenter gameDataCenter;

		public UIManager UIManager;

		public PersistDataManager persistDataManager;

		public PronounceManager pronounceManager;
			
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


	}
}
