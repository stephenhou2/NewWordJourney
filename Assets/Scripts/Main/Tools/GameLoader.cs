﻿using UnityEngine;
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;


namespace WordJourney
{

	public class CheckDataModel
    {

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


    /// <summary>
    /// 进入游戏初始数据加载类
    /// </summary>
	public class GameLoader : MonoBehaviour
	{
		
		public bool alwaysPersistData;

		public bool dataReady;

		//public bool indentifyAndroidDevice;

		void Awake()
		{
			Application.targetFrameRate = 30;
#if UNITY_EDITOR
			Debug.unityLogger.logEnabled = true;
			StringEncryption.isEncryptionOn = true;
#else
            Debug.unityLogger.logEnabled = false;
			StringEncryption.isEncryptionOn = true;         
#endif
			Debug.Log(CommonData.persistDataPath);

			dataReady = false;

		}

		void Start()
		{

			IEnumerator waitCoroutine = WaitGamemanagerReady();

			StartCoroutine(waitCoroutine);
      
		}

		private IEnumerator WaitGamemanagerReady(){

			yield return new WaitUntil(() => TransformManager.FindTransform("GameManager") != null);

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.updateDataCanvasBundleName, "UpdateDataCanvas", delegate
            {
                TransformManager.FindTransform("UpdateDataCanvas").GetComponent<UpdateDataViewController>().SetUpUpdateDataView();

            });
                     
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
			Debug.Log("data ready");

		}


		/// <summary>
		/// 初始化游戏基础数据
		/// </summary>
		private void LoadDatas()
		{
                 
			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData();

			Player.mainPlayer.SetUpPlayerWithPlayerData(playerData);


		}

        /// <summary>
        /// 版本更新时数据处理
        /// </summary>
        /// <param name="checkData">Check data.</param>
        /// <param name="sql">Sql.</param>
		private void OnVersionUpdate(CheckDataModel checkData,MySQLiteHelper sql){
        
            //更新原始数据
			if(checkData.chatRecords != null && checkData.chatRecords.Count > 0){
				DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(checkData.chatRecords, CommonData.chatRecordsFilePath);
            }
			if(checkData.mapEventsRecords != null && checkData.mapEventsRecords.Count > 0){
				DataHandler.SaveInstanceListToFile<MapEventsRecord>(checkData.mapEventsRecords, CommonData.mapEventsRecordFilePath);
            }
			if(checkData.gameSettings != null){
				DataHandler.SaveInstanceDataToFile<GameSettings>(checkData.gameSettings, CommonData.gameSettingsDataFilePath);
            }
			if(checkData.miniMapRecord != null){
				DataHandler.SaveInstanceDataToFile<MiniMapRecord>(checkData.miniMapRecord, CommonData.miniMapRecordFilePath);
            }
			if(checkData.currentMapEventsRecord != null){
				DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(checkData.currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);  
            }
			if(checkData.playRecords != null){
				DataHandler.SaveInstanceListToFile<PlayRecord>(checkData.playRecords, CommonData.playRecordsFilePath);
            }
			if (checkData.buyRecord != null)
            {
                DataHandler.SaveInstanceDataToFile<BuyRecord>(checkData.buyRecord, CommonData.buyRecordFilePath, true);
            }

            sql.GetConnectionWith(CommonData.dataBaseName);
            sql.BeginTransaction();

			if (checkData.learnedWordsInSimple.Count > 0){
				Debug.Log(checkData.learnedWordsInSimple.Count);
				UpdateWordsDataBase(sql, 0, checkData.learnedWordsInSimple);
			}

			if(checkData.learnedWordsInMedium.Count > 0){
				UpdateWordsDataBase(sql, 1, checkData.learnedWordsInMedium);
			}

			if(checkData.learnedWordsInMaster.Count > 0){
				UpdateWordsDataBase(sql, 2, checkData.learnedWordsInMaster);
			}


            sql.EndTransaction();

            sql.CloseConnection(CommonData.dataBaseName);

			WordType wordType = WordType.Simple;

			if(checkData.gameSettings != null){
				wordType = checkData.gameSettings.wordType;
            }

            // 更新版本信息
			ApplicationInfo.Instance.currentVersion = GameManager.Instance.currentVersion;

			DataHandler.SaveInstanceDataToFile<ApplicationInfo>(ApplicationInfo.Instance,CommonData.applicationInfoFilePath);

			if(checkData.playerData == null){
				GameManager.Instance.persistDataManager.SaveCompletePlayerData();
				return;
			}

			if(checkData.playerData.currentExploreStartDateString == null 
			   || checkData.playerData.currentExploreStartDateString == string.Empty){
				
				string dateString = DateTime.Now.ToShortDateString();

                Player.mainPlayer.currentExploreStartDateString = dateString;

                checkData.playerData.currentExploreStartDateString = dateString;
			}

            // 更新单词数据
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

			if(checkData.playerData.titleQualifications != null){
				
				bool hasOldVersionTitle = false;
				
				for (int i = 0; i < checkData.playerData.titleQualifications.Length;i++){
					if(checkData.playerData.titleQualifications[i]){
						hasOldVersionTitle = true;
						break;
					}
				}

				if(checkData.playerData.titleQualificationsOfSimple == null || checkData.playerData.titleQualificationsOfSimple.Length == 0){
					checkData.playerData.titleQualificationsOfSimple = new bool[] { false, false, false, false, false, false };
				}

				if(checkData.playerData.titleQualificationsOfMedium == null || checkData.playerData.titleQualificationsOfMedium.Length == 0){
					checkData.playerData.titleQualificationsOfMedium = new bool[] { false, false, false, false, false, false };               
				}

				if(checkData.playerData.titleQualificationsOfMaster == null || checkData.playerData.titleQualificationsOfMaster.Length == 0){
					checkData.playerData.titleQualificationsOfMaster = new bool[] { false, false, false, false, false, false };
				}

				Player.mainPlayer.titleQualificationsOfSimple = checkData.playerData.titleQualificationsOfSimple;
				Player.mainPlayer.titleQualificationsOfMedium = checkData.playerData.titleQualificationsOfMedium;
				Player.mainPlayer.titleQualificationsOfMaster = checkData.playerData.titleQualificationsOfMaster;
                 
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
            }         
           
			DataHandler.SaveInstanceDataToFile<PlayerData>(checkData.playerData, CommonData.playerDataFilePath,true);         
		}




        /// <summary>
        /// 没有旧版本时的安装逻辑
        /// </summary>
		private void OnNewInstall(){
         
			GameSettings gameSettings = GameManager.Instance.gameDataCenter.gameSettings;

            string dateString = DateTime.Now.ToShortDateString();

            gameSettings.installDateString = dateString;

            GameManager.Instance.persistDataManager.SaveGameSettings();

            Player.mainPlayer.currentExploreStartDateString = dateString;

			PlayerData playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.playerDataFilePath);

            playerData.currentExploreStartDateString = dateString;

			ApplicationInfo.Instance.currentVersion = GameManager.Instance.currentVersion;

            DataHandler.SaveInstanceDataToFile<ApplicationInfo>(ApplicationInfo.Instance, CommonData.applicationInfoFilePath);

			DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath,true);

		}

        /// <summary>
        /// 检查数据是否完整
        /// </summary>
        /// <returns><c>true</c>, if data validate was checked, <c>false</c> otherwise.</returns>
		private bool CheckDataValidate()
        {
			string playerDataPath = CommonData.originDataPath + "PlayerData.json";
            string oriPlayerData = DataHandler.LoadDataString(playerDataPath);
			bool playerDataValidate = StringEncryption.Validate(oriPlayerData);

            string buyRecordDataPath = CommonData.originDataPath + "/BuyRecord.json";
            string oriBuyRecordData = DataHandler.LoadDataString(buyRecordDataPath);
			bool buyRecordValidate = StringEncryption.Validate(oriBuyRecordData);

			return playerDataValidate && buyRecordValidate;
        }


        /// <summary>
        /// 持久化数据
        /// </summary>
		public void PersistData()
		{         
			DirectoryInfo persistDi = new DirectoryInfo(CommonData.persistDataPath);

			IEnumerator initDataCoroutine = null;

			CheckDataModel checkData = new CheckDataModel();
         
			MySQLiteHelper sql = MySQLiteHelper.Instance;

            // 检查数据的完整性
			bool dataComplete = CheckDataComplete();
         
            // 如果原始玩家数据存在的话
			if (File.Exists(CommonData.playerDataFilePath))
			{
				// 记录原始玩家数据
				checkData.playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.playerDataFilePath);
				checkData.playerData.needChooseDifficulty = false;

				if(ApplicationInfo.Instance != null){
					checkData.versionUpdate = ApplicationInfo.Instance.currentVersion + 0.001f < GameManager.Instance.currentVersion;
				}else{
					checkData.versionUpdate = true;
				}
			}
            
			if (checkData.versionUpdate)
			{
				if(File.Exists(CommonData.dataBaseFilePath)){
					
					sql.GetConnectionWith(CommonData.dataBaseName);
					
					sql.BeginTransaction();
					
					checkData.learnedWordsInSimple = GetWordRecordsInDataBase(sql, 0);
					checkData.learnedWordsInMedium = GetWordRecordsInDataBase(sql, 1);
					checkData.learnedWordsInMaster = GetWordRecordsInDataBase(sql, 2);
					
					sql.EndTransaction();
					sql.CloseConnection(CommonData.dataBaseName);
                }

				checkData.playerData.isNewPlayer = true;  

				if(File.Exists(CommonData.buyRecordFilePath)){
					checkData.buyRecord = BuyRecord.Instance;
                }

				if(File.Exists(CommonData.chatRecordsFilePath)){
					checkData.chatRecords = GameManager.Instance.gameDataCenter.chatRecords;
                }

				if(File.Exists(CommonData.mapEventsRecordFilePath)){
					checkData.mapEventsRecords = GameManager.Instance.gameDataCenter.mapEventsRecords;
                }

				if(File.Exists(CommonData.gameSettingsDataFilePath)){
					checkData.gameSettings = GameManager.Instance.gameDataCenter.gameSettings;
                }

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
                     
			if (!persistDi.Exists || checkData.versionUpdate || !dataComplete)
			{

				bool dataValidate = CheckDataValidate();

				if(!dataValidate){
					return;
				}

			    GameManager.Instance.persistDataManager.BackUpDataWhenUpdataVersion(checkData);

				DataHandler.CopyDirectory(CommonData.originDataPath, CommonData.persistDataPath, true);

				if (!persistDi.Exists)
                {
                    OnNewInstall();
                }

                else if (checkData.versionUpdate || !dataComplete)
                {
			        GameManager.Instance.persistDataManager.versionUpdateWhenLoad = checkData.versionUpdate;

                    OnVersionUpdate(checkData, sql);
                }

				initDataCoroutine = InitData();

				StartCoroutine(initDataCoroutine);

				return;
			}
			else if (alwaysPersistData)
			{
				bool dataValidate = CheckDataValidate();

                if (!dataValidate)
                {
                    return;
                }

			    GameManager.Instance.persistDataManager.BackUpDataWhenUpdataVersion(checkData);

				DataHandler.CopyDirectory(CommonData.originDataPath, CommonData.persistDataPath, true);

				if (!persistDi.Exists)
                {
                    OnNewInstall();
                }

				else if (checkData.versionUpdate || !dataComplete)
                {      
					GameManager.Instance.persistDataManager.versionUpdateWhenLoad = checkData.versionUpdate;

					OnVersionUpdate(checkData,sql);               
				}
			}

			initDataCoroutine = InitData();

			StartCoroutine(initDataCoroutine);


#elif UNITY_ANDROID


			if (!persistDi.Exists || checkData.versionUpdate || !dataComplete)
            {
    			bool dataValidate = CheckDataValidate();

                if(!dataValidate){
                    return;
                }

				GameManager.Instance.persistDataManager.BackUpDataWhenUpdataVersion(checkData);
            
    			IEnumerator copyDataCoroutine = CopyDataForPersist(delegate{
                
					if(!persistDi.Exists){
						OnNewInstall();
					}
					else if (checkData.versionUpdate || !dataComplete)
                    {         
			            GameManager.Instance.persistDataManager.versionUpdateWhenLoad = checkData.versionUpdate ;			
            			OnVersionUpdate(checkData,sql);
    			    }                   
    			});
			    StartCoroutine(copyDataCoroutine);
			    return;
			}
			else if (alwaysPersistData)
            {
			    bool dataValidate = CheckDataValidate();

                if(!dataValidate){
                    return;
                }

                if(DataHandler.DirectoryExist(CommonData.persistDataPath + "/Data")){
                    DataHandler.DeleteDirectory(CommonData.persistDataPath + "/Data");
                }

				GameManager.Instance.persistDataManager.BackUpDataWhenUpdataVersion(checkData);

                IEnumerator copyDataCoroutine = CopyDataForPersist(delegate{
                
					if(!persistDi.Exists){
						OnNewInstall();
					}
					else if (checkData.versionUpdate || !dataComplete)
                    {
						GameManager.Instance.persistDataManager.versionUpdateWhenLoad = checkData.versionUpdate;
            			OnVersionUpdate(checkData,sql);         
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


        /// <summary>
        /// 检查数据完整性
        /// </summary>
        /// <returns><c>true</c>, if data complete was checked, <c>false</c> otherwise.</returns>
		private bool CheckDataComplete(){
        

			//循环文件
            for (int i = 0; i < CommonData.originDataArr.Length; i++)
            {

                //获取数组中的文件字典数据
                KVPair originDataKV = CommonData.originDataArr[i];

                string fileName = originDataKV.key;
				string filePath = Application.persistentDataPath + originDataKV.value;

                if (fileName.Equals("Level"))
                {
                    //执行level的循环操作
                    for (int j = 0; j < 51; j++)
                    {
						filePath = Application.persistentDataPath + "/Data/MapData/Level_" + j + ".json";

						if(!File.Exists(filePath)){
							return false;
						}

                    }
                }
                else if (fileName.Equals("NPC"))
                {
                    //执行npc的循环操作
                    for (int j = 0; j < 13; j++)
                    {
						filePath = Application.persistentDataPath + "/Data/NPCs/NPC_" + j + ".json";
                       
						if(!File.Exists(filePath)){
							return false;
						}
                    }
                }
                else
                {
					if(!File.Exists(filePath)){
						return false;
					}
                }

            }

			return true;


		}


	}
}
