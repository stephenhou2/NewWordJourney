using UnityEngine;
using System;
using System.IO;
using System.Collections;


namespace WordJourney
{

	public class GameLoader : MonoBehaviour {

		public bool alwaysPersistData;

		void Awake(){
			Application.targetFrameRate = 30;
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
			Debug.Log (CommonData.persistDataPath);
            
            

		}

		void Start(){
			PersistData();
		}

		private IEnumerator InitData(){

			yield return new WaitUntil(()=> MyResourceManager.Instance.isManifestReady);
                     
			LoadDatas ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

				GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.homeBgmName);

			});

			//CheckItemSprites();
				
		}


		/// <summary>
		/// 初始化游戏基础数据
		/// </summary>
		private void LoadDatas(){
         
			//GameManager.Instance.gameDataCenter.InitPersistentGameData ();

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);


		}
              

		private void PersistData(){

         
			DirectoryInfo persistDi = new DirectoryInfo (CommonData.persistDataPath);

#if UNITY_EDITOR || UNITY_IOS
                     
			if (!persistDi.Exists) {
				
				DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);
            
                GameSettings gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

                string dateString = DateTime.Now.ToShortDateString();

                gameSettings.installDateString = dateString;

                GameManager.Instance.persistDataManager.SaveGameSettings();


				StartCoroutine ("InitData");
				return;
			}

			if (alwaysPersistData) {
				DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);

				GameSettings gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

                string dateString = DateTime.Now.ToShortDateString();

                gameSettings.installDateString = dateString;

                GameManager.Instance.persistDataManager.SaveGameSettings();
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
    			if(DataHandler.DirectoryExist(CommonData.persistDataPath + "/Data")){
			        DataHandler.DeleteDirectory(CommonData.persistDataPath + "/Data");
                }
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
			Directory.CreateDirectory(Application.persistentDataPath + "/Data/GameItems");

			//循环拷贝文件
			for (int i = 0; i < CommonData.originDataArr.Length; i++)
			{

				//获取数组中的文件字典数据
				KVPair originDataKV = CommonData.originDataArr[i];

				string fileName = originDataKV.key;
				string filePath = originDataKV.value;

				if (fileName.Equals ("Level")) {
					//执行level的循环操作
					for (int j = 0; j < 51; j++) {
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
					//执行npc的循环操作
					for (int j = 0; j < 13; j++) {
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

			GameSettings gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

            string dateString = DateTime.Now.ToShortDateString();

            gameSettings.installDateString = dateString;

            GameManager.Instance.persistDataManager.SaveGameSettings();

			//DataHandler.DeleteFile(CommonData.persistDataPath + "/PlayerData.json");

			//初始化数据
			StartCoroutine("InitData");
		}


        
		//private void CheckItemSprites(){


		//	Debug.Log("CHECK ITEM SPRITE NAME");         
		//	Sprite sprite = null;
		//	for (int i = 0; i < GameManager.Instance.gameDataCenter.allEquipmentModels.Count;i++){

		//		EquipmentModel equipmentModel = GameManager.Instance.gameDataCenter.allEquipmentModels[i];

		//		sprite = GameManager.Instance.gameDataCenter.allEquipmentSprites.Find(delegate (Sprite obj)
		//		{
		//			return obj.name == equipmentModel.spriteName;
		//		});

		//		if (sprite == null){
		//			Debug.Log(equipmentModel.spriteName);
		//		}

		//	}

		//	for (int i = 0; i < GameManager.Instance.gameDataCenter.allConsumablesModels.Count; i++)
  //          {

		//		ConsumablesModel consumablesModel = GameManager.Instance.gameDataCenter.allConsumablesModels[i];

		//		sprite = GameManager.Instance.gameDataCenter.allConsumablesSprites.Find(delegate (Sprite obj)
  //              {
		//			return obj.name == consumablesModel.spriteName;
  //              });

  //              if (sprite == null)
  //              {
		//			Debug.Log(consumablesModel.spriteName);
  //              }

  //          }

		//	for (int i = 0; i < GameManager.Instance.gameDataCenter.allSpecialItemModels.Count; i++)
  //          {

		//		SpecialItemModel specialItemModel = GameManager.Instance.gameDataCenter.allSpecialItemModels[i];

		//		sprite = GameManager.Instance.gameDataCenter.allSpecialItemSprites.Find(delegate (Sprite obj)
  //              {
		//			return obj.name == specialItemModel.spriteName;
  //              });

  //              if (sprite == null)
  //              {
		//			Debug.Log(specialItemModel.spriteName);
  //              }

  //          }

		//	for (int i = 0; i < GameManager.Instance.gameDataCenter.allPropertyGemstoneModels.Count; i++)
  //          {

		//		PropertyGemstoneModel propertyGemstoneModel = GameManager.Instance.gameDataCenter.allPropertyGemstoneModels[i];

		//		sprite = GameManager.Instance.gameDataCenter.allPropertyGemstoneSprites.Find(delegate (Sprite obj)
  //              {
		//			return obj.name == propertyGemstoneModel.spriteName;
  //              });

  //              if (sprite == null)
  //              {
		//			Debug.Log(propertyGemstoneModel.spriteName);
  //              }

			
		//	}

		//	for (int i = 0; i < GameManager.Instance.gameDataCenter.allSkillScrollModels.Count; i++)
  //          {

		//		SkillScrollModel skillScrollModel = GameManager.Instance.gameDataCenter.allSkillScrollModels[i];

		//		sprite = GameManager.Instance.gameDataCenter.allSkillScrollSprites.Find(delegate (Sprite obj)
  //              {
		//			return obj.name == skillScrollModel.spriteName;
  //              });

  //              if (sprite == null)
  //              {
		//			Debug.Log(skillScrollModel.spriteName);
  //              }

  //          }
            


		//}

	}
}
