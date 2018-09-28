using UnityEngine;
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;


namespace WordJourney
{
	[System.Serializable]
	public class QualifiedUserInfo
    {
		public string nickName;
		public string identifier;
		public string uniqueDeviceId;
		public string deviceType;

    }

	public class GameLoader : MonoBehaviour
	{
		private class CheckDataModel{

			public PlayerData playerData;
			public BuyRecord buyRecord;
			public List<HLHNPCChatRecord> chatRecords;
			public List<MapEventsRecord> mapEventsRecords;
			public GameSettings gameSettings;
			public MiniMapRecord miniMapRecord;
			public CurrentMapEventsRecord currentMapEventsRecord;
			public List<PlayRecord> playRecords;
			public bool versionUpdate = false;

			public List<HLHWord> learnedWordsInSimple = new List<HLHWord>();
			public List<HLHWord> learnedWordsInMedium = new List<HLHWord>();
			public List<HLHWord> learnedWordsInMaster = new List<HLHWord>();

		}

		public bool alwaysPersistData;

		public bool dataReady;

		public bool indentifyAndroidDevice;

		void Awake()
		{
			Application.targetFrameRate = 30;
#if UNITY_EDITOR
			Debug.unityLogger.logEnabled = true;
			StringEncryption.isEncryptionOn = false;
#else
            Debug.unityLogger.logEnabled = true;
			StringEncryption.isEncryptionOn = true;         
#endif
			Debug.Log(CommonData.persistDataPath);

			dataReady = false;

		}

		void Start()
		{
#if UNITY_ANDROID
			if(indentifyAndroidDevice && !MyTool.DistinguishTestDevice()){
				Application.Quit();
			}
#endif
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


		private void OnVersionUpdate(CheckDataModel checkData,MySQLiteHelper sql){
         
            DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(checkData.chatRecords, CommonData.chatRecordsFilePath);
            DataHandler.SaveInstanceListToFile<MapEventsRecord>(checkData.mapEventsRecords, CommonData.mapEventsRecordFilePath);
            DataHandler.SaveInstanceDataToFile<GameSettings>(checkData.gameSettings, CommonData.gameSettingsDataFilePath);
			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(checkData.miniMapRecord, CommonData.miniMapRecordFilePath);
            DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(checkData.currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
			DataHandler.SaveInstanceListToFile<PlayRecord>(checkData.playRecords, CommonData.playRecordsFilePath);


            sql.GetConnectionWith(CommonData.dataBaseName);
            sql.BeginTransaction();

			UpdateWordsDataBase(sql, 0, checkData.learnedWordsInSimple);
			UpdateWordsDataBase(sql, 1, checkData.learnedWordsInMedium);
			UpdateWordsDataBase(sql, 2, checkData.learnedWordsInMaster);

            sql.EndTransaction();

            sql.CloseConnection(CommonData.dataBaseName);

			WordType wordType = checkData.gameSettings.wordType;

			if(checkData.playerData.currentExploreStartDateString == null 
			   || checkData.playerData.currentExploreStartDateString == string.Empty){
				
				string dateString = DateTime.Now.ToShortDateString();

                Player.mainPlayer.currentExploreStartDateString = dateString;

                checkData.playerData.currentExploreStartDateString = dateString;
			}

			if(checkData.playerData.maxWordContinuousRightRecord > 0){
				switch(wordType){
					case WordType.Simple:
						Player.mainPlayer.maxSimpleWordContinuousRightRecord = checkData.playerData.maxWordContinuousRightRecord;
						checkData.playerData.maxSimpleWordContinuousRightRecord = checkData.playerData.maxWordContinuousRightRecord;
						break;
					case WordType.Medium:
						Player.mainPlayer.maxMediumWordContinuousRightRecord = checkData.playerData.maxWordContinuousRightRecord;
						checkData.playerData.maxMediumWordContinuousRightRecord = checkData.playerData.maxWordContinuousRightRecord;
						break;
					case WordType.Master:
						Player.mainPlayer.maxMasterWordContinuousRightRecord = checkData.playerData.maxWordContinuousRightRecord;
						checkData.playerData.maxMasterWordContinuousRightRecord = checkData.playerData.maxWordContinuousRightRecord;
						break;
				}
				checkData.playerData.maxWordContinuousRightRecord = 0;
			}

			if(checkData.playerData.wordContinuousRightRecord > 0){
				switch (wordType)
                {
                    case WordType.Simple:
						Player.mainPlayer.simpleWordContinuousRightRecord = checkData.playerData.wordContinuousRightRecord;
						checkData.playerData.simpleWordContinuousRightRecord = checkData.playerData.wordContinuousRightRecord;
                        break;
                    case WordType.Medium:
						Player.mainPlayer.mediumWordContinuousRightRecord = checkData.playerData.wordContinuousRightRecord;
						checkData.playerData.mediumWordContinuousRightRecord = checkData.playerData.wordContinuousRightRecord;
                        break;
                    case WordType.Master:
						Player.mainPlayer.masterWordContinuousRightRecord = checkData.playerData.wordContinuousRightRecord;
						checkData.playerData.masterWordContinuousRightRecord = checkData.playerData.wordContinuousRightRecord;
                        break;
                }
				checkData.playerData.wordContinuousRightRecord = 0;
			}

			bool hasOldVersionTitle = false;

			for (int i = 0; i < checkData.playerData.titleQualifications.Length;i++){
				if(checkData.playerData.titleQualifications[i]){
					hasOldVersionTitle = true;
					break;
				}
			}

			if(hasOldVersionTitle){
				switch (wordType)
                {
                    case WordType.Simple:
						Player.mainPlayer.titleQualificationsOfSimple = checkData.playerData.titleQualifications;
						checkData.playerData.titleQualificationsOfSimple = checkData.playerData.titleQualifications;
                        break;
                    case WordType.Medium:
						Player.mainPlayer.titleQualificationsOfMedium = checkData.playerData.titleQualifications;
						checkData.playerData.titleQualificationsOfMedium = checkData.playerData.titleQualifications;
                        break;
                    case WordType.Master:
						Player.mainPlayer.titleQualificationsOfMaster = checkData.playerData.titleQualifications;
						checkData.playerData.titleQualificationsOfMaster = checkData.playerData.titleQualifications;
                        break;
                }
				checkData.playerData.titleQualifications = new bool[]{false,false,false,false,false,false};
			}
                    

			Player.mainPlayer.currentVersion = GameManager.Instance.currentVersion;
			checkData.playerData.currentVersion = GameManager.Instance.currentVersion;
           
			DataHandler.SaveInstanceDataToFile<PlayerData>(checkData.playerData, CommonData.playerDataFilePath,true);
			DataHandler.SaveInstanceDataToFile<BuyRecord>(checkData.buyRecord, CommonData.buyRecordFilePath,true);
         
		}

		private void OnNewInstall(){

			GameSettings gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

            string dateString = DateTime.Now.ToShortDateString();

            gameSettings.installDateString = dateString;

            GameManager.Instance.persistDataManager.SaveGameSettings();

            Player.mainPlayer.currentExploreStartDateString = dateString;

			PlayerData playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.oriPlayerDataFilePath);

            playerData.currentExploreStartDateString = dateString;

			Player.mainPlayer.currentVersion = GameManager.Instance.currentVersion;
			playerData.currentVersion = GameManager.Instance.currentVersion;

			DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath,true);

		}


		public void PersistData()
		{         
			DirectoryInfo persistDi = new DirectoryInfo(CommonData.persistDataPath);

			IEnumerator initDataCoroutine = null;

			CheckDataModel checkData = new CheckDataModel();
         
			MySQLiteHelper sql = MySQLiteHelper.Instance;
         
			if (persistDi.Exists)
			{
				checkData.playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.playerDataFilePath);
				checkData.playerData.needChooseDifficulty = checkData.playerData.isNewPlayer;
				checkData.versionUpdate = checkData.playerData.currentVersion + 0.001f < GameManager.Instance.currentVersion;
			}

			if (checkData.versionUpdate)
			{
				sql.GetConnectionWith(CommonData.dataBaseName);

				sql.BeginTransaction();

				checkData.learnedWordsInSimple = GetWordRecordsInDataBase(sql, 0);
				checkData.learnedWordsInMedium = GetWordRecordsInDataBase(sql, 1);
				checkData.learnedWordsInMaster = GetWordRecordsInDataBase(sql, 2);

				sql.EndTransaction();
				sql.CloseConnection(CommonData.dataBaseName);

				checkData.playerData.currentVersion = GameManager.Instance.currentVersion;

				checkData.playerData.isNewPlayer = true;            
				checkData.buyRecord = BuyRecord.Instance;
				checkData.chatRecords = GameManager.Instance.gameDataCenter.chatRecords;
				checkData.mapEventsRecords = GameManager.Instance.gameDataCenter.mapEventsRecords;
				checkData.gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

				if(File.Exists(CommonData.miniMapRecordFilePath)){
					checkData.miniMapRecord = GameManager.Instance.gameDataCenter.currentMapMiniMapRecord;
				}
				if(File.Exists(CommonData.currentMapEventsRecordFilePath)){
					checkData.currentMapEventsRecord = GameManager.Instance.gameDataCenter.currentMapEventsRecord;
				}
				if(File.Exists(CommonData.playRecordsFilePath)){
					checkData.playRecords = GameManager.Instance.gameDataCenter.allPlayRecords;
				}

			}


#if UNITY_IOS

			if (!persistDi.Exists || checkData.versionUpdate)
			{

				DataHandler.CopyDirectory(CommonData.originDataPath, CommonData.persistDataPath, true);

				if (checkData.versionUpdate)
				{
					GameManager.Instance.persistDataManager.versionUpdateWhenLoad = true;

					OnVersionUpdate(checkData, sql);               
				}
				else
				{               
					OnNewInstall();               
				}

				initDataCoroutine = InitData();

				StartCoroutine(initDataCoroutine);

				return;
			}
			else if (alwaysPersistData)
			{
				DataHandler.CopyDirectory(CommonData.originDataPath, CommonData.persistDataPath, true);

				if (checkData.versionUpdate)
				{      
					GameManager.Instance.persistDataManager.versionUpdateWhenLoad = true;

					OnVersionUpdate(checkData,sql);               
				}
				else
				{               
					OnNewInstall();
				}
			}

			initDataCoroutine = InitData();

			StartCoroutine(initDataCoroutine);


#elif UNITY_ANDROID


			if (!persistDi.Exists || checkData.versionUpdate)
            {
    			IEnumerator copyDataCoroutine = CopyDataForPersist(delegate{
                
    			    if (checkData.versionUpdate)
                    {         
						OnVersionUpdate(checkData,sql);

    			    }else{         
						OnNewInstall();
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
                
                    if (checkData.versionUpdate)
                    {
						OnVersionUpdate(checkData,sql);

                    }else{
						OnNewInstall();
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
