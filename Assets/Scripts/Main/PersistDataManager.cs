using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;

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
			
			string playerDataPath = Path.Combine (CommonData.persistDataPath, "PlayerData.json");

			PlayerData playerData = new PlayerData (Player.mainPlayer);

//			playerData.isNewPlayer = false;

			DataHandler.SaveInstanceDataToFile<PlayerData> (playerData, playerDataPath);
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

			return DataHandler.LoadDataToSingleModelWithPath<PlayerData> (playerDataPath);

		}


		public GameSettings LoadGameSettings(){
			string settingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "GameSettings.json");
			return DataHandler.LoadDataToSingleModelWithPath<GameSettings> (settingsPath);
		}

//		public LearningInfo LoadLearnInfo(){
//			string learnInfoPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "LearningInfo.json");
//			return DataHandler.LoadDataToSingleModelWithPath<LearningInfo> (learnInfoPath);
//		}


		public void ResetPlayerDataToOriginal(){

			string sourcePlayerDataPath = CommonData.persistDataPath + "/OriginalPlayerData.json";
			string targetPlayerDataPath = CommonData.persistDataPath + "/PlayerData.json";
			DataHandler.CopyFile (sourcePlayerDataPath, targetPlayerDataPath);

		}

	}
}
