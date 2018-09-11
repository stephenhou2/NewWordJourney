using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;
	using System;

	public class PersistDataManager{

		/// <summary>
		/// 保存游戏设置，学习信息，玩家游戏数据到本地
		/// </summary>
		public void SavePersistDatas(){
			SaveGameSettings ();
//			SaveLearnInfo ();
			SaveCompletePlayerData ();
		}

		/// <summary>
		/// 保存游戏购买记录
		/// </summary>
		public void SaveBuyRecord(){
			DataHandler.SaveInstanceDataToFile<BuyRecord> (BuyRecord.Instance, CommonData.buyRecordFilePath);

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
        


		/// <summary>
		/// Saves the game settings.
		/// </summary>
		public void SaveGameSettings(){
			
			string gameSettingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "GameSettings.json");

			DataHandler.SaveInstanceDataToFile<GameSettings> (GameManager.Instance.gameDataCenter.gameSettings, gameSettingsPath);
		}

		/// <summary>
		/// Saves the learn info.
		/// </summary>
//		public void SaveLearnInfo(){
//			
//			string learnInfoPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "LearningInfo.json");
//
//			DataHandler.SaveInstanceDataToFile<LearningInfo> (learnInfo, learnInfoPath);
//		}

		/// <summary>
		/// Saves the player data.
		/// </summary>
		public void SaveCompletePlayerData(){

			string playerDataPath = CommonData.playerDataFilePath;

			PlayerData playerData = new PlayerData (Player.mainPlayer);
         
			DataHandler.SaveInstanceDataToFile<PlayerData> (playerData, playerDataPath);
		}

		public void SaveMiniMapRecords(){

			DataHandler.SaveInstanceListToFile<MiniMapRecord>(GameManager.Instance.gameDataCenter.allMiniMapRecords, CommonData.miniMapRecordsFilePath);

		}

		public void SaveCurrentMapEventsRecords(){
			DataHandler.SaveInstanceDataToFile<CurrentMapEventsRecord>(GameManager.Instance.gameDataCenter.currentMapEventsRecord, CommonData.currentMapEventsRecordFilePath);
		}


		public void RefreshSpellItem(){
			List<SpellItemModel> spellItemModels = GameManager.Instance.gameDataCenter.allSpellItemModels;
			DataHandler.SaveInstanceListToFile<SpellItemModel>(spellItemModels, CommonData.spellItemDataFilePath);
		}

		public void ResetSpellItemModels(){
			List<SpellItemModel> spellItemModels = GameManager.Instance.gameDataCenter.allSpellItemModels;
            for (int i = 0; i < spellItemModels.Count; i++)
            {
                spellItemModels[i].hasUsed = false;
            }
            DataHandler.SaveInstanceListToFile<SpellItemModel>(spellItemModels, CommonData.spellItemDataFilePath);
		}

		/// <summary>
		/// 从本地加载玩家游戏数据
		/// </summary>
		public PlayerData LoadPlayerData(){


			string playerDataPath = Path.Combine (CommonData.persistDataPath, "PlayerData.json");

			if (!File.Exists (playerDataPath)) {
				playerDataPath = Path.Combine (CommonData.persistDataPath, "OriginalPlayerData.json");
			}

			if (!File.Exists (playerDataPath)) {
				string error = "未找到玩家信息数据";
				Debug.LogError (error);
				return null;
			}

			PlayerData pd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (playerDataPath);

			return pd;

		}


		//public GameSettings LoadGameSettings(){
		//	string settingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "");
		//	return DataHandler.LoadDataToSingleModelWithPath<GameSettings> (settingsPath);
		//}

//		public LearningInfo LoadLearnInfo(){
//			string learnInfoPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "LearningInfo.json");
//			return DataHandler.LoadDataToSingleModelWithPath<LearningInfo> (learnInfoPath);
//		}


		public void ResetPlayerDataToOriginal(){

			string sourcePlayerDataPath = CommonData.oriPlayerDataFilePath;

			string targetPlayerDataPath = CommonData.playerDataFilePath;

			PlayerData resetPd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (sourcePlayerDataPath);
                     
			PlayerData oriPd = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (targetPlayerDataPath);

			resetPd.isNewPlayer = oriPd.isNewPlayer;

			resetPd.totalLearnedWordCount = Player.mainPlayer.totalLearnedWordCount;
			resetPd.totalUngraspWordCount = Player.mainPlayer.totalUngraspWordCount;
			resetPd.wordContinuousRightRecord = Player.mainPlayer.wordContinuousRightRecord;
			resetPd.maxWordContinuousRightRecord = Player.mainPlayer.maxWordContinuousRightRecord;
			resetPd.titleQualifications = Player.mainPlayer.titleQualifications;
			resetPd.isNewPlayer = Player.mainPlayer.isNewPlayer;
			resetPd.needChooseDifficulty = Player.mainPlayer.needChooseDifficulty;

			resetPd.learnedWordsCountInCurrentExplore = 0;
			resetPd.correctWordsCountInCurrentExplore = 0;

			resetPd.currentExploreStartDateString = DateTime.Now.ToShortDateString();


			Player.mainPlayer.SetUpPlayerWithPlayerData (resetPd);

			Player.mainPlayer.InitializeMapIndex();

			SaveCompletePlayerData ();

			ResetChatRecords();

			ResetMapEventsRecord();

			ResetMapEventsRecord();

			ResetSpellItemModels();

			GameManager.Instance.gameDataCenter.ResetGameData();

		}

		public void SaveChatRecords(int npcId,int npcDialogGroupId){
			
			List<HLHNPCChatRecord> chatRecords = GameManager.Instance.gameDataCenter.chatRecords;

			HLHNPCChatRecord chatRecord = new HLHNPCChatRecord(npcId, npcDialogGroupId);

			chatRecords.Add(chatRecord);

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
