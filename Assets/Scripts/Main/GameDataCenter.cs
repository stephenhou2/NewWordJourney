using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;

	public class GameDataCenter {

		public enum GameDataType
		{
			GameSettings,
			GameLevelDatas,
			EquipmentModels,
			ConsumablesModels,
			SkillGemstoneModels,
			EquipmentSprites,
			ConsumablesSprites,
			SkillGemstoneSprites,
			MapSprites,
			Skills,
			SkillSprites,
			Monsters,
			NPCs,
		}

		private GameSettings mGameSettings;
		private List<HLHGameLevelData> mGameLevelDatas = new List<HLHGameLevelData>();
		private List<EquipmentModel> mAllEquipmentModels = new List<EquipmentModel> ();
		private List<ConsumablesModel> mAllConsumablesModels = new List<ConsumablesModel>();
		private List<SkillGemstoneModel> mAllSkillGemstoneModels = new List<SkillGemstoneModel> ();
		private List<Sprite> mAllEquipmentSprites = new List<Sprite>();
		private List<Sprite> mAllConsumablesSprites = new List<Sprite> ();
		private List<Sprite> mAllSkillGemstoneSprites = new List<Sprite> ();
		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		private List<Skill> mAllSkills = new List<Skill>();
		private List<Sprite> mAllSkillSprites = new List<Sprite>();
//		private List<HLHNPC> mAllNpcs = new List<HLHNPC>();


		public void InitPersistentGameData(){
			LoadEquipmentModels ();
			LoadAllEquipmentSprites ();
			LoadAllSkills ();
		}


		public GameSettings gameSettings{

			get{
				if (mGameSettings == null) {
					LoadGameSettings ();
				}
				return mGameSettings;
			}
			set{
				mGameSettings = value;
			}
		}

		private void LoadGameSettings(){
			if (mGameSettings != null) {
				return;
			}
			mGameSettings = GameManager.Instance.persistDataManager.LoadGameSettings ();
			if (mGameSettings == null) {
				mGameSettings = new GameSettings ();
			}
		}




//		public LearningInfo learnInfo{
//			get{
//				if (mLearnInfo == null) {
//					LoadLearnInfo ();
//				}
//				return mLearnInfo;
//			}
//		}
//
//		private void LoadLearnInfo(){
//			if(inLoadingDataTypes.Contains(GameDataType.LearnInfo)){
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.LearnInfo);
//			mLearnInfo = GameManager.Instance.persistDataManager.LoadLearnInfo ();
//			if (mLearnInfo == null) {
//				mLearnInfo = new LearningInfo ();
//			}
//			dataReadyDic [GameDataType.LearnInfo] = true;
//			inLoadingDataTypes.Remove (GameDataType.LearnInfo);
//		}


		public List<HLHGameLevelData> gameLevelDatas{
			get{
				if (mGameLevelDatas.Count == 0) {
					LoadGameLevelDatas ();
				}
				return mGameLevelDatas;
			}

		}

		private void LoadGameLevelDatas(){
			HLHGameLevelData[] gameLevelDatasArray = DataHandler.LoadDataToModelsWithPath<HLHGameLevelData> (CommonData.gameLevelDataFilePath);
			for (int i = 0; i < gameLevelDatasArray.Length; i++) {
				mGameLevelDatas.Add(gameLevelDatasArray[i]);
			}
		}


		public List<EquipmentModel> allEquipmentModels{
			get{
				if (mAllEquipmentModels.Count == 0) {
					LoadEquipmentModels ();
				}
				return mAllEquipmentModels;
			}

		}

		private void LoadEquipmentModels(){
			if (mAllEquipmentModels.Count > 0) {
				return;
			}
			EquipmentModel[] itemModels = DataHandler.LoadDataToModelsWithPath<EquipmentModel> (CommonData.equipmentDataFilePath);
			for (int i = 0; i < itemModels.Length; i++) {
				mAllEquipmentModels.Add (itemModels [i]);
			}
		}
			
		public List<ConsumablesModel> allConsumablesModels{
			get{
				if (mAllConsumablesModels.Count == 0) {
					LoadConsumablesModels ();
				}
				return mAllConsumablesModels;
			}

		}

		private void LoadConsumablesModels(){
			if (mAllConsumablesModels.Count > 0) {
				return;
			}
			ConsumablesModel[] consumablesModels = DataHandler.LoadDataToModelsWithPath<ConsumablesModel> (CommonData.consumablesDataFilePath);
			for (int i = 0; i < consumablesModels.Length; i++) {
				mAllConsumablesModels.Add (consumablesModels [i]);
			}
		}

		public List<SkillGemstoneModel> allSkillGemstoneModels{
			get{
				if (mAllSkillGemstoneModels.Count == 0) {
					LoadSkillGemstoneModels ();
				}
				return mAllSkillGemstoneModels;
			}

		}

		private void LoadSkillGemstoneModels(){
			if (mAllSkillGemstoneModels.Count > 0) {
				return;
			}
			SkillGemstoneModel[] skillGemstoneModels = DataHandler.LoadDataToModelsWithPath<SkillGemstoneModel> (CommonData.skillGemstoneDataFilePath);
			for (int i = 0; i < skillGemstoneModels.Length; i++) {
				mAllSkillGemstoneModels.Add (skillGemstoneModels [i]);
			}
		}
	
		public List<Sprite> allEquipmentSprites{
			get{
				if (mAllEquipmentSprites.Count == 0) {
					LoadAllEquipmentSprites();
				}
				return mAllEquipmentSprites;
			}

		}

		private void LoadAllEquipmentSprites(){
			if (mAllEquipmentSprites.Count > 0) {
				return;
			}
			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allEquipmentSpritesBundleName);
			for (int i = 0; i < spriteCache.Length; i++) {
				mAllEquipmentSprites.Add (spriteCache[i]);
			}
		}

		public List<Sprite> allConsumablesSprites{
			get{
				if (mAllConsumablesSprites.Count == 0) {
					LoadAllConsumablesSprites();
				}
				return mAllConsumablesSprites;
			}

		}

		private void LoadAllConsumablesSprites(){
			if (mAllConsumablesSprites.Count > 0) {
				return;
			}
			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allConsumablesSpritesBundleName);
			for (int i = 0; i < spriteCache.Length; i++) {
				mAllConsumablesSprites.Add (spriteCache[i]);
			}
		}

		public List<Sprite> allSkillGemstoneSprites{
			get{
				if (mAllSkillGemstoneSprites.Count == 0) {
					LoadAllSkillGemstoneSprites();
				}
				return mAllSkillGemstoneSprites;
			}

		}

		private void LoadAllSkillGemstoneSprites(){
			if (mAllSkillGemstoneSprites.Count > 0) {
				return;
			}
			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allSkillGemstoneSpritesBundleName);
			for (int i = 0; i < spriteCache.Length; i++) {
				mAllSkillGemstoneSprites.Add (spriteCache[i]);
			}
		}

		public Sprite GetGameItemSprite(Item item){
			Sprite s = null;
			switch (item.itemType) {
			case ItemType.Equipment:
				s = allEquipmentSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});
				break;
			case ItemType.Consumables:
				s = allConsumablesSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});
				break;
			case ItemType.Gemstone:
				s = allSkillGemstoneSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});
				break;
			}
			return s;
		}

		public Sprite GetGameItemSprite(string spriteName){
			
			Sprite s = null;

			s = allEquipmentSprites.Find (delegate(Sprite obj) {
				return obj.name == spriteName;
			});

			if (s == null) {
				s = allConsumablesSprites.Find (delegate(Sprite obj) {
					return obj.name == spriteName;
				});
			}

			if (s == null) {
				s = allSkillGemstoneSprites.Find (delegate(Sprite obj) {
					return obj.name == spriteName;
				});
			}

			return s;
		}

		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {
					Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allMapSpritesBundleName);
					for (int i = 0; i < spriteCache.Length; i++) {
						mAllMapSprites.Add (spriteCache[i]);
					}
				}
				return mAllMapSprites;
			}
		}


		public List<Skill> allSkills{
			get{
				if(mAllSkills.Count == 0){
					LoadAllSkills ();
				}
				return mAllSkills;
			}
		}

		private void LoadAllSkills(){

			if (mAllSkills.Count > 0) {
				return;
			}

			GameObject[] skillCache = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allSkillsBundleName);

			Transform skillsContainer = TransformManager.FindOrCreateTransform ("AllSkills");

			for (int i = 0; i < skillCache.Length; i++) {
				GameObject skill = GameObject.Instantiate (skillCache [i]);
				skill.name = skillCache [i].name;
				skill.transform.SetParent (skillsContainer);
				mAllSkills.Add(skill.GetComponent<Skill>());
			}

			SortSkillsById (mAllSkills);
		}



	
		// 技能按照id排序方法
		private void SortSkillsById(List<Skill> skills){
			Skill temp;
			for (int i = 0; i < skills.Count - 1; i++) {
				for (int j = 0; j < skills.Count - 1 - i; j++) {
					Skill sBefore = skills [j];
					Skill sAfter = skills [j + 1];
					if (sBefore.skillId > sAfter.skillId) {
						temp = sBefore;
						skills [j] = sAfter;
						skills [j + 1] = temp; 
					}
				}
			}
		}
			

		public List<Sprite> allSkillSprites{
			get{
				if (mAllSkillSprites.Count == 0) {
					Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allSkillSpritesBundleName);
					for (int i = 0; i < spriteCache.Length; i++) {
						mAllSkillSprites.Add (spriteCache[i]);
					}
				}
				return mAllSkillSprites;
			}
		}


	

//		public List<HLHNPC> allNpcs{
//			get{
//				if (mAllNpcs.Count == 0) {
//					LoadNPCs ();
//				}
//				return mAllNpcs;
//			}
//		}


//		private void LoadNPCs(){
//
//			if (mAllNpcs.Count > 0) {
//				return;
//			}
//
//			string npcDataDirectory = CommonData.npcsDataDirPath;
//
//			DirectoryInfo npcDirectoryInfo = new DirectoryInfo (npcDataDirectory);
//
//			FileInfo[] npcFiles = npcDirectoryInfo.GetFiles ();
//
//			for (int i = 0; i <npcFiles.Length ; i++) {
//				FileInfo npcData = npcFiles [i];
//				if (npcData.Extension != ".json") {
//					continue;
//				}
//				HLHNPC npc = null;
//				if (npcData.Name.Contains ("Normal")) {
//					npc = DataHandler.LoadDataToSingleModelWithPath<HLHNPC> (npcData.FullName);
//				}else if(npcData.Name.Contains("Trader")){
//					npc = DataHandler.LoadDataToSingleModelWithPath<HLHNPC> (npcData.FullName);
//				}
//				mAllNpcs.Add (npc);
//			}
//		}

		/// <summary>
		/// 加载指定id的npc数据
		/// </summary>
		/// <returns>The npc.</returns>
		/// <param name="npcId">Npc identifier.</param>
		public HLHNPC LoadNpc(int npcId){

			string npcDataPath = string.Format ("{0}/NPC_{1}.json", CommonData.npcsDataDirPath,npcId);

			HLHNPC npc = DataHandler.LoadDataToSingleModelWithPath<HLHNPC> (npcDataPath);

			return npc;
		}


		/// <summary>
		/// 加载指定id的npc数据
		/// </summary>
		/// <returns>The npc.</returns>
		/// <param name="npcId">Npc identifier.</param>
		public GameObject LoadMapNpc(string npcName){
			
			GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allMapNpcBundleName, npcName);

			GameObject mapNpc = GameObject.Instantiate (assets [0]);

			mapNpc.name = assets [0].name;

			return mapNpc;

		}



		public GameObject LoadMonster(string monsterName){

			GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allMonstersBundleName, monsterName);

			GameObject monster = GameObject.Instantiate (assets [0]);

			monster.name = assets [0].name;

			return monster;

		}



		public void ReleaseDataWithDataTypes(GameDataType[] dataTypes){

			for (int i = 0; i < dataTypes.Length; i++) {
				ReleaseDataWithName (dataTypes [i]);
			}

//			Resources.UnloadUnusedAssets ();
//
//			System.GC.Collect ();

		}

		private void ReleaseDataWithName(GameDataType type){

			switch (type) {
			case GameDataType.GameSettings:
				mGameSettings = null;
				break;
//			case GameDataType.LearnInfo:
//				mLearnInfo = null;
//				dataReadyDic [GameDataType.LearnInfo] = false;
//				break;
			case GameDataType.GameLevelDatas:
				mGameLevelDatas.Clear ();
				break;
			case GameDataType.EquipmentModels:
				mAllEquipmentModels.Clear ();
				break;
			case GameDataType.ConsumablesModels:
				mAllConsumablesModels.Clear ();
				break;
			case GameDataType.SkillGemstoneModels:
				mAllSkillGemstoneModels.Clear ();
				break;
			case GameDataType.EquipmentSprites:
				mAllEquipmentSprites.Clear ();
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allEquipmentSpritesBundleName,true);
				break;
			case GameDataType.ConsumablesSprites:
				mAllConsumablesSprites.Clear ();
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allConsumablesSpritesBundleName,true);
				break;
			case GameDataType.SkillGemstoneSprites:
				mAllSkillGemstoneSprites.Clear ();
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allSkillGemstoneSpritesBundleName,true);
				break;
			case GameDataType.MapSprites:
				mAllMapSprites.Clear ();
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allMapSpritesBundleName,true);
				break;
			case GameDataType.Skills:
				mAllSkills.Clear ();
				TransformManager.DestroyTransfromWithName ("AllSkills");
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allSkillsBundleName,true);
				break;
			case GameDataType.SkillSprites:
				mAllSkillSprites.Clear ();
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allSkillSpritesBundleName,true);
				break;
			case GameDataType.Monsters:
//				TransformManager.DestroyTransfromWithName ("MonstersContainer", TransformRoot.InstanceContainer);
				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allMonstersBundleName,true);
				break;
//			case GameDataType.NPCs:
//				mAllNpcs.Clear ();
//				break;
			}
		}



	}
}
