using UnityEngine;
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;


namespace WordJourney
{

	public class GameLoader : MonoBehaviour
	{
		private class CheckDataModel{

			public PlayerData playerData;
			public BuyRecord buyRecord;
			public List<HLHNPCChatRecord> chatRecords;
			public List<MapEventsRecord> mapEventsRecords;
			public GameSettings gameSettings;
			public List<MiniMapRecord> miniMapRecords;
			public CurrentMapEventsRecord currentMapEventsRecord;
			public bool versionUpdate = false;

		}

		public bool alwaysPersistData;

		public bool dataReady;

		void Awake()
		{
			Application.targetFrameRate = 30;
#if UNITY_EDITOR
			Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
			Debug.Log(CommonData.persistDataPath);

			dataReady = false;

		}

		void Start()
		{
			PersistData();
		}

		private IEnumerator InitData()
		{

			yield return new WaitUntil(() => MyResourceManager.Instance.isManifestReady);

			LoadDatas();

			if (BuyRecord.Instance.bag_4_unlocked)
			{
				GameManager.Instance.purchaseManager.buyedGoodsChange.Add("Bag_4");
				BuyRecord.Instance.bag_4_unlocked = false;
				GameManager.Instance.persistDataManager.SaveBuyRecord();
				Player.mainPlayer.totalGold += 500;
				GameManager.Instance.persistDataManager.UpdateBuyGoldToPlayerDataFile();
			}

			dataReady = true;

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName, "HomeCanvas", () =>
			{

				HomeViewController homeViewController = TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>();

				homeViewController.SetUpHomeView();
            
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					homeViewController.homeView.ShowNoAvalableNetHintHUD();
				}
				else
				{
					homeViewController.homeView.HideNoAvalableNetHintHUD();
				}

				GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.homeBgmName);

			});

			//CheckItemSprites();

		}


		/// <summary>
		/// 初始化游戏基础数据
		/// </summary>
		private void LoadDatas()
		{

			//GameManager.Instance.gameDataCenter.InitPersistentGameData ();

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData();

			Player.mainPlayer.SetUpPlayerWithPlayerData(playerData);


		}




		public void PersistData()
		{


			DirectoryInfo persistDi = new DirectoryInfo(CommonData.persistDataPath);

			IEnumerator initDataCoroutine = null;

			CheckDataModel checkData = new CheckDataModel();

			PlayerData playerData = null;
			BuyRecord buyRecord = null;
			List<HLHNPCChatRecord> chatRecords = null;
			List<MapEventsRecord> mapEventsRecords = null;
			GameSettings gameSettings = null;
			List<MiniMapRecord> miniMapRecords = null;
			CurrentMapEventsRecord currentMapEventsRecord = null;
			bool versionUpdate = false;

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			List<HLHWord> learnedWordsInSimple = new List<HLHWord>();
			List<HLHWord> learnedWordsInMedium = new List<HLHWord>();
			List<HLHWord> learnedWordsInMaster = new List<HLHWord>();

			if (persistDi.Exists)
			{
				playerData = GameManager.Instance.persistDataManager.LoadPlayerData();
                playerData.needChooseDifficulty = playerData.isNewPlayer;
                versionUpdate = playerData.currentVersion + 0.001f < GameManager.Instance.currentVersion;
			}

			if (versionUpdate)
			{
				sql.GetConnectionWith(CommonData.dataBaseName);

				sql.BeginTransaction();

				learnedWordsInSimple = GetWordRecordsInDataBase(sql, 0);
				learnedWordsInMedium = GetWordRecordsInDataBase(sql, 1);
				learnedWordsInMaster = GetWordRecordsInDataBase(sql, 2);

				sql.EndTransaction();
				sql.CloseConnection(CommonData.dataBaseName);

				playerData.currentVersion = GameManager.Instance.currentVersion;

				playerData.isNewPlayer = true;            
				buyRecord = BuyRecord.Instance;
				chatRecords = GameManager.Instance.gameDataCenter.chatRecords;
				mapEventsRecords = GameManager.Instance.gameDataCenter.mapEventsRecords;
				gameSettings = GameManager.Instance.gameDataCenter.gameSettings;
				if(File.Exists(CommonData.miniMapRecordsFilePath)){
					miniMapRecords = GameManager.Instance.gameDataCenter.allMiniMapRecords;
				}
				if(File.Exists(CommonData.currentMapEventsRecordFilePath)){
					currentMapEventsRecord = GameManager.Instance.gameDataCenter.currentMapEventsRecord;
				}

			}


#if UNITY_EDITOR || UNITY_IOS

			if (!persistDi.Exists || versionUpdate)
			{

				DataHandler.CopyDirectory(CommonData.originDataPath, CommonData.persistDataPath, true);

				if (versionUpdate)
				{
					DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
					DataHandler.SaveInstanceDataToFile<BuyRecord>(buyRecord, CommonData.buyRecordFilePath);
					DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(chatRecords, CommonData.chatRecordsFilePath);
					DataHandler.SaveInstanceListToFile<MapEventsRecord>(mapEventsRecords, CommonData.mapEventsRecordFilePath);
					DataHandler.SaveInstanceDataToFile<GameSettings>(gameSettings, CommonData.gameSettingsDataFilePath);

					DataHandler.SaveInstanceListToFile<MiniMapRecord>(miniMapRecords, CommonData.miniMapRecordsFilePath);
					DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
                                   
					sql.GetConnectionWith(CommonData.dataBaseName);
					sql.BeginTransaction();

					UpdateWordsDataBase(sql, 0, learnedWordsInSimple);
					UpdateWordsDataBase(sql, 1, learnedWordsInMedium);
					UpdateWordsDataBase(sql, 2, learnedWordsInMaster);

					sql.EndTransaction();

					sql.CloseConnection(CommonData.dataBaseName);

					string dateString = DateTime.Now.ToShortDateString();

					Player.mainPlayer.currentExploreStartDateString = dateString;

					playerData.currentExploreStartDateString = dateString;

					DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);


				}
				else
				{

					gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

					string dateString = DateTime.Now.ToShortDateString();

					gameSettings.installDateString = dateString;

					GameManager.Instance.persistDataManager.SaveGameSettings();

					Player.mainPlayer.currentExploreStartDateString = dateString;

					//GameManager.Instance.persistDataManager.SaveCompletePlayerData();

					playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.oriPlayerDataFilePath);

					playerData.currentExploreStartDateString = dateString;

					DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);

				}

				initDataCoroutine = InitData();

				StartCoroutine(initDataCoroutine);

				return;
			}
			else if (alwaysPersistData)
			{
				DataHandler.CopyDirectory(CommonData.originDataPath, CommonData.persistDataPath, true);

				if (versionUpdate)
				{
					DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
					DataHandler.SaveInstanceDataToFile<BuyRecord>(buyRecord, CommonData.buyRecordFilePath);
					DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(chatRecords, CommonData.chatRecordsFilePath);
					DataHandler.SaveInstanceListToFile<MapEventsRecord>(mapEventsRecords, CommonData.mapEventsRecordFilePath);
					DataHandler.SaveInstanceDataToFile<GameSettings>(gameSettings, CommonData.gameSettingsDataFilePath);

					DataHandler.SaveInstanceListToFile<MiniMapRecord>(miniMapRecords, CommonData.miniMapRecordsFilePath);
                    DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
					//DataHandler.SaveInstanceDataToFile<>

					sql.GetConnectionWith(CommonData.dataBaseName);
					sql.BeginTransaction();

					UpdateWordsDataBase(sql, 0, learnedWordsInSimple);
					UpdateWordsDataBase(sql, 1, learnedWordsInMedium);
					UpdateWordsDataBase(sql, 2, learnedWordsInMaster);

					sql.EndTransaction();

					sql.CloseConnection(CommonData.dataBaseName);

					string dateString = DateTime.Now.ToShortDateString();

					Player.mainPlayer.currentExploreStartDateString = dateString;

					playerData.currentExploreStartDateString = dateString;

					DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);

				}
				else
				{

					gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

					string dateString = DateTime.Now.ToShortDateString();

					gameSettings.installDateString = dateString;

					GameManager.Instance.persistDataManager.SaveGameSettings();

					Player.mainPlayer.currentExploreStartDateString = dateString;

					playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.oriPlayerDataFilePath);

                    playerData.currentExploreStartDateString = dateString;

                    DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
                
				}
			}

			initDataCoroutine = InitData();

			StartCoroutine(initDataCoroutine);


#elif UNITY_ANDROID


			if (!persistDi.Exists)
			{
    			IEnumerator copyDataCoroutine = CopyDataForPersist(delegate{
                
    			    if (versionUpdate)
                    {


                        DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
                        DataHandler.SaveInstanceDataToFile<BuyRecord>(buyRecord, CommonData.buyRecordFilePath);
                        DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(chatRecords, CommonData.chatRecordsFilePath);
                        DataHandler.SaveInstanceListToFile<MapEventsRecord>(mapEventsRecords, CommonData.mapEventsRecordFilePath);
                        DataHandler.SaveInstanceDataToFile<GameSettings>(gameSettings, CommonData.gameSettingsDataFilePath);

    			        DataHandler.SaveInstanceListToFile<MiniMapRecord>(miniMapRecords, CommonData.miniMapRecordsFilePath);
                        DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);

    			        sql.GetConnectionWith(CommonData.dataBaseName);
                        sql.BeginTransaction();

    			        UpdateWordsDataBase(sql, 0, learnedWordsInSimple);
                        UpdateWordsDataBase(sql, 1, learnedWordsInMedium);
                        UpdateWordsDataBase(sql, 2, learnedWordsInMaster);

                        sql.EndTransaction();

                        sql.CloseConnection(CommonData.dataBaseName);

			            string dateString = DateTime.Now.ToShortDateString();

			            Player.mainPlayer.currentExploreStartDateString = dateString;

			            playerData.currentExploreStartDateString = dateString;

			            DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);

    			    }else{

                        gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

                        string dateString = DateTime.Now.ToShortDateString();

                        gameSettings.installDateString = dateString;

                        GameManager.Instance.persistDataManager.SaveGameSettings();  

			            Player.mainPlayer.currentExploreStartDateString = dateString;

			            playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.oriPlayerDataFilePath);

			            playerData.currentExploreStartDateString = dateString;

			            DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
                    }                   

    			});
			    StartCoroutine(copyDataCoroutine);
			    return;
			}
			else if (alwaysPersistData)
            {
                if(DataHandler.DirectoryExist(CommonData.persistDataPath + "/Data")){
                    DataHandler.DeleteDirectory(CommonData.persistDataPath + "/Data");
                }

                    IEnumerator copyDataCoroutine = CopyDataForPersist(delegate{
                
                    if (versionUpdate)
                    {
                        DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
                        DataHandler.SaveInstanceDataToFile<BuyRecord>(buyRecord, CommonData.buyRecordFilePath);
                        DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(chatRecords, CommonData.chatRecordsFilePath);
                        DataHandler.SaveInstanceListToFile<MapEventsRecord>(mapEventsRecords, CommonData.mapEventsRecordFilePath);
                        DataHandler.SaveInstanceDataToFile<GameSettings>(gameSettings, CommonData.gameSettingsDataFilePath);
			            DataHandler.SaveInstanceListToFile<MiniMapRecord>(miniMapRecords, CommonData.miniMapRecordsFilePath);
                        DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);

                        sql.GetConnectionWith(CommonData.dataBaseName);
                        sql.BeginTransaction();

                        UpdateWordsDataBase(sql, 0, learnedWordsInSimple);
                        UpdateWordsDataBase(sql, 1, learnedWordsInMedium);
                        UpdateWordsDataBase(sql, 2, learnedWordsInMaster);

                        sql.EndTransaction();

                        sql.CloseConnection(CommonData.dataBaseName);

                        string dateString = DateTime.Now.ToShortDateString();

                        Player.mainPlayer.currentExploreStartDateString = dateString;

			            playerData.currentExploreStartDateString = dateString;

                        DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);

                    }else{

                        gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

                        string dateString = DateTime.Now.ToShortDateString();

                        gameSettings.installDateString = dateString;

                        GameManager.Instance.persistDataManager.SaveGameSettings();
                           
                        Player.mainPlayer.currentExploreStartDateString = dateString;

			            playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.oriPlayerDataFilePath);

			            playerData.currentExploreStartDateString = dateString;

                        DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath);
                    }        
                  
                });
                StartCoroutine(copyDataCoroutine);
                
            }else{
                initDataCoroutine = InitData();

                StartCoroutine (initDataCoroutine);
            }

         
#endif

		}


		//安卓下使用www的方法拷贝到永久存在的文件夹
		IEnumerator CopyDataForPersist(CallBack copyFinishCallBack)
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

				if (fileName.Equals("Level"))
				{
					//执行level的循环操作
					for (int j = 0; j < 51; j++)
					{
						filePath = "/Data/MapData/Level_" + j + ".json";

						Debug.Log(Application.streamingAssetsPath + filePath);

						//						WWW data = new WWW(pathHead + Application.streamingAssetsPath + filePath);

						WWW data = MyResourceManager.Instance.LoadAssetsUsingWWW(filePath);

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

						Debug.Log("地图完成" + j);

						if (data.isDone)
						{
							data.Dispose();
						}
					}
				}
				else if (fileName.Equals("NPC"))
				{
					//执行npc的循环操作
					for (int j = 0; j < 13; j++)
					{
						filePath = "/Data/NPCs/NPC_" + j + ".json";
						WWW data = MyResourceManager.Instance.LoadAssetsUsingWWW(filePath);

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

						Debug.Log("NPC完成" + j);

						if (data.isDone)
						{
							data.Dispose();
						}
					}
				}
				else
				{
					Debug.Log(Application.streamingAssetsPath + filePath);

					WWW data = MyResourceManager.Instance.LoadAssetsUsingWWW(filePath);

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

			if (copyFinishCallBack != null)
			{
				copyFinishCallBack();
			}

			GameSettings gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

			string dateString = DateTime.Now.ToShortDateString();

			gameSettings.installDateString = dateString;

			GameManager.Instance.persistDataManager.SaveGameSettings();

			//DataHandler.DeleteFile(CommonData.persistDataPath + "/PlayerData.json");

			//初始化数据
			IEnumerator initDataCoroutine = InitData();
			StartCoroutine(initDataCoroutine);
		}

		/// <summary>
		/// 获取数据库中的单词数据[0:simple 1:middle 2:master]
		/// </summary>
		/// <returns>The word records in data base.</returns>
		/// <param name="wordType">Word type.</param>
		private List<HLHWord> GetWordRecordsInDataBase(MySQLiteHelper sql,int wordType)
		{
			List<HLHWord> wordsList = new List<HLHWord>();
         
			string query = string.Empty;

			switch(wordType){
				case 0:
					query = string.Format("SELECT * FROM {0} WHERE learnedTimes>0",CommonData.simpleWordsTable);
					break;
				case 1:
					query = string.Format("SELECT * FROM {0} WHERE learnedTimes>0",CommonData.mediumWordsTabel);;
					break;
				case 2:
					query = string.Format("SELECT * FROM {0} WHERE learnedTimes>0",CommonData.masterWordsTabel);
					break;
			}


			IDataReader reader = sql.ExecuteQuery(query);

			while (reader.Read())
			{

				int wordId = reader.GetInt32(0);

                string spell = reader.GetString(1);

                string phoneticSymble = reader.GetString(2);

                string explaination = reader.GetString(3);

                string sentenceEN = reader.GetString(4);

                string sentenceCH = reader.GetString(5);

                string pronounciationURL = reader.GetString(6);

                int wordLength = reader.GetInt16(7);

                int learnedTimes = reader.GetInt16(8);

                int ungraspTimes = reader.GetInt16(9);

                bool isFamiliar = reader.GetInt16(10) == 1;

				HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL,
										  wordLength, learnedTimes, ungraspTimes, isFamiliar, "");

				wordsList.Add(word);

			}
			return wordsList;
		}

        /// <summary>
        /// 更新数据库中的单词数据
        /// </summary>
        /// <param name="wordType">Word type.</param>
		private void UpdateWordsDataBase(MySQLiteHelper sql, int wordType,List<HLHWord> wordsList){

			string query = string.Empty;

			for (int i = 0; i < wordsList.Count;i++){

				HLHWord word = wordsList[i];

				int wordId = word.wordId;

				switch (wordType)
                {
                    case 0:
						query = string.Format("UPDATE {0} SET learnedTimes={1},ungraspTimes={2},isFamiliar={3} WHERE spell='{4}'", 
						                      CommonData.simpleWordsTable,word.learnedTimes,word.ungraspTimes,word.isFamiliar?1:0,word.spell);
                        break;
                    case 1:
						query = string.Format("UPDATE {0} SET learnedTimes={1},ungraspTimes={2},isFamiliar={3} WHERE spell='{4}'",
						                      CommonData.mediumWordsTabel, word.learnedTimes, word.ungraspTimes, word.isFamiliar ? 1 : 0, word.spell);
                        break;
                    case 2:
						query = string.Format("UPDATE {0} SET learnedTimes={1},ungraspTimes={2},isFamiliar={3} WHERE spell='{4}'",
						                      CommonData.masterWordsTabel, word.learnedTimes, word.ungraspTimes, word.isFamiliar ? 1 : 0, word.spell);
                        break;
                }

				sql.ExecuteQuery(query);
			}
         

		}


	}
}
