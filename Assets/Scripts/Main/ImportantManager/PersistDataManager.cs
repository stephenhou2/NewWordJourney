using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;
	using System;

	public class PersistDataManager{



		/// <summary>
		/// 保存游戏购买记录
		/// </summary>
		public void SaveBuyRecord(){
			DataHandler.SaveInstanceDataToFile<BuyRecord> (BuyRecord.Instance, CommonData.buyRecordFilePath, true);

		}

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
        
		public void ResetMiniMapEventsRecord(){
		         
			bool hasRecord = DataHandler.FileExist(CommonData.miniMapRecordFilePath);

            if (hasRecord)
            {            
				DataHandler.DeleteFile(CommonData.miniMapRecordFilePath);
            }

			MiniMapRecord currentMinimapRecord = null;

			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(currentMinimapRecord, CommonData.miniMapRecordFilePath);


		}

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
		/// Saves the game settings.
		/// </summary>
		public void SaveGameSettings(){
			
			string gameSettingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "GameSettings.json");

			DataHandler.SaveInstanceDataToFile<GameSettings> (GameManager.Instance.gameDataCenter.gameSettings, gameSettingsPath);
		}
        

		public void UpdateBuyGoldToPlayerDataFile(){

			PlayerData playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData>(CommonData.playerDataFilePath);

			playerData.totalGold = Player.mainPlayer.totalGold;

			DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.playerDataFilePath, true);

		}



		/// <summary>
		/// Saves the player data.
		/// </summary>
		public void SaveCompletePlayerData(){

			string playerDataPath = CommonData.playerDataFilePath;

			PlayerData playerData = new PlayerData (Player.mainPlayer);
         
			DataHandler.SaveInstanceDataToFile<PlayerData> (playerData, playerDataPath, true);
		}

		public void SaveCurrentMapMiniMapRecord(){

			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(GameManager.Instance.gameDataCenter.currentMapMiniMapRecord, CommonData.miniMapRecordFilePath);

		}

		public void ResetCurrentMapMiniMapRecordAndSave(){

			GameManager.Instance.gameDataCenter.currentMapMiniMapRecord = null;

			DataHandler.SaveInstanceDataToFile<MiniMapRecord>(null, CommonData.miniMapRecordFilePath);

		}

		public void SaveCurrentMapEventsRecords(){
			DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(GameManager.Instance.gameDataCenter.currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
		}

		public void SaveCurrentMapWordsRecords(){
			DataHandler.SaveInstanceListToFile<HLHWord>(GameManager.Instance.gameDataCenter.currentMapWordRecords, CommonData.currentMapWordsRecordsFilePath);
		}

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
      

		public void ResetPlayerDataToOriginal(){

			string sourcePlayerDataPath = CommonData.oriPlayerDataFilePath;

			string targetPlayerDataPath = CommonData.playerDataFilePath;

			PlayerData resetPd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (sourcePlayerDataPath);
                     
			PlayerData oriPd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (targetPlayerDataPath);

			resetPd.isNewPlayer = oriPd.isNewPlayer;

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
         

			resetPd.isNewPlayer = Player.mainPlayer.isNewPlayer;
			resetPd.needChooseDifficulty = Player.mainPlayer.needChooseDifficulty;
			resetPd.mapIndexRecord.Clear();

			resetPd.learnedWordsCountInCurrentExplore = 0;
			resetPd.correctWordsCountInCurrentExplore = 0;
			resetPd.totaldefeatMonsterCount = 0;

			resetPd.currentExploreStartDateString = DateTime.Now.ToShortDateString();
			resetPd.currentVersion = Player.mainPlayer.currentVersion;
			resetPd.spellRecord.Clear();
         
			Player.mainPlayer.SetUpPlayerWithPlayerData (resetPd);

			Player.mainPlayer.InitializeMapIndex();
                     
			ResetChatRecords();

			ResetMapEventsRecord();

			ResetMiniMapEventsRecord();

			ResetCurrentMapEventRecord();

			SaveCompletePlayerData();

		}

		public void ResetDataWhenPlayerDie(){

			PlayerData playerData = LoadPlayerData();

			Player.mainPlayer.SetUpPlayerWithPlayerData(playerData);

			Player.mainPlayer.LoseEquipmentsAndExperienceWhenDie();

			SaveCompletePlayerData(); 

			SaveMapEventsRecord();


                       
		}





		public void SaveChatRecords(){

			List<HLHNPCChatRecord> chatRecords = GameManager.Instance.gameDataCenter.chatRecords;

			DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(chatRecords, CommonData.chatRecordsFilePath);

		}

		public void ResetChatRecords(){         
			DataHandler.SaveInstanceListToFile<HLHNPCChatRecord>(new List<HLHNPCChatRecord>(), CommonData.chatRecordsFilePath);
		}

		public void SavePlayRecords(List<PlayRecord> playRecords){
			DataHandler.SaveInstanceListToFile<PlayRecord>(playRecords, CommonData.playRecordsFilePath);
		}

	}
}
