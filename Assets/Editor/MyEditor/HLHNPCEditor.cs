using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;
	using System.IO;
	using System.Text;


	public class HLHNPCEditor : EditorWindow {


		private static HLHNPCEditor mInstance;

		public static HLHNPCEditor npcEditor{

			get{
				if (mInstance == null) {
					mInstance = ScriptableObject.CreateInstance<HLHNPCEditor> ();
					mInstance.ShowUtility ();
				}

				return mInstance;
			}


		}
			
		private Vector2 scrollPos;

		private string npcDataPath;

		private Rect rect;

		private HLHNPC npc;

		private GUILayoutOption[] seperatorLayouts = new GUILayoutOption[]{ GUILayout.Height(10), GUILayout.Width(800)};

		private GUILayoutOption[] singleIntLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(20)};

		private GUILayoutOption[] tinyLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(50)};

		private GUILayoutOption[] shorterLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(100)};

		private GUILayoutOption[] shortLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(200)};

		private GUILayoutOption[] middleLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(300)};

		private GUILayoutOption[] longLayouts = new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(1000)};

		private GUILayoutOption[] buttonLayouts = new GUILayoutOption[]{ GUILayout.Height (20), GUILayout.Width (200) };

		private GUIStyle style = new GUIStyle();

        
		private bool[] levelDefiniteDialogGroupFoldOutInfoArray;

		private bool[,] levelDefiniteDialogFoldOutInfoArray;

		private bool[] levelUndefiniteDialogGroupFoldOutInfoArray;

        private bool[,] levelUndefiniteDialogFoldOutInfoArray;

		private bool[] goodsFoldOutInfoArray;

		private bool[] regularGreetingDialogFoldOutInfoArray;

		//private bool[,] wordRightDialogFoldoutInfoArray;
		//private bool[,] wordRightChoiceFoldoutInfoArray;

		//private bool[,] wordWrongDialogFoldoutInfoArray;
		//private bool[,] wordWrongChoiceFoldoutInfoArray;

		private int dataResult = -1;


		[MenuItem("EditHelper/HLHNPCEditor")]
		public static void InitHLHNPCEditor(){

			HLHNPCEditor editor = HLHNPCEditor.npcEditor;
			editor.npc = new HLHNPC ();

			editor.levelDefiniteDialogGroupFoldOutInfoArray = new bool[50];
			editor.levelDefiniteDialogFoldOutInfoArray = new bool[50, 50];

			editor.levelUndefiniteDialogGroupFoldOutInfoArray = new bool[50];
            editor.levelUndefiniteDialogFoldOutInfoArray = new bool[50, 50];         
			editor.goodsFoldOutInfoArray = new bool[10];
			editor.regularGreetingDialogFoldOutInfoArray = new bool[50]; 
			editor.style.richText = true;

		}



		public void OnGUI(){

			Rect windowRect = npcEditor.position;

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos,new GUILayoutOption[]{
				GUILayout.Height(windowRect.height),
				GUILayout.Width(windowRect.width)
			});

			EditorGUILayout.LabelField ("please input information of npc");

			EditorGUILayout.LabelField ("npc数据源",longLayouts);

			EditorGUILayout.BeginHorizontal ();

			rect = EditorGUILayout.GetControlRect(false,GUILayout.Width(300));

			npcDataPath = EditorGUI.TextField(rect, npcDataPath);

			switch (dataResult) {
			case -1:
				EditorGUILayout.LabelField ("加载失败,目标文件不存在", shortLayouts);
				break;
			case 0:
				EditorGUILayout.LabelField ("加载完成", shortLayouts);
				break;
			case 1:
				EditorGUILayout.LabelField ("保存完成", shortLayouts);
				break;
			case -2:
				EditorGUILayout.LabelField ("保存失败", shortLayouts);
				break;

			}

			EditorGUILayout.EndHorizontal ();

			//如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内
			if ((UnityEngine.Event.current.type == UnityEngine.EventType.DragUpdated
				|| UnityEngine.Event.current.type == UnityEngine.EventType.DragExited)
				&& rect.Contains (UnityEngine.Event.current.mousePosition)) {
				//改变鼠标的外表
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {
					npcDataPath = DragAndDrop.paths [0];
				}
			}
				
			bool loadNpcData = GUILayout.Button ("加载npc数据", buttonLayouts);
			bool saveNpcData = GUILayout.Button ("保存npc数据", buttonLayouts);



			if (loadNpcData) {
				if (File.Exists (npcDataPath)) {
					string npcData = File.ReadAllText (npcDataPath);
					if (npcData == string.Empty) {
						npc = new HLHNPC ();
						//npc.monsterData = new MonsterData ();
//						npc.regularGreeting = new HLHDialogGroup ();
						for (int i = 0; i < 10; i++) {
							HLHNPCGoodsGroup gg = new HLHNPCGoodsGroup ();
							npc.npcGoodsGroupList.Add (gg);
						}
					} else {
						npc = JsonUtility.FromJson<HLHNPC> (npcData);
					}
					dataResult = 0;
				} else {
					dataResult = -1;
				}
			}

			if (saveNpcData) {

				if (File.Exists (npcDataPath)) {
					string npcData = JsonUtility.ToJson (npc);

					File.WriteAllText (npcDataPath, npcData);

					dataResult = 1;
				} else {
					dataResult = -2;
				}
			}

			npc.npcName = EditorGUILayout.TextField ("npc名称:", npc.npcName,middleLayouts);

			npc.npcId = EditorGUILayout.IntField ("npc ID:", npc.npcId,shortLayouts);

			npc.saveOnChange = EditorGUILayout.Toggle ("数据变化时是否保存", npc.saveOnChange, shortLayouts);

			npc.isChatTriggered = EditorGUILayout.Toggle("触发聊天", npc.isChatTriggered, shortLayouts);

			if (npc.isChatTriggered)
            {
				DrawRegularDialogGroups(npc.levelDefiniteDialogGroups,levelDefiniteDialogGroupFoldOutInfoArray,levelDefiniteDialogFoldOutInfoArray);
				DrawRegularDialogGroups(npc.levelUndefiniteDialogGroups,levelUndefiniteDialogGroupFoldOutInfoArray,levelUndefiniteDialogFoldOutInfoArray);
            }

			npc.isTradeTriggered = EditorGUILayout.Toggle("触发交易", npc.isTradeTriggered, shortLayouts);


			if (npc.isTradeTriggered)
            {
                DrawGoods();
            }

			npc.isTransportTriggered = EditorGUILayout.Toggle("触发传送", npc.isTransportTriggered, shortLayouts);


            if (npc.isTransportTriggered)
            {

                List<int> transportLevelList = npc.transportLevelList;

                EditorGUILayout.LabelField("编辑在当前npc处可以传送到的关卡id列表", longLayouts);

                EditorGUILayout.BeginHorizontal();
                bool addNewLevel = GUILayout.Button("添加关卡", buttonLayouts);
                bool removeLastLevel = GUILayout.Button("删除尾部关卡", buttonLayouts);
                EditorGUILayout.EndHorizontal();

                if (addNewLevel)
                {
                    transportLevelList.Add(0);
                }

                if (removeLastLevel && transportLevelList.Count > 0)
                {
                    transportLevelList.RemoveAt(transportLevelList.Count - 1);
                }

               
                for (int i = 0; i < transportLevelList.Count; i++)
                {
                    transportLevelList[i] = EditorGUILayout.IntField("关卡id", transportLevelList[i], shortLayouts);
                }
                

                EditorGUILayout.LabelField("================================================================", seperatorLayouts);

            }



			npc.isLearnSkillTriggered = EditorGUILayout.Toggle("触发学习技能", npc.isLearnSkillTriggered, shortLayouts);

			if (npc.isLearnSkillTriggered)
            {

                List<int> npcSkillIds = npc.npcSkillIds;
                            

                if (npcSkillIds == null || npcSkillIds.Count == 0)
                {
                    npcSkillIds.Add(0);
                    npcSkillIds.Add(0);
                    npcSkillIds.Add(0);
					npcSkillIds.Add(0);
                }

				if(npcSkillIds.Count == 3){
					npcSkillIds.Add(0);
				}

                EditorGUILayout.LabelField("编辑可以在当前npc处购买学习的技能id数组", longLayouts);

                npcSkillIds[0] = EditorGUILayout.IntField("技能1：", npcSkillIds[0], shortLayouts);

                npcSkillIds[1] = EditorGUILayout.IntField("技能2：", npcSkillIds[1], shortLayouts);

                npcSkillIds[2] = EditorGUILayout.IntField("技能3：", npcSkillIds[2], shortLayouts);
            
				npcSkillIds[3] = EditorGUILayout.IntField("技能4：", npcSkillIds[3], shortLayouts);


                EditorGUILayout.LabelField("================================================================", seperatorLayouts);

            }


			npc.isPropertyPromotionTriggered = EditorGUILayout.Toggle("触发属性提升", npc.isPropertyPromotionTriggered, shortLayouts);

			if (npc.isPropertyPromotionTriggered)
            {

                List<HLHPropertyPromotion> propertyPromotions = npc.propertyPromotionList;

                EditorGUILayout.LabelField("================================================================", seperatorLayouts);

                EditorGUILayout.LabelField("编辑可以在当前npc处提升的属性列表 注：闪避，暴击，暴击倍率使用x1000以后的数值", longLayouts);
				EditorGUILayout.LabelField("0:最大生命 1：最大魔法 2:物攻 3:魔攻 4:移速 5:护甲 6:抗性 7:护甲穿透 8:抗性穿透 9:暴击 10:闪避 11:暴击倍率 12:物伤倍率 13:法伤倍率 14:额外金钱 15:额外经验 16:生命回复 17:魔法回复", longLayouts);

                EditorGUILayout.BeginHorizontal();

                bool addPropertyPromotion = GUILayout.Button("添加属性", buttonLayouts);
                bool removeLastPropertyPromotion = GUILayout.Button("移除尾部属性", buttonLayouts);

                EditorGUILayout.EndHorizontal();

                if (addPropertyPromotion)
                {

                    propertyPromotions.Add(new HLHPropertyPromotion());
                }

                if (removeLastPropertyPromotion && propertyPromotions.Count > 0)
                {

                    propertyPromotions.RemoveAt(propertyPromotions.Count - 1);
                }


                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < propertyPromotions.Count; i++)
                {
                    EditorGUILayout.BeginVertical();

                    HLHPropertyPromotion propertyPromotion = propertyPromotions[i];

                    PropertyType propertyType = (PropertyType)(EditorGUILayout.IntField("属性类型：", (int)(propertyPromotion.propertyType), shortLayouts));

                    int promotion = EditorGUILayout.IntField("提升值：", propertyPromotion.promotion, shortLayouts);

                    int promotionPrice = EditorGUILayout.IntField("提升价格：", propertyPromotion.promotionPrice, shortLayouts);

                    HLHPropertyPromotion newPropertyPromotion = new HLHPropertyPromotion(propertyType, promotion, promotionPrice);

                    propertyPromotions[i] = newPropertyPromotion;

                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("================================================================", seperatorLayouts);

            }

			npc.isAddGemStoneTriggered = EditorGUILayout.Toggle("触发镶嵌宝石", npc.isAddGemStoneTriggered, shortLayouts);         
  
			DrawRegularGreetings ();
         
			EditorGUILayout.EndScrollView ();
		}


		private void DrawRegularGreetings (){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑寒暄", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			bool createNewDialog = GUILayout.Button ("添加新的寒暄",buttonLayouts);
			bool removeLastDialog = GUILayout.Button ("删除尾部寒暄",buttonLayouts);
			bool unfoldAll = GUILayout.Button("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (createNewDialog) {
				HLHDialog dg = new HLHDialog ();
				//dg.dialogs = new List<HLHDialog> ();
				//dg.choices = new List<HLHChoice> ();
				npc.regularGreetings.Add (dg);

			}

			if (removeLastDialog && npc.regularGreetings.Count > 0) {
				npc.regularGreetings.RemoveAt (npc.regularGreetings.Count - 1);
			}


			if(unfoldAll){
				for (int i = 0; i < regularGreetingDialogFoldOutInfoArray.Length;i++){
					regularGreetingDialogFoldOutInfoArray[i] = true;
				}
			}

			if(foldAll){
				for (int i = 0; i < regularGreetingDialogFoldOutInfoArray.Length;i++){
					regularGreetingDialogFoldOutInfoArray[i] = false;
				}

			}

			for (int i = 0; i < npc.regularGreetings.Count; i++) {

				regularGreetingDialogFoldOutInfoArray [i] = EditorGUILayout.Foldout (regularGreetingDialogFoldOutInfoArray [i], "npc寒暄:" + npc.regularGreetings[i].dialogContent);


				if (regularGreetingDialogFoldOutInfoArray [i]) {
                    
					HLHDialog dialog = npc.regularGreetings [i];

					dialog.dialogId = i;
                                   
					DrawGreeting (dialog);
				}
			}


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);

//			EditorGUILayout.BeginHorizontal ();
//			EditorGUILayout.LabelField ("***编辑常规寒暄对话", new GUILayoutOption[] {
//				GUILayout.Height (20),
//				GUILayout.Width (120)
//			});
//			EditorGUILayout.BeginVertical ();
//
//			if (npc.regularGreeting == null) {
//				npc.regularGreeting = new HLHDialogGroup ();
//			}
//
//			HLHDialogGroup dg = npc.regularGreeting;
//
//			dg.dialogGroupId = -1;
//
//			dg.isTaskTriggeredDg = false;
//
//			dg.triggerLevel = -1;
//
//			DrawDialogs (dg,regularGreetingDialogFoldOutInfoArray);
//
//			DrawChoices (dg,regularGreetingChoiceFoldOutInfoArray);
//				
//
//			EditorGUILayout.EndVertical ();
//			EditorGUILayout.EndHorizontal ();
//
//			EditorGUILayout.Separator ();
//			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);

		}

        

		private void DrawRegularDialogGroups(List<HLHDialogGroup> dialogGroups,bool[] dialogGroupFoldOutArray, bool[,] dialogsFoldOutArray){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑对话组", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});
			EditorGUILayout.BeginVertical ();



			EditorGUILayout.BeginHorizontal ();
			bool createNewDialogGroup = GUILayout.Button ("添加新的对话组",buttonLayouts);
			bool removeLastDialogGroup = GUILayout.Button ("删除尾部对话组",buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (createNewDialogGroup) {
				HLHDialogGroup dg = new HLHDialogGroup ();
				dg.dialogs = new List<HLHDialog> ();
				//dg.choices = new List<HLHChoice> ();
				dialogGroups.Add (dg);

			}
			if (removeLastDialogGroup && dialogGroups.Count > 0) {
				dialogGroups.RemoveAt (dialogGroups.Count - 1);
			}


			for (int i = 0; i < dialogGroups.Count; i++) {

				dialogGroupFoldOutArray [i] = EditorGUILayout.Foldout (dialogGroupFoldOutArray [i], "npc对话组   情况" + (i+1).ToString() + "   对话组ID:" + i.ToString());


				if (dialogGroupFoldOutArray [i]) {

					HLHDialogGroup dg = dialogGroups [i];

					dg.dialogGroupId = i;

					dg.triggerLevel = EditorGUILayout.IntField ("触发关卡", dg.triggerLevel, middleLayouts);


					dg.isMultiTimes = EditorGUILayout.ToggleLeft("是否可反复对话", dg.isMultiTimes, shorterLayouts);

					dg.isRewardTriggered = EditorGUILayout.ToggleLeft("触发奖励", dg.isRewardTriggered, shorterLayouts);

					if(dg.isRewardTriggered){


						EditorGUILayout.BeginHorizontal();
						HLHRewardType rewardType = (HLHRewardType)EditorGUILayout.IntField("奖励类型", (int)dg.reward.rewardType, shortLayouts);
						EditorGUILayout.LabelField("0:奖励金币 1:奖励物品", middleLayouts);
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						int rewardValue = EditorGUILayout.IntField("奖励数据", dg.reward.rewardValue, shortLayouts);
						EditorGUILayout.LabelField("奖励金币时为金币数量，奖励物品时为物品id", middleLayouts);
                        EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						int attachValue = EditorGUILayout.IntField("附加信息", dg.reward.attachValue, shortLayouts);
						EditorGUILayout.LabelField("奖励物品时为物品数量", middleLayouts);
                        EditorGUILayout.EndHorizontal();

						dg.reward = new HLHNPCReward(rewardType, rewardValue, attachValue);

					}

					DrawDialogs (dg,dialogsFoldOutArray);


				}
			}
				

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);

		}

		private void DrawGoods(){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑商品信息", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < goodsFoldOutInfoArray.Length; i++) {
					goodsFoldOutInfoArray [i] = true;
				}
			}
			if (foldAll) {
				for (int i = 0; i < goodsFoldOutInfoArray.Length; i++) {
					goodsFoldOutInfoArray [i] = false;
				}
			}

			if (npc.npcGoodsGroupList.Count < 10) {
				npc.npcGoodsGroupList.Clear ();
				for (int i = 0; i < 10; i++) {
					HLHNPCGoodsGroup gg = new HLHNPCGoodsGroup ();
					npc.npcGoodsGroupList.Add (gg);
				}
			}

			for(int i = 0;i<10;i++){
				goodsFoldOutInfoArray [i] = EditorGUILayout.Foldout (goodsFoldOutInfoArray [i], string.Format ("{0}-{1}层商品信息", i * 5, (i + 1) * 5));

				if (goodsFoldOutInfoArray [i]) {

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("\t\t\t\t", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (20)
					});

					EditorGUILayout.BeginVertical ();

					HLHNPCGoodsGroup gg = npc.npcGoodsGroupList [i];

					EditorGUILayout.BeginHorizontal ();

					ShowGoods (gg.goodsList_1);
					ShowGoods (gg.goodsList_2);
					ShowGoods (gg.goodsList_3);
					ShowGoods (gg.goodsList_4);
					ShowGoods (gg.goodsList_5);

					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				}


			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);


		}

		private void ShowGoods(List<HLHNPCGoods> goodsList){

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			bool addNewGoods = GUILayout.Button ("添加商品", shorterLayouts);
			bool removeLastGoods = GUILayout.Button ("删除尾部商品", shorterLayouts);
			EditorGUILayout.EndHorizontal ();

			if (addNewGoods) {
				goodsList.Add (new HLHNPCGoods ());
			}

			if (removeLastGoods && goodsList.Count > 0) {
				goodsList.RemoveAt (goodsList.Count - 1);
			}

			for (int i = 0; i < goodsList.Count; i++) {

				HLHNPCGoods goods = goodsList [i];

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("商品id", tinyLayouts);
				goods.goodsId = EditorGUILayout.IntField (goods.goodsId, tinyLayouts);
				EditorGUILayout.LabelField ("价格浮动", tinyLayouts);
				goods.priceFloat = EditorGUILayout.FloatField ( goods.priceFloat, tinyLayouts);
				EditorGUILayout.LabelField ("装备品质", tinyLayouts);
				goods.equipmentQuality = EditorGUILayout.IntField (goods.equipmentQuality, singleIntLayouts);
				EditorGUILayout.EndHorizontal ();

			}

			EditorGUILayout.EndVertical ();

		}

	


		private void DrawGreeting(HLHDialog dialog)
        {
                          
			dialog.dialogId = 0;
           

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("\t\t\t\t\t", new GUILayoutOption[] {
                GUILayout.Height (20),
                GUILayout.Width (20)
            });
            EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField("对话ID", dialog.dialogId.ToString(), shortLayouts);

			dialog.dialogContent = EditorGUILayout.TextField("对话内容", dialog.dialogContent, longLayouts);
            
                        
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
                     
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("================================================================", seperatorLayouts);
        }

      
		private void DrawDialogs(HLHDialogGroup dg,bool[,] dgFoldoutInfoArray){
			
			if (dg.dialogs == null) {
				dg.dialogs = new List<HLHDialog> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑对话数组", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});
			EditorGUILayout.BeginVertical ();

			//dg.isMultiOff = EditorGUILayout.Toggle ("是否可以反复触发", dg.isMultiOff, middleLayouts);

			EditorGUILayout.BeginHorizontal ();
			bool createNewDialog = GUILayout.Button ("添加新的对话",buttonLayouts);
			bool removeLastDialog = GUILayout.Button ("删除尾部对话",buttonLayouts);
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < dgFoldoutInfoArray.GetLength(1); i++) {
					dgFoldoutInfoArray[dg.dialogGroupId,i] = true;
				}
			}

			if (foldAll) {
				for (int i = 0; i < dgFoldoutInfoArray.GetLength(1); i++) {
					dgFoldoutInfoArray[dg.dialogGroupId,i] = false;
				}
			}


			if (createNewDialog) {
				HLHDialog d = new HLHDialog ();
				dg.dialogs.Add (d);
			}
			if (removeLastDialog && dg.dialogs.Count > 0) {
				dg.dialogs.RemoveAt (dg.dialogs.Count - 1);
			}

			for (int j = 0; j < dg.dialogs.Count; j++) {

				HLHDialog d = dg.dialogs [j];

				d.dialogId = j;

				string foldContent = string.Format ("编辑对话	\t\t***对话ID:   {0}\t\t\t***对话内容:   {1}", d.dialogId, d.dialogContent);

				dgFoldoutInfoArray [dg.dialogGroupId, j] = EditorGUILayout.Foldout (dgFoldoutInfoArray [dg.dialogGroupId, j], foldContent);

				if (dgFoldoutInfoArray [dg.dialogGroupId, j]) {

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("\t\t\t\t\t", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (20)
					});
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.LabelField ("对话ID", d.dialogId.ToString(), shortLayouts);

					d.dialogContent = EditorGUILayout.TextField ("对话内容", d.dialogContent, longLayouts);

					if (d.choiceIds == null) {
						d.choiceIds = new List<int> ();
					}
                                   
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();

				}

			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
		}
			
	
	}

}
