using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Networking;


namespace WordJourney
{

	public class GameLoader : MonoBehaviour {

		public bool alwaysPersistData;

		void Awake(){
			Debug.Log (CommonData.persistDataPath);
//			#if UNITY_EDITOR
//			Debug.unityLogger.logEnabled = true;
//			#else
//			Debug.unityLogger.logEnabled = false;
//			#endif
		}

		void Start(){
//			if (MyTool.isIphoneX) {
//				GetComponent<SetCanvasBounds> ().enabled = true;
//			}
			PersistData();
		}

		private IEnumerator InitData(){

			yield return new WaitUntil(()=> MyResourceManager.Instance.isManifestReady);

			LoadDatas ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

			});
				
		}


		/// <summary>
		/// 初始化游戏基础数据
		/// </summary>
		private void LoadDatas(){
			
			GameManager.Instance.gameDataCenter.InitPersistentGameData ();

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);

//			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {
//				Debug.Log (Player.mainPlayer.allEquipedEquipments [i]);
//			}

		}





		private void PersistData(){

//			Debug.Log (CommonData.persistDataPath);

			DirectoryInfo persistDi = new DirectoryInfo (CommonData.persistDataPath);

#if UNITY_EDITOR || UNITY_IOS

			if (!persistDi.Exists) {
				DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);

				StartCoroutine ("InitData");
				return;
			}

			if (alwaysPersistData) {
				DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);
			}

			StartCoroutine ("InitData");
#elif UNITY_ANDROID
			if (!persistDi.Exists)
			{
				StartCoroutine("CopyDataForPersist");
				return;
			}

			if (alwaysPersistData)
			{
				StartCoroutine("CopyDataForPersist");
			}else{
				StartCoroutine("InitData");
			}

#endif

		}


		//安卓下使用www的方法拷贝到永久存在的文件夹
		IEnumerator CopyDataForPersist()
		{
			//创建文件夹目录
			Directory.CreateDirectory(Application.persistentDataPath + "/Data");
			Directory.CreateDirectory(Application.persistentDataPath + "/Data/MapData");
			Directory.CreateDirectory(Application.persistentDataPath + "/Data/NPCs");

			//循环拷贝文件
			for (int i = 0; i < CommonData.originDataArr.Length; i++)
			{

				//获取数组中的文件字典数据
				KVPair originDataKV = CommonData.originDataArr[i];

				string fileName = originDataKV.key;
				string filePath = originDataKV.value;

				if (fileName.Equals ("Level")) {
					//执行level的循环操作
					for (int j = 0; j < 30; j++) {
						filePath = "/Data/MapData/Level_" + j + ".json";

						Debug.Log (Application.streamingAssetsPath + filePath);

//						WWW data = new WWW(pathHead + Application.streamingAssetsPath + filePath);

						WWW data = MyResourceManager.Instance.LoadAssetsUsingWWW (filePath);

						yield return data;

						//判断www访问的数据是否发生了错误
						if (!String.IsNullOrEmpty (data.error)) {
							//打印错误的信息
							Debug.Log (data.error);

						} else {
							FileStream originFile = File.Create (Application.persistentDataPath + filePath);
							originFile.Write (data.bytes, 0, data.bytes.Length);
							originFile.Flush ();
							originFile.Close ();

						}

						Debug.Log ("地图完成" + j);

						if (data.isDone) {
							data.Dispose ();
						}
					}
				} else if (fileName.Equals ("NPC")) {
					//执行level的循环操作
					for (int j = 0; j < 1; j++) {
						filePath = "/Data/NPCs/NPC_" + j + ".json";
						WWW data = MyResourceManager.Instance.LoadAssetsUsingWWW (filePath);

						yield return data;

						//判断www访问的数据是否发生了错误
						if (!String.IsNullOrEmpty (data.error)) {
							//打印错误的信息
							Debug.Log (data.error);

						} else {
							FileStream originFile = File.Create (Application.persistentDataPath + filePath);
							originFile.Write (data.bytes, 0, data.bytes.Length);
							originFile.Flush ();
							originFile.Close ();

						}

						Debug.Log ("NPC完成" + j);

						if (data.isDone) {
							data.Dispose ();
						}
					}
				}else{
					Debug.Log(Application.streamingAssetsPath + filePath);

					WWW data = MyResourceManager.Instance.LoadAssetsUsingWWW (filePath);

//					WWW data = new WWW(pathHead + Application.streamingAssetsPath + filePath);

					yield return data;

					//判断www访问的数据是否发生了错误
					if (!String.IsNullOrEmpty(data.error))
					{
						//打印错误的信息
						Debug.Log(data.error);
					}
					else
					{
						FileStream originFile = File.Create(Application.persistentDataPath + filePath);
						originFile.Write(data.bytes, 0, data.bytes.Length);
						originFile.Flush();
						originFile.Close();

					}

					Debug.Log(fileName + "完成了");
					Debug.Log(filePath);

					if (data.isDone)
					{
						data.Dispose();
					}
				}

			}
			//初始化数据
			StartCoroutine("InitData");
		}



	}
}
