using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;

	public class GameDataCenter
	{

		public enum GameDataType
		{
			GameSettings,
			GameLevelDatas,
			EquipmentModels,
			ConsumablesModels,
			SkillGemstoneModels,
			SpecialItemModels,
			SkillScrollModels,
			SpellItemModels,
			EquipmentSprites,
			ConsumablesSprites,
			SkillGemstoneSprites,
			SpecialItemSprites,
			SkillScrollSprites,
			MapSprites,
			MiniMapNpcSprites,
			MapTileAtlas,
			CharacterSprites,
			Skills,
			SkillSprites,
			Monsters,
			MonstersUI,
			MonstersData,
			NPCs,
			Effects,
			Proverbs,
			ChatRecord,
			Diary,
			Puzzle,
			PlayRecord,
			MiniMapRecord,
            //CurrentMapEventsRecord,
			ExploreScene,
			BagCanvas,
			NPCCanvas,
			ShareCanvas,
			SettingCanvas,
			RecordCanvas,
			HomeCanvas,
			LoadingCanvas,
			GuideCanvas,
			PlayRecordCanvas
		}

		private GameSettings mGameSettings;
		private List<HLHGameLevelData> mGameLevelDatas = new List<HLHGameLevelData>();
		private List<EquipmentModel> mAllEquipmentModels = new List<EquipmentModel>();
		private List<ConsumablesModel> mAllConsumablesModels = new List<ConsumablesModel>();
		private List<PropertyGemstoneModel> mAllPropertyGemstoneModels = new List<PropertyGemstoneModel>();
		private List<SkillScrollModel> mAllSkillScrollModels = new List<SkillScrollModel>();
		private List<SpecialItemModel> mAllSpecialItemModels = new List<SpecialItemModel>();
		private List<SpellItemModel> mAllSpellItemModels = new List<SpellItemModel>();
		private List<Sprite> mAllEquipmentSprites = new List<Sprite>();
		private List<Sprite> mAllConsumablesSprites = new List<Sprite>();
		private List<Sprite> mAllPropertyGemstoneSprites = new List<Sprite>();
		private List<Sprite> mAllSkillScrollSprites = new List<Sprite>();
		private List<Sprite> mAllSpecialItemSprites = new List<Sprite>();
		private List<Sprite> mAllMapSprites = new List<Sprite>();
		private List<Sprite> mAllMinimapNpcSprites = new List<Sprite>();
		private List<Sprite> mAllCharacterSprites = new List<Sprite>();
		private List<Skill> mAllSkills = new List<Skill>();
		private List<Sprite> mAllSkillSprites = new List<Sprite>();
		private List<EffectAnim> mAllEffects = new List<EffectAnim>();
		private List<HLHSentenceAndPoem> mAllProverbs = new List<HLHSentenceAndPoem>();
		private List<Puzzle> mAllPuzzles = new List<Puzzle>();
		private List<HLHNPCChatRecord> mChatRecords = new List<HLHNPCChatRecord>();
		private List<MapEventsRecord> mMapEventsRecords = new List<MapEventsRecord>();
		private List<DiaryModel> mAllDiaryModels = new List<DiaryModel>();
		private List<PlayRecord> mAllPlayRecords = new List<PlayRecord>();
		private List<MiniMapRecord> mAllMiniMapRecords = new List<MiniMapRecord>();

		private List<MonsterData> mAllMonstersData = new List<MonsterData>();
		private CurrentMapEventsRecord mCurrentMapEventsRecord;

		public void InitExplorePrepareGameData()
		{
			LoadEquipmentModels();
			LoadAllEquipmentSprites();
			LoadAllSkills();
			LoadAllEffects();
		}


		public GameSettings gameSettings
		{

			get
			{
				if (mGameSettings == null)
				{
					LoadGameSettings();
				}
				return mGameSettings;
			}
			set
			{
				mGameSettings = value;
			}
		}

		private void LoadGameSettings()
		{
			if (mGameSettings != null)
			{
				return;
			}
			mGameSettings = DataHandler.LoadDataToSingleModelWithPath<GameSettings>(CommonData.gameSettingsDataFilePath);
			if (mGameSettings == null)
			{
				mGameSettings = new GameSettings();
			}
		}

		public HLHMapData LoadMapDataOfLevel(int level)
		{

			string mapDataFilePath = string.Format("{0}/MapData/Level_{1}.json", CommonData.persistDataPath, level);

			//Debug.Log(DataHandler.FileExist(mapDataFilePath));

			HLHMapData mapData = DataHandler.LoadDataToSingleModelWithPath<HLHMapData>(mapDataFilePath);

			//Debug.Log(mapData);

			return mapData;

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


		public List<HLHGameLevelData> gameLevelDatas
		{
			get
			{
				if (mGameLevelDatas.Count == 0)
				{
					LoadGameLevelDatas();
				}
				return mGameLevelDatas;
			}

		}

		public void LoadGameLevelDatas()
		{
			HLHGameLevelData[] gameLevelDatasArray = DataHandler.LoadDataToModelsWithPath<HLHGameLevelData>(CommonData.gameLevelDataFilePath);
			mGameLevelDatas.Clear();
			for (int i = 0; i < gameLevelDatasArray.Length; i++)
			{
				mGameLevelDatas.Add(gameLevelDatasArray[i]);
			}
		}


		public List<EquipmentModel> allEquipmentModels
		{
			get
			{
				if (mAllEquipmentModels.Count == 0)
				{
					LoadEquipmentModels();
				}
				return mAllEquipmentModels;
			}

		}

		private void LoadEquipmentModels()
		{
			if (mAllEquipmentModels.Count > 0)
			{
				return;
			}
			EquipmentModel[] itemModels = DataHandler.LoadDataToModelsWithPath<EquipmentModel>(CommonData.equipmentDataFilePath);
			for (int i = 0; i < itemModels.Length; i++)
			{
				mAllEquipmentModels.Add(itemModels[i]);
			}
		}

		public List<ConsumablesModel> allConsumablesModels
		{
			get
			{
				if (mAllConsumablesModels.Count == 0)
				{
					LoadConsumablesModels();
				}
				return mAllConsumablesModels;
			}

		}

		private void LoadConsumablesModels()
		{
			if (mAllConsumablesModels.Count > 0)
			{
				return;
			}
			ConsumablesModel[] consumablesModels = DataHandler.LoadDataToModelsWithPath<ConsumablesModel>(CommonData.consumablesDataFilePath);
			for (int i = 0; i < consumablesModels.Length; i++)
			{
				mAllConsumablesModels.Add(consumablesModels[i]);
			}
		}

		public List<PropertyGemstoneModel> allPropertyGemstoneModels
		{
			get
			{
				if (mAllPropertyGemstoneModels.Count == 0)
				{
					LoadPropertyGemstoneModels();
				}
				return mAllPropertyGemstoneModels;
			}

		}

		private void LoadPropertyGemstoneModels()
		{
			if (mAllPropertyGemstoneModels.Count > 0)
			{
				return;
			}
			PropertyGemstoneModel[] propertyGemstoneModels = DataHandler.LoadDataToModelsWithPath<PropertyGemstoneModel>(CommonData.propertyGemstoneDataFilePath);
			for (int i = 0; i < propertyGemstoneModels.Length; i++)
			{
				mAllPropertyGemstoneModels.Add(propertyGemstoneModels[i]);
			}
		}


		public List<SkillScrollModel> allSkillScrollModels
		{
			get
			{
				if (mAllSkillScrollModels.Count == 0)
				{
					LoadAllSkillScrollModels();
				}
				return mAllSkillScrollModels;
			}
		}

		private void LoadAllSkillScrollModels()
		{
			if (mAllSkillScrollModels.Count > 0)
			{
				return;
			}
			SkillScrollModel[] skillScrollModels = DataHandler.LoadDataToModelsWithPath<SkillScrollModel>(CommonData.skillScrollDataFilePath);
			for (int i = 0; i < skillScrollModels.Length; i++)
			{
				mAllSkillScrollModels.Add(skillScrollModels[i]);
			}
		}

		public List<SpecialItemModel> allSpecialItemModels
		{
			get
			{
				if (mAllSpecialItemModels.Count == 0)
				{
					LoadAllSpecialItemModels();
				}
				return mAllSpecialItemModels;
			}
		}

		private void LoadAllSpecialItemModels()
		{
			if (mAllSpecialItemModels.Count > 0)
			{
				return;
			}
			SpecialItemModel[] specialItemModels = DataHandler.LoadDataToModelsWithPath<SpecialItemModel>(CommonData.specialItemDataFilePath);

			for (int i = 0; i < specialItemModels.Length; i++)
			{
				mAllSpecialItemModels.Add(specialItemModels[i]);
			}

		}

		public List<SpellItemModel> allSpellItemModels
		{
			get
			{
				if (mAllSpellItemModels.Count == 0)
				{
					LoadAllSpellItemModels();
				}
				return mAllSpellItemModels;
			}
		}

		private void LoadAllSpellItemModels()
		{

			if (mAllSpellItemModels.Count > 0)
			{
				return;
			}

			SpellItemModel[] spellItemModels = DataHandler.LoadDataToModelsWithPath<SpellItemModel>(CommonData.spellItemDataFilePath);

			for (int i = 0; i < spellItemModels.Length; i++)
			{
				mAllSpellItemModels.Add(spellItemModels[i]);
			}
		}


		public List<HLHSentenceAndPoem> allProverbs
		{
			get
			{
				if (mAllProverbs.Count == 0)
				{
					LoadAllProverbs();
				}

				return mAllProverbs;
			}


		}

		private void LoadAllProverbs()
		{

			if (mAllProverbs.Count > 0)
			{
				return;
			}

			HLHSentenceAndPoem[] proverbs = DataHandler.LoadDataToModelsWithPath<HLHSentenceAndPoem>(CommonData.proverbsDataFilePath);

			for (int i = 0; i < proverbs.Length; i++)
			{
				mAllProverbs.Add(proverbs[i]);
			}
		}

		public HLHSentenceAndPoem GetARandomProverb()
		{

			int randomIndex = Random.Range(0, allProverbs.Count);

			return allProverbs[randomIndex];

		}


		public List<Puzzle> allPuzzles
		{
			get
			{
				if (mAllPuzzles.Count == 0)
				{
					LoadAllPuzzles();
				}
				return mAllPuzzles;
			}
		}

		private void LoadAllPuzzles()
		{

			if (mAllPuzzles.Count > 0)
			{
				return;
			}

			Puzzle[] puzzles = DataHandler.LoadDataToModelsWithPath<Puzzle>(CommonData.puzzlesDataFilePath);

			for (int i = 0; i < puzzles.Length; i++)
			{
				mAllPuzzles.Add(puzzles[i]);
			}

		}

		public Puzzle GetARandomPuzzle()
		{

			int randomIndex = Random.Range(0, allPuzzles.Count);

			return allPuzzles[randomIndex];

		}



		public List<DiaryModel> diaryModels
		{
			get
			{
				if (mAllDiaryModels.Count == 0)
				{
					LoadDiaryModels();
				}
				return mAllDiaryModels;
			}
		}

		private void LoadDiaryModels()
		{

			if (mAllDiaryModels.Count > 0)
			{
				return;
			}

			DiaryModel[] diaryModelArray = DataHandler.LoadDataToModelsWithPath<DiaryModel>(CommonData.diaryDataFilePath);

			for (int i = 0; i < diaryModelArray.Length; i++)
			{
				mAllDiaryModels.Add(diaryModelArray[i]);
			}
		}

		public DiaryModel GetDiaryInLevel(int level)
		{
			DiaryModel diaryModel = diaryModels.Find(delegate (DiaryModel obj)
			{
				return obj.triggeredLevel == level;
			});
			return diaryModel;
		}

		public List<MiniMapRecord> allMiniMapRecords
		{         
			get
			{
				if (mAllMiniMapRecords.Count == 0)
				{
					LoadAllMiniMapRecords();
				}            
				return mAllMiniMapRecords;            
			}         
		}

		private void LoadAllMiniMapRecords(){

			MiniMapRecord[] miniMapRecordsArray = DataHandler.LoadDataToModelsWithPath<MiniMapRecord>(CommonData.miniMapRecordsFilePath);

			for (int i = 0; i < miniMapRecordsArray.Length;i++){
				mAllMiniMapRecords.Add(miniMapRecordsArray[i]);
			}         
		}
	

		public MiniMapRecord GetMiniMapRecordAt(int mapIndex){

			MiniMapRecord record = null;

			record = allMiniMapRecords.Find(delegate (MiniMapRecord obj)
			{
				return obj.mapIndex == mapIndex;
			});

			return record;

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

		public List<Sprite> allPropertyGemstoneSprites{
			get{
				if (mAllPropertyGemstoneSprites.Count == 0) {
					LoadAllPropertyGemstoneSprites();
				}
				return mAllPropertyGemstoneSprites;
			}

		}

		private void LoadAllPropertyGemstoneSprites(){
			if (mAllPropertyGemstoneSprites.Count > 0) {
				return;
			}
			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allPropertyGemstoneSpritesBundleName);
			for (int i = 0; i < spriteCache.Length; i++) {
				mAllPropertyGemstoneSprites.Add (spriteCache[i]);
			}
		}


		public List<Sprite> allSkillScrollSprites{
			get{
				if(mAllSkillScrollSprites.Count == 0){
					LoadAllSkillScrollSprites();
				}
				return mAllSkillScrollSprites;
			}
		}

		private void LoadAllSkillScrollSprites(){
			if(mAllSkillScrollSprites.Count > 0){
				return;
			}

			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite>(CommonData.allSkillScrollSpritesBundleName);

            for (int i = 0; i < spriteCache.Length; i++)
            {
				mAllSkillScrollSprites.Add(spriteCache[i]);
            }
		}
        

		public List<Sprite> allSpecialItemSprites{
			get{
				if(mAllSpecialItemSprites.Count == 0){
					LoadAllSpecialItemSprites();
				}
				return mAllSpecialItemSprites;
			}
		}



		private void LoadAllSpecialItemSprites(){
			if(mAllSpecialItemSprites.Count > 0){
				return;
			}
			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite>(CommonData.allSpecialItemSpritesBundleName);

			for (int i = 0; i < spriteCache.Length;i++){
				mAllSpecialItemSprites.Add(spriteCache[i]);
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
    			case ItemType.PropertyGemstone:
    				s = allPropertyGemstoneSprites.Find (delegate(Sprite obj) {
    					return obj.name == item.spriteName;
    				});
					break;
				case ItemType.SkillScroll:
					s = allSkillScrollSprites.Find(delegate (Sprite obj) {
                        return obj.name == item.spriteName;
                    });
    				break;
				case ItemType.SpecialItem:
					s = allSpecialItemSprites.Find(delegate (Sprite obj) {
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
				s = allPropertyGemstoneSprites.Find (delegate(Sprite obj) {
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

		public List<Sprite> allMiniMapNpcSprites{
			get{
				if(mAllMinimapNpcSprites.Count == 0){
					Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite>(CommonData.allMinimapNpcSpritesBundleName);
					for (int i = 0; i < spriteCache.Length;i++){
						mAllMinimapNpcSprites.Add(spriteCache[i]);
					}
				}
				return mAllMinimapNpcSprites;
			}
		}

		public List<Sprite> allCHaracterSprites{
			get{
				if(mAllCharacterSprites.Count == 0){
					Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite>(CommonData.allCharacterSpritesBundleName);

					for (int i = 0; i < spriteCache.Length;i++){
						mAllCharacterSprites.Add(spriteCache[i]);
					}
				}
				return mAllCharacterSprites;
			}
		}
        

		private string GetMapTilesetSpriteBundleName(string tilesetImageName)
        {

            string tilesetSpriteBundleName = string.Empty;

            switch (tilesetImageName)
            {
                case "Dungeon_1":
                    tilesetSpriteBundleName = CommonData.mapTileset_1_BundleName;
                    break;
                case "Dungeon_2":
                    tilesetSpriteBundleName = CommonData.mapTileset_2_BundleName;
                    break;
                case "Dungeon_3":
                    tilesetSpriteBundleName = CommonData.mapTileset_3_BundleName;
                    break;
                case "Dungeon_4":
                    tilesetSpriteBundleName = CommonData.mapTileset_4_BundleName;
                    break;
                case "Dungeon_5":
                    tilesetSpriteBundleName = CommonData.mapTileset_5_BundleName;
                    break;

            }
            return tilesetSpriteBundleName;

        }

		public List<Sprite> GetMapTileSpritesFrom(string mapTileName){

			string bundleName = GetMapTilesetSpriteBundleName(mapTileName);

			List<Sprite> mapTileList = new List<Sprite>();

			if(CommonData.mapTileset_1_BundleName != bundleName){
				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_1_BundleName,true);
			}
			if (CommonData.mapTileset_2_BundleName != bundleName)
            {
				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_2_BundleName, true);
            }
			if (CommonData.mapTileset_3_BundleName != bundleName)
            {
				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_3_BundleName, true);
            }
			if (CommonData.mapTileset_4_BundleName != bundleName)
            {
				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_4_BundleName, true);
            }
			if (CommonData.mapTileset_5_BundleName != bundleName)
            {
				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_5_BundleName, true);
            }

            
			Sprite[] mapTiles = MyResourceManager.Instance.LoadAssets<Sprite>(bundleName);

			for (int i = 0; i < mapTiles.Length;i++){
				mapTileList.Add(mapTiles[i]);
			}

			return mapTileList;         
		}

		public List<PlayRecord> allPlayRecords{
			get{
				if(mAllPlayRecords.Count == 0){
					LoadPlayRecords();
				}
				return mAllPlayRecords;
			}
		}

		private void LoadPlayRecords(){
			
			if(mAllPlayRecords.Count > 0){
				return;
			}

			PlayRecord[] playRecords = DataHandler.LoadDataToModelsWithPath<PlayRecord>(CommonData.playRecordsFilePath);

			for (int i = 0; i < playRecords.Length;i++){
				PlayRecord playRecord = playRecords[i];
				mAllPlayRecords.Add(playRecord);
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

		public List<HLHNPCChatRecord> chatRecords{

			get
			{
				if (mChatRecords.Count == 0)
				{
					LoadChatRecords();
				}

				return mChatRecords;
			}

		}

		private void LoadChatRecords(){

			if(!DataHandler.FileExist(CommonData.chatRecordsFilePath)){
				return;
			}

			HLHNPCChatRecord[] chatRecordsArray = DataHandler.LoadDataToModelsWithPath<HLHNPCChatRecord>(CommonData.chatRecordsFilePath);

			for (int i = 0; i < chatRecordsArray.Length;i++){

				mChatRecords.Add(chatRecordsArray[i]);

			}

		}


		public List<MapEventsRecord> mapEventsRecords{
			get{
				if(mMapEventsRecords.Count == 0){
					LoadMapEventsRecords();
				}
				return mMapEventsRecords;
			}
		}


		private void LoadMapEventsRecords(){
			
			if(mMapEventsRecords.Count > 0){
				return;
			}

			MapEventsRecord[] mapEventsRecordsArray = DataHandler.LoadDataToModelsWithPath<MapEventsRecord>(CommonData.mapEventsRecordFilePath);

			if(mapEventsRecordsArray.Length == 0){
				GameManager.Instance.persistDataManager.ResetMapEventsRecord();
				mapEventsRecordsArray = DataHandler.LoadDataToModelsWithPath<MapEventsRecord>(CommonData.mapEventsRecordFilePath);
			}

			for (int i = 0; i < mapEventsRecordsArray.Length; i++)
			{
				mMapEventsRecords.Add(mapEventsRecordsArray[i]);
			}

		}


		public CurrentMapEventsRecord currentMapEventsRecord{
			get{
				if(mCurrentMapEventsRecord == null){
					mCurrentMapEventsRecord = DataHandler.LoadDataToSingleModelWithPath<CurrentMapEventsRecord>(CommonData.currentMapEventsRecordFilePath);
				}
				return mCurrentMapEventsRecord;
			}
			set{
				mCurrentMapEventsRecord = value;
			}
		}


		public GameObject LoadMonster(string monsterName){

			GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allMonstersBundleName, monsterName);

			GameObject monster = GameObject.Instantiate (assets [0]);

			monster.name = assets [0].name;

			return monster;

		}

		public GameObject LoadMonsterUI(string monsterUIName){
			
			GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject>(CommonData.allMonstersUIBundleNAme, monsterUIName);

            GameObject monsterUI = GameObject.Instantiate(assets[0]);

            monsterUI.name = assets[0].name;

            return monsterUI;

		}

		public List<MonsterData> monsterDatas{
			get{
				if(mAllMonstersData.Count == 0){
					LoadMonstersData();
				}
				return mAllMonstersData;
			}
		}

		private void LoadMonstersData(){
			if(mAllMonstersData.Count > 0){
				return;
			}
			MonsterData[] mMonstersDataArray = DataHandler.LoadDataToModelsWithPath<MonsterData>(CommonData.monstersDataFilePath);
			for (int i = 0; i < mMonstersDataArray.Length;i++){
				mAllMonstersData.Add(mMonstersDataArray[i]);
			}
		}

		public List<EffectAnim> allEffects{
			get {
				if (mAllEffects.Count == 0) {
					LoadAllEffects ();
				}
				return mAllEffects;
			}
		}

		private void LoadAllEffects(){

			if (mAllEffects.Count > 0) {
				return;
			}

			GameObject[] effectCache = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allEffectsBundleName);

			Transform effectsContainer = TransformManager.FindOrCreateTransform ("AllEffects");

            effectsContainer.position = new Vector3(1000, 0, 0);

			for (int i = 0; i < effectCache.Length; i++) {
				GameObject effect = GameObject.Instantiate (effectCache [i]);
				effect.name = effectCache [i].name;
                effect.transform.SetParent (effectsContainer,false);
				mAllEffects.Add(effect.GetComponent<EffectAnim>());
			}
		}

		public void ResetGameData(){
			mChatRecords.Clear();
			mMapEventsRecords.Clear();
			mGameSettings = null;
              
		}


		public void ReleaseDataWithDataTypes(GameDataType[] dataTypes){

			for (int i = 0; i < dataTypes.Length; i++) {
				ReleaseDataWithName (dataTypes [i]);
			}

		}

		private void ReleaseDataWithName(GameDataType type){

			switch (type) {
    			case GameDataType.GameSettings:
    				mGameSettings = null;
    				break;
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
    				mAllPropertyGemstoneModels.Clear ();
    				break;
    			case GameDataType.SpecialItemModels:
    				mAllSpecialItemModels.Clear();
    				break;
    			case GameDataType.SpellItemModels:
    				mAllSpellItemModels.Clear();
    				break;
    			case GameDataType.SkillScrollModels:
                    mAllSkillScrollModels.Clear();
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
    				mAllPropertyGemstoneSprites.Clear ();
    				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allPropertyGemstoneSpritesBundleName,true);
    				break;
    			case GameDataType.SpecialItemSprites:
    				mAllSpecialItemSprites.Clear();
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.allSpecialItemSpritesBundleName, true);
    				break;
    			case GameDataType.SkillScrollSprites:
    				mAllSkillScrollSprites.Clear();
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.allSkillScrollSpritesBundleName, true);
    				break;
    			case GameDataType.MapSprites:
    				mAllMapSprites.Clear ();
    				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allMapSpritesBundleName,true);
    				break;
    			case GameDataType.MapTileAtlas:
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_1_BundleName, true);
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_2_BundleName, true);
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_3_BundleName, true);
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_4_BundleName, true);
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.mapTileset_5_BundleName, true);
    				break;
    			case GameDataType.CharacterSprites:
    				mAllCharacterSprites.Clear();
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.allCharacterSpritesBundleName, true);
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
				case GameDataType.MiniMapNpcSprites:
					mAllMinimapNpcSprites.Clear();
					MyResourceManager.Instance.UnloadAssetBundle(CommonData.allMinimapNpcSpritesBundleName, true);
					break;
    			case GameDataType.Monsters:
    				MyResourceManager.Instance.UnloadAssetBundle (CommonData.allMonstersBundleName,true);
    				break;
    			case GameDataType.NPCs:
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.allMapNpcBundleName, true);
    				break;
    			case GameDataType.Effects:
    				mAllEffects.Clear();
    				TransformManager.DestroyTransfromWithName("AllEffects");
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.allEffectsBundleName, true);
    				break;
    			case GameDataType.Diary:
    				mAllDiaryModels.Clear();
    				break;
    			case GameDataType.Proverbs:
    				mAllProverbs.Clear();
    				break;
				case GameDataType.PlayRecord:
					mAllPlayRecords.Clear();
					break;
				case GameDataType.ChatRecord:
					mChatRecords.Clear();
					break;
    			case GameDataType.BagCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("BagCanvas");
    				break;
    			case GameDataType.SettingCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("SettingCanvas");
    				break;
    			case GameDataType.NPCCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("NPCCanvas");
    				break;
    			case GameDataType.LoadingCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("LoadingCanvas");               
    				break;
    			case GameDataType.GuideCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("GuideCanvas");
    				break;
    			case GameDataType.RecordCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("RecordCanvas");
    				break;
    			case GameDataType.HomeCanvas:
    				GameManager.Instance.UIManager.RemoveCanvasCache("HomeCanvas");
    				break;
    			case GameDataType.ExploreScene:
    				GameManager.Instance.UIManager.RemoveCanvasCache("ExploreCanvas");
    				MyResourceManager.Instance.UnloadAssetBundle(CommonData.exploreSceneBundleName, true);
    				break;
				case GameDataType.PlayRecordCanvas:
					GameManager.Instance.UIManager.RemoveCanvasCache("PlayRecordCanvas");
					break;

			}
		}



	}
}
