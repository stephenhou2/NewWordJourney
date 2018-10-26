using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;
	using System;

	public class DataBackUpWhenVersionUpdate{
		public PlayerData playerData;
        public BuyRecord buyRecord;
        public List<HLHNPCChatRecord> chatRecords;
        public List<MapEventsRecord> mapEventsRecords;
        public GameSettings gameSettings;
        public MiniMapRecord miniMapRecord;
        public CurrentMapEventsRecord currentMapEventsRecord;
        public List<PlayRecord> playRecords;

	}


    // 数据持久化控制器
	public class PersistDataManager{

        // 标记在数据加载过程中是否进行了数据更新
		public bool versionUpdateWhenLoad = false;

		public DataBackUpWhenVersionUpdate dataBackUp;
      
		/// <summary>
		/// 保存游戏购买记录
		/// </summary>
		public void SaveBuyRecord(){
			DataHandler.SaveInstanceDataToFile<BuyRecord> (BuyRecord.Instance, CommonData.buyRecordFilePath, true);
            
		}

		public void BackUpDataWhenUpdataVersion(CheckDataModel checkData){
			dataBackUp = new DataBackUpWhenVersionUpdate();
			dataBackUp.playerData= checkData.playerData;
			dataBackUp.buyRecord = checkData.buyRecord;
			dataBackUp.chatRecords = checkData.chatRecords;
			dataBackUp.currentMapEventsRecord = checkData.currentMapEventsRecord;
			dataBackUp.gameSettings = checkData.gameSettings;
			dataBackUp.mapEventsRecords = checkData.mapEventsRecords;
			dataBackUp.miniMapRecord = checkData.miniMapRecord;
			dataBackUp.playRecords = checkData.playRecords;
		}

        /// <summary>
        /// 保存地图事件记录
        /// </summary>
		public void SaveMapEventsRecord(){
			DataHandler.SaveInstanceListToFile<MapEventsRecord>(GameManager.Instance.gameDataCenter.mapEventsRecords, CommonData.mapEventsRecordFilePath);
		}

		/// <summary>
        /// 重置地图事件触发记录文件
        /// </summary>
        public void ResetMapEventsRecord(){

            bool hasRecord = DataHandler.FileExist(CommonData.mapEventsRecordFilePath);

            if(hasRecord){

                DataHandler.DeleteFile(CommonData.mapEventsRecordFilePath);
            }

            List<MapEventsRecord> mapEventsRecords = new List<MapEventsRecord>();

            for (int i = 0; i <= CommonData.maxLevelIndex;i++){
				mapEventsRecords.Add(new MapEventsRecord(i, new List<Vector2>(),false,false));
            }

            DataHandler.SaveInstanceListToFile<MapEventsRecord>(mapEventsRecords, CommonData.mapEventsRecordFilePath);

        }       
        
        /// <summary>
        /// 重置小地图记录
        /// </summary>
		public void ResetMiniMapEventsRecord(){
		         
			bool hasRecord = DataHandler.FileExist(CommonData.miniMapRecordFilePath);

            if (hasRecord)
            {            
				DataHandler.DeleteFile(CommonData.miniMapRecordFilePath);
            }

			MiniMapRecord currentMinimapRecord = null;
			GameManager.Instance.gameDataCenter.currentMapMiniMapRecord = null;

			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(currentMinimapRecord, CommonData.miniMapRecordFilePath);


		}

        /// <summary>
        /// 重置本关地图事件记录
        /// </summary>
		public void ResetCurrentMapEventRecord(){
			
			bool hasRecord = DataHandler.FileExist(CommonData.currentMapEventsRecordFilePath);

            if (hasRecord)
            {            
				DataHandler.DeleteFile(CommonData.currentMapEventsRecordFilePath);
            }

			CurrentMapEventsRecord currentMapEventsRecord = null;

			DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
		}


		/// <summary>
		/// 保存游戏设置
		/// </summary>
		public void SaveGameSettings(){
			
			string gameSettingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "GameSettings.json");

			DataHandler.SaveInstanceDataToFile<GameSettings> (GameManager.Instance.gameDataCenter.gameSettings, gameSettingsPath);
		}
        
        /// <summary>
        /// 保存应用信息【主要是当前版本号】
        /// </summary>
		public void SaveApplicationInfo(){
			DataHandler.SaveInstanceDataToFile<ApplicationInfo>(ApplicationInfo.Instance, CommonData.applicationInfoFilePath);
		}

        /// <summary>
        /// 更新人物金钱并保存
        /// </summary>
		public void UpdateBuyGoldToPlayerDataFile(){

			PlayerData playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.playerDataFilePath);

			playerData.totalGold = Player.mainPlayer.totalGold;

			DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath, true);

		}

        /// <summary>
        /// 更新人物技能点并保存
        /// </summary>
		public void AddOneSkillPointToPlayerDataFile(){
			
			PlayerData playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.playerDataFilePath);

			playerData.skillNumLeft++;

            DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath, true);
		}



		/// <summary>
		/// 保存玩家角色数据
		/// </summary>
		public void SaveCompletePlayerData(){

			string playerDataPath = CommonData.playerDataFilePath;

			PlayerData playerData = new PlayerData (Player.mainPlayer);
         
			DataHandler.SaveInstanceDataToFile<PlayerData> (playerData, playerDataPath, true);
		}

        /// <summary>
        /// 保存当前小地图记录
        /// </summary>
		public void SaveCurrentMapMiniMapRecord(){

			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(GameManager.Instance.gameDataCenter.currentMapMiniMapRecord, CommonData.miniMapRecordFilePath);

		}

        /// <summary>
        /// 重置当前小地图记录并保存
        /// </summary>
		public void ResetCurrentMapMiniMapRecordAndSave(){

			GameManager.Instance.gameDataCenter.currentMapMiniMapRecord = null;

			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(null, CommonData.miniMapRecordFilePath);

		}

        /// <summary>
        /// 保存当前关卡地图事件记录
        /// </summary>
		public void SaveCurrentMapEventsRecords(){
			DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(GameManager.Instance.gameDataCenter.currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
		}

        /// <summary>
        /// 保存当前关卡单词学习记录
        /// </summary>
		public void SaveCurrentMapWordsRecords(){
			DataHandler.SaveInstanceListToFile<HLHWord>(GameManager.Instance.gameDataCenter.currentMapWordRecords, CommonData.currentMapWordsRecordsFilePath);
		}

        /// <summary>
        /// 清理当前单词学习记录并保存
        /// </summary>
		public void ClearCurrentMapWordsRecordAndSave(){
			GameManager.Instance.gameDataCenter.currentMapWordRecords.Clear();
			DataHandler.SaveInstanceListToFile<HLHWord>(new List<HLHWord>(), CommonData.currentMapWordsRecordsFilePath);
		}
      

		/// <summary>
		/// 从本地加载玩家游戏数据
		/// </summary>
		public PlayerData LoadPlayerData(){


			string playerDataPath = CommonData.playerDataFilePath;

			if (!File.Exists (playerDataPath)) {
				playerDataPath = CommonData.oriPlayerDataFilePath;
			}

			if (!File.Exists (playerDataPath)) {
				string error = "未找到玩家信息数据";
				Debug.LogError (error);
				return null;
			}

			PlayerData pd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (playerDataPath);

			return pd;

		}
      

        /// <summary>
        /// 玩家角色数据重置回初始状态
        /// </summary>
		public void ResetPlayerDataToOriginal(){

			string sourcePlayerDataPath = CommonData.oriPlayerDataFilePath;

			string targetPlayerDataPath = CommonData.playerDataFilePath;

            // 使用原始角色数据作为重置数据源
			PlayerData resetPd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (sourcePlayerDataPath);
                 
			//PlayerData oriPd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (targetPlayerDataPath);

   //         // 读取当前角色数据中的新玩家标记
			//resetPd.isNewPlayer = oriPd.isNewPlayer;

            // 学习数据更新进新数据中
			resetPd.totalLearnedWordCount = LearningInfo.Instance.learnedWordCount;
            resetPd.totalUngraspWordCount = LearningInfo.Instance.ungraspedWordCount;

			resetPd.simpleWordContinuousRightRecord = Player.mainPlayer.simpleWordContinuousRightRecord;
			resetPd.maxSimpleWordContinuousRightRecord = Player.mainPlayer.maxSimpleWordContinuousRightRecord;
			resetPd.titleQualificationsOfSimple = Player.mainPlayer.titleQualificationsOfSimple;

			resetPd.mediumWordContinuousRightRecord = Player.mainPlayer.mediumWordContinuousRightRecord;
			resetPd.maxMediumWordContinuousRightRecord = Player.mainPlayer.maxMediumWordContinuousRightRecord;
			resetPd.titleQualificationsOfMedium = Player.mainPlayer.titleQualificationsOfMedium;

			resetPd.masterWordContinuousRightRecord = Player.mainPlayer.masterWordContinuousRightRecord;
			resetPd.maxMasterWordContinuousRightRecord = Player.mainPlayer.maxMasterWordContinuousRightRecord;
			resetPd.titleQualificationsOfMaster = Player.mainPlayer.titleQualificationsOfMaster;
         
            // 一些不随重置改变的属性
			resetPd.isNewPlayer = Player.mainPlayer.isNewPlayer;
			resetPd.needChooseDifficulty = Player.mainPlayer.needChooseDifficulty;
			resetPd.mapIndexRecord.Clear();

            // 重置本次探索的探索数据
			resetPd.learnedWordsCountInCurrentExplore = 0;
			resetPd.correctWordsCountInCurrentExplore = 0;
			resetPd.totaldefeatMonsterCount = 0;

			resetPd.currentExploreStartDateString = DateTime.Now.ToShortDateString();
			resetPd.spellRecord.Clear();

			if(ExploreManager.Instance != null){
				ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft = 0;
			}
            
            // 重新初始化人物
			Player.mainPlayer.SetUpPlayerWithPlayerData (resetPd);
            // 地图随机
			Player.mainPlayer.InitializeMapIndex();
            // 重置聊天记录  
			ResetChatRecords();
            // 重置地图事件记录
			ResetMapEventsRecord();
            // 重置小地图记录
			ResetMiniMapEventsRecord();
            // 重置本关地图事件记录
			ResetCurrentMapEventRecord();
            // 保存玩家角色数据
			SaveCompletePlayerData();
            
		}

        /// <summary>
        /// 角色死亡时重置数据
        /// </summary>
		public void ResetDataWhenPlayerDie(){

            // 加载本地存档
			PlayerData playerData = LoadPlayerData();
            // 初始化
			Player.mainPlayer.SetUpPlayerWithPlayerData(playerData);
            // 掉落装备，损失经验和金钱作为死亡惩罚
			Player.mainPlayer.LoseEquipmentsAndExperienceAndGoldWhenDie();
            // 重算角色属性
			Player.mainPlayer.ResetBattleAgentProperties(false);
            // 保存玩家数据
			SaveCompletePlayerData();
            // 重置小地图数据
			ResetMiniMapEventsRecord();
            // 重置当前关卡地图事件记录
			GameManager.Instance.gameDataCenter.currentMapEventsRecord.Reset();
            // 保存特殊地图事件记录
			SaveMapEventsRecord();

			GameManager.Instance.gameDataCenter.currentMapEventsRecord = DataHandler.LoadDataToSingleModelWithPath<CurrentMapEventsRecord>(CommonData.currentMapEventsRecordFilePath);
		}

             
        /// <summary>
        /// 保存和npc的聊天记录
        /// </summary>
		public void SaveChatRecords(){

			List<HLHNPCChatRecord> chatRecords = GameManager.Instance.gameDataCenter.chatRecords;

			DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(chatRecords, CommonData.chatRecordsFilePath);

		}


        /// <summary>
        /// 重置和npc的聊天记录
        /// </summary>
		public void ResetChatRecords(){         
			DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(new List<HLHNPCChatRecord>(), CommonData.chatRecordsFilePath);
		}


        /// <summary>
        /// 保存通关记录
        /// </summary>
        /// <param name="playRecords">Play records.</param>
		public void SavePlayRecords(List<PlayRecord> playRecords){
			DataHandler.SaveInstanceListToFile<PlayRecord>(playRecords, CommonData.playRecordsFilePath);
		}




		/// <summary>
        /// 探索场景中保存数据
        /// </summary>
        /// <param name="saveFinishCallBack">Save finish call back.</param>
        /// <param name="updateDB">If set to <c>true</c> update db.</param>
        public void SaveDataInExplore(CallBack saveFinishCallBack, bool updateDB = true)
        {         
			if(ExploreManager.Instance != null){
				Player.mainPlayer.savePosition = ExploreManager.Instance.battlePlayerCtr.transform.position;
                Player.mainPlayer.saveTowards = ExploreManager.Instance.battlePlayerCtr.towards;
			}

            // 实际的保存操作
			if (updateDB && ExploreManager.Instance != null)
            {
				ExploreManager.Instance.UpdateWordDataBase();
            }

            SaveGameSettings();
            SaveMapEventsRecord();
            SaveCompletePlayerData();
            SaveCurrentMapMiniMapRecord();
            SaveCurrentMapEventsRecords();
            SaveChatRecords();
            SaveCurrentMapWordsRecords();
         
            // 保存完成后的回调
            if (saveFinishCallBack != null)
            {
                saveFinishCallBack();
            }

        }




	}
}
