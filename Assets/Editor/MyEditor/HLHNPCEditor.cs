using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;
	using System.IO;


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

		private GUILayoutOption[] tinyLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(50)};

		private GUILayoutOption[] shorterLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(100)};

		private GUILayoutOption[] shortLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(200)};

		private GUILayoutOption[] middleLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(300)};

		private GUILayoutOption[] longLayouts = new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(1000)};

		private GUILayoutOption[] buttonLayouts = new GUILayoutOption[]{ GUILayout.Height (20), GUILayout.Width (200) };

		private GUIStyle style = new GUIStyle();


//		private Dictionary<string,bool> foldOutInfoDic = new Dictionary<string, bool> () {
//			{"dialogGroups",false},
//			{"taskList",false},
//			{"npcGoodsList",false},
//			{"regularGreetings",false}
//
//		};

		private bool[] dialogGroupFoldOutInfoArray;

		private bool[,] dialogFoldOutInfoArray;

		private bool[,] choiceFoldOutInfoArray;

		private bool[] taskFoldOutInfoArray;

		private bool[] goodsFoldOutInfoArray;

		private bool[] regularGreetingInfoArray;

		private bool[] triggerConditionInfoArray;

		private int dataResult = -1;


		[MenuItem("EditHelper/HLHNPCEditor")]
		public static void InitHLHNPCEditor(){

			HLHNPCEditor editor = HLHNPCEditor.npcEditor;
			editor.npc = new HLHNPC ();

			editor.dialogFoldOutInfoArray = new bool[50, 50];
			editor.choiceFoldOutInfoArray = new bool[50, 50];
			editor.taskFoldOutInfoArray = new bool[50];
			editor.goodsFoldOutInfoArray = new bool[50];
			editor.regularGreetingInfoArray = new bool[50];
			editor.dialogGroupFoldOutInfoArray = new bool[50];
			editor.triggerConditionInfoArray = new bool[50];

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
//					npc = DataHandler.LoadDataToSingleModelWithPath<HLHNPC> (npcDataPath);
					string npcData = File.ReadAllText (npcDataPath);
					if (npcData == string.Empty) {
						npc = new HLHNPC ();
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


			DrawDialogGroups ();

			DrawTasks ();

			DrawGoods ();

			DrawRegularGreetings ();


			EditorGUILayout.EndScrollView ();
		}


		private void DrawRegularGreetings (){

			if (npc.regularGreetings == null) {
				npc.regularGreetings = new List<HLHDialogGroup> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();


			EditorGUILayout.BeginHorizontal ();
			bool createNewRegularGreeting = GUILayout.Button ("添加新的寒暄",buttonLayouts);
			bool removeLastRegularGreeting = GUILayout.Button ("删除尾部寒暄",buttonLayouts);
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < regularGreetingInfoArray.Length; i++) {
					regularGreetingInfoArray[i] = true;
				}
			}

			if (foldAll) {
				for (int i = 0; i < regularGreetingInfoArray.Length; i++) {
					regularGreetingInfoArray[i] = false;
				}
			}


			if (createNewRegularGreeting) {
				HLHDialogGroup greeting = new HLHDialogGroup ();
				npc.regularGreetings.Add (greeting);
			}
			if (removeLastRegularGreeting && npc.regularGreetings.Count > 0) {
				npc.regularGreetings.RemoveAt (npc.regularGreetings.Count - 1);
			}


			for (int i = 0; i < npc.regularGreetings.Count; i++) {
				
				int specialDgsCount = npc.dialogGroups.Count;
				regularGreetingInfoArray [i] = EditorGUILayout.Foldout (regularGreetingInfoArray [i], "寒暄" + (i+1).ToString() + "  对话组ID" + (specialDgsCount + i).ToString());

				if (regularGreetingInfoArray [i]) {

					HLHDialogGroup rdg = npc.regularGreetings [i];

					rdg.dialogGroupId = i + specialDgsCount;

					DrawTriggerConditions (rdg);

					DrawDialogs (rdg);

					DrawChoices (rdg);

				}

			}


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);



		}



		private void DrawDialogGroups(){

//			EditorGUILayout.Separator ();
//			EditorGUILayout.LabelField ("************************对话组编辑区*************************", new GUILayoutOption[] {
//				GUILayout.Height (20),
//				GUILayout.Width (600)
//			});
//			EditorGUILayout.Separator ();

			if (npc.dialogGroups == null) {
				npc.dialogGroups = new List<HLHDialogGroup> ();
			}


			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();



			EditorGUILayout.BeginHorizontal ();
			bool createNewDialogGroup = GUILayout.Button ("添加新的对话组",buttonLayouts);
			bool removeLastDialogGroup = GUILayout.Button ("删除尾部对话组",buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (createNewDialogGroup) {
				HLHDialogGroup dg = new HLHDialogGroup ();
				dg.dialogs = new List<HLHDialog> ();
				dg.choices = new List<HLHChoice> ();
				npc.dialogGroups.Add (dg);

			}
			if (removeLastDialogGroup && npc.dialogGroups.Count > 0) {
				npc.dialogGroups.RemoveAt (npc.dialogGroups.Count - 1);
			}


			for (int i = 0; i < npc.dialogGroups.Count; i++) {

				dialogGroupFoldOutInfoArray [i] = EditorGUILayout.Foldout (dialogGroupFoldOutInfoArray [i], "npc对话组   情况" + (i+1).ToString() + "   对话组ID:" + i.ToString());


				if (dialogGroupFoldOutInfoArray [i]) {

					HLHDialogGroup dg = npc.dialogGroups [i];

					dg.dialogGroupId = i;

					dg.isMultiTimes = EditorGUILayout.Toggle ("是否是可重复型的对话", dg.isMultiTimes, shortLayouts);

					dg.isTaskTriggeredDg = EditorGUILayout.Toggle ("是否任务触发的对话", dg.isTaskTriggeredDg, shortLayouts);

//					EditorGUILayout.LabelField ("对话组ID", i.ToString(), shortLayouts);

					DrawTriggerConditions (dg);

					DrawDialogs (dg);

					DrawChoices (dg);


				}
			}
				

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);

		}

		private void DrawGoods(){

			if (npc.npcGoodsList == null) {
				npc.npcGoodsList = new List<HLHNPCGoods> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();


			EditorGUILayout.BeginHorizontal ();
			bool createNewGoods = GUILayout.Button ("添加新的商品",buttonLayouts);
			bool removeLastGoods = GUILayout.Button ("删除尾部商品",buttonLayouts);
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < goodsFoldOutInfoArray.Length; i++) {
					goodsFoldOutInfoArray[i] = true;
				}
			}

			if (foldAll) {
				for (int i = 0; i < goodsFoldOutInfoArray.Length; i++) {
					goodsFoldOutInfoArray[i] = false;
				}
			}


			if (createNewGoods) {
				HLHNPCGoods goods = new HLHNPCGoods ();
				npc.npcGoodsList.Add (goods);
			}
			if (removeLastGoods && npc.npcGoodsList.Count > 0) {
				npc.npcGoodsList.RemoveAt (npc.npcGoodsList.Count - 1);
			}


			for (int i = 0; i < npc.npcGoodsList.Count; i++) {

				goodsFoldOutInfoArray [i] = EditorGUILayout.Foldout (goodsFoldOutInfoArray [i], "编辑商品");

				if (goodsFoldOutInfoArray [i]) {

					HLHNPCGoods goods = npc.npcGoodsList [i];

					goods.goodsId = EditorGUILayout.IntField ("商品 ID", goods.goodsId, shortLayouts);

//					goods.goodsPrice = EditorGUILayout.IntField ("商品价格", goods.goodsPrice, shortLayouts);

					goods.totalCount = EditorGUILayout.IntField ("商品总数量", goods.totalCount, shortLayouts);

					goods.isFixedCount = EditorGUILayout.Toggle ("售出后数量是否会减少", goods.isFixedCount, shortLayouts);
				}

			}


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);


		}

		private void DrawTasks(){

			if (npc.taskList == null) {
				npc.taskList = new List<HLHTask> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();


			EditorGUILayout.BeginHorizontal ();
			bool createNewTask = GUILayout.Button ("添加新的任务",buttonLayouts);
			bool removeLastTask = GUILayout.Button ("删除尾部任务",buttonLayouts);
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < taskFoldOutInfoArray.Length; i++) {
					taskFoldOutInfoArray[i] = true;
				}
			}

			if (foldAll) {
				for (int i = 0; i < taskFoldOutInfoArray.Length; i++) {
					taskFoldOutInfoArray[i] = false;
				}
			}


			if (createNewTask) {
				HLHTask task = new HLHTask ();
				npc.taskList.Add (task);
			}
			if (removeLastTask && npc.taskList.Count > 0) {
				npc.taskList.RemoveAt (npc.taskList.Count - 1);
			}


			for (int i = 0; i < npc.taskList.Count; i++) {

				taskFoldOutInfoArray [i] = EditorGUILayout.Foldout (taskFoldOutInfoArray [i], "编辑任务");

				if (taskFoldOutInfoArray [i]) {

					HLHTask task = npc.taskList [i];

					int npcId = EditorGUILayout.IntField ("npc ID", task.npcId, shortLayouts);

					EditorGUILayout.LabelField ("任务ID", i.ToString(), shortLayouts);

					int taskItemId = EditorGUILayout.IntField ("任务物品ID", task.taskItemId, shortLayouts);

					int taskItemCount = EditorGUILayout.IntField ("任务物品数量", task.taskItemCount, shortLayouts);

					int dialogGroupId = EditorGUILayout.IntField ("完成任务触发的对话组ID", task.dialogGroupId, shortLayouts);

					string taskDescription = EditorGUILayout.TextField ("任务描述", task.taskDescription, longLayouts);

					bool isCurrentLevelTask = EditorGUILayout.Toggle ("是否是本层任务", task.isCurrentLevelTask, shortLayouts);

					HLHTask newTask = new HLHTask (npcId, i, taskItemId, taskItemCount, dialogGroupId, taskDescription,isCurrentLevelTask);

					npc.taskList [i] = newTask;
				}

			}


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);

		}

		private void DrawChoices(HLHDialogGroup dg){

			if (dg.choices == null) {
				dg.choices = new List<HLHChoice> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			bool createNewChoice = GUILayout.Button ("添加新的选择",buttonLayouts);
			bool removeLastChoice = GUILayout.Button ("删除尾部选择",buttonLayouts);
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < choiceFoldOutInfoArray.GetLength(1); i++) {
					choiceFoldOutInfoArray[dg.dialogGroupId,i] = true;
				}
			}

			if (foldAll) {
				for (int i = 0; i < choiceFoldOutInfoArray.GetLength(1); i++) {
					choiceFoldOutInfoArray[dg.dialogGroupId,i] = false;
				}
			}


			if (createNewChoice) {
				HLHChoice c = new HLHChoice ();
				dg.choices.Add (c);
			}
			if (removeLastChoice && dg.choices.Count > 0) {
				dg.choices.RemoveAt (dg.choices.Count - 1);
			}

			for (int j = 0; j < dg.choices.Count; j++) {

				choiceFoldOutInfoArray [dg.dialogGroupId, j] = EditorGUILayout.Foldout (choiceFoldOutInfoArray [dg.dialogGroupId, j], "编辑选择");

				if (choiceFoldOutInfoArray [dg.dialogGroupId, j]) {

					HLHChoice c = dg.choices [j];

					c.choiceId = EditorGUILayout.IntField ("选择ID",c.choiceId, shortLayouts);

					c.choiceContent = EditorGUILayout.TextField ("选择的内容", c.choiceContent, longLayouts);

					c.triggerDialogId = EditorGUILayout.IntField ("npc下一句话id", c.triggerDialogId, shortLayouts);

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("触发事件类型:", shortLayouts);

					EditorGUILayout.BeginHorizontal ();
					c.isHiddenPropertyChangeTriggered = EditorGUILayout.Toggle ("\t    隐藏属性变化", c.isHiddenPropertyChangeTriggered, shortLayouts);
					c.isReceiveTaskTriggered = EditorGUILayout.Toggle ("\t\t    接受任务", c.isReceiveTaskTriggered, shortLayouts);
					c.isHandInTaskTriggered = EditorGUILayout.Toggle ("\t\t    提交任务", c.isHandInTaskTriggered, shortLayouts);
					c.isRewardTriggered = EditorGUILayout.Toggle ("\t\t    奖励", c.isRewardTriggered, shortLayouts);
					c.isTradeTriggered = EditorGUILayout.Toggle ("\t\t    交易", c.isTradeTriggered, shortLayouts);
					c.isAddSkillTriggered = EditorGUILayout.Toggle ("\t\t    添加技能", c.isAddSkillTriggered, shortLayouts);
					c.isFightTriggered = EditorGUILayout.Toggle ("\t\t    战斗", c.isFightTriggered, shortLayouts);

					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("隐藏属性变化",shortLayouts);
					c.playerJusticeChange = EditorGUILayout.IntField ("正义度", c.playerJusticeChange, shortLayouts);
					c.playerPowerChange = EditorGUILayout.IntField ("强大度", c.playerPowerChange, shortLayouts);
					EditorGUILayout.EndHorizontal ();

	
					EditorGUILayout.LabelField ("可能的奖励(属性 0:最大血量，1:最大魔法，2:攻击，3:魔法攻击，4:移速，5:护甲，6:抗性，" +
						"7:暴击，8:闪避，9:暴击倍率，10:额外金钱，11:额外经验，12:生命回复，13:魔法回复)", longLayouts);
					EditorGUILayout.BeginHorizontal ();


					if (c.possibleRewards == null) {
						c.possibleRewards = new List<HLHNPCReward> ();
					}

					for (int k = 0; k < c.possibleRewards.Count; k++) {
						HLHNPCReward reward = c.possibleRewards [k];
						HLHRewardType rewardType = (HLHRewardType)EditorGUILayout.EnumPopup (reward.rewardType, tinyLayouts);
						int rewardValue = EditorGUILayout.IntField (reward.rewardValue, tinyLayouts);
						int attachValue = EditorGUILayout.IntField (reward.attachValue, tinyLayouts);
						HLHNPCReward newReward = new HLHNPCReward ();
						newReward.rewardType = rewardType;
						newReward.attachValue = attachValue;
						newReward.rewardValue = rewardValue;
						c.possibleRewards [k] = newReward;
					}


					bool addNewReward = GUILayout.Button ("添加", tinyLayouts);
					bool removeLastReward = GUILayout.Button ("删除", tinyLayouts);

					if (addNewReward) {
						c.possibleRewards.Add (new HLHNPCReward ());
					}

					if (removeLastReward && c.possibleRewards.Count >= 1) {
						c.possibleRewards.RemoveAt (c.possibleRewards.Count - 1);
					}

					EditorGUILayout.EndHorizontal ();

					c.triggeredTaskId = EditorGUILayout.IntField ("触发的任务id", c.triggeredTaskId, shortLayouts);
					c.isEnd = EditorGUILayout.Toggle ("是否结束对话组",c.isEnd, shortLayouts);
					EditorGUILayout.Separator ();
					EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
				}

			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
		}


		private void DrawDialogs(HLHDialogGroup dg){
			
			if (dg.dialogs == null) {
				dg.dialogs = new List<HLHDialog> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			bool createNewDialog = GUILayout.Button ("添加新的对话",buttonLayouts);
			bool removeLastDialog = GUILayout.Button ("删除尾部对话",buttonLayouts);
			bool unfoldAll = GUILayout.Button ("全部展开", buttonLayouts);
			bool foldAll = GUILayout.Button ("全部合上", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (unfoldAll) {
				for (int i = 0; i < dialogFoldOutInfoArray.GetLength(1); i++) {
					dialogFoldOutInfoArray[dg.dialogGroupId,i] = true;
				}
			}

			if (foldAll) {
				for (int i = 0; i < dialogFoldOutInfoArray.GetLength(1); i++) {
					dialogFoldOutInfoArray[dg.dialogGroupId,i] = false;
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

				dialogFoldOutInfoArray [dg.dialogGroupId, j] = EditorGUILayout.Foldout (dialogFoldOutInfoArray [dg.dialogGroupId, j], "编辑对话");

				if (dialogFoldOutInfoArray [dg.dialogGroupId, j]) {

					HLHDialog d = dg.dialogs [j];

					d.dialogId = j;

					EditorGUILayout.LabelField ("对话ID", d.dialogId.ToString(), shortLayouts);

					d.dialogContent = EditorGUILayout.TextField ("对话内容", d.dialogContent, longLayouts);

					if (d.choiceIds == null) {
						d.choiceIds = new List<int> ();
					}

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (20)
					});
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.BeginHorizontal ();
					bool createNewChoice = GUILayout.Button ("添加新的选择",buttonLayouts);
					bool removeLastChoice = GUILayout.Button ("删除尾部选择",buttonLayouts);
					EditorGUILayout.EndHorizontal ();

					if (createNewChoice) {
						d.choiceIds.Add (0);
					}

					if (removeLastChoice && d.choiceIds.Count > 0) {
						d.choiceIds.RemoveAt (d.choiceIds.Count - 1);
					}

					EditorGUILayout.BeginHorizontal ();
					for (int k = 0; k < d.choiceIds.Count; k++) {
						d.choiceIds [k] = EditorGUILayout.IntField (d.choiceIds [k], shortLayouts);
					}
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


		private void DrawTriggerConditions(HLHDialogGroup dg){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();
			triggerConditionInfoArray [dg.dialogGroupId] = EditorGUILayout.Foldout (triggerConditionInfoArray [dg.dialogGroupId], "编辑触发条件");

			if (triggerConditionInfoArray [dg.dialogGroupId]) {
				EditorGUILayout.LabelField ("-----------触发条件------------", middleLayouts);
				EditorGUILayout.BeginHorizontal ();
				bool addNewTriggerCondition = GUILayout.Button ("添加触发条件2", buttonLayouts);
				bool removeLastTriggerCondition = GUILayout.Button ("移除触发条件2", buttonLayouts);
				EditorGUILayout.EndHorizontal ();

				if (dg.triggerCondition.condition_1 == null) {
					dg.triggerCondition.condition_1 = new List<HLHValueWithLink> ();
					dg.triggerCondition.condition_1.Add (new HLHValueWithLink ());
				}

				if (addNewTriggerCondition && dg.triggerCondition.condition_2 == null) {
					dg.triggerCondition.condition_2 = new List<HLHValueWithLink> ();
					dg.triggerCondition.condition_2.Add (new HLHValueWithLink ());

				}

				if (removeLastTriggerCondition && dg.triggerCondition.condition_2 != null) {
					dg.triggerCondition.condition_2 = null;
				}


				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
					GUILayout.Height (20),
					GUILayout.Width (20)
				});
				EditorGUILayout.BeginVertical ();


				EditorGUILayout.LabelField ("触发条件1", shortLayouts);
				EditorGUILayout.BeginHorizontal ();
				for (int i = 0; i < dg.triggerCondition.condition_1.Count; i++) {
					HLHValueWithLink vwl = dg.triggerCondition.condition_1 [i];
					EditorGUILayout.BeginHorizontal ();
					HLHConditionType type = (HLHConditionType)EditorGUILayout.EnumPopup (vwl.type, shorterLayouts);
					HLHCalculateLink calculateLink = (HLHCalculateLink)EditorGUILayout.EnumPopup (vwl.calculateLink, shorterLayouts);
					int value = EditorGUILayout.IntField (vwl.value, shorterLayouts);
					HLHValueWithLink newVwl = new HLHValueWithLink ();
					newVwl.type = type;
					newVwl.calculateLink = calculateLink;
					newVwl.value = value;
					dg.triggerCondition.condition_1 [i] = newVwl;

					EditorGUILayout.EndHorizontal ();

				}

				bool addNewVwlInCondition1 = GUILayout.Button ("添加", tinyLayouts);
				bool removeLastVwlInCondition1 = GUILayout.Button ("删除", tinyLayouts);

				if (addNewVwlInCondition1) {
					dg.triggerCondition.condition_1.Add (new HLHValueWithLink ());
				}

				if (removeLastVwlInCondition1 && dg.triggerCondition.condition_1.Count > 1) {
					dg.triggerCondition.condition_1.RemoveAt (dg.triggerCondition.condition_1.Count - 1);
				}

				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndHorizontal ();


				if (dg.triggerCondition.condition_2 != null) {

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("**************************************", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (20)
					});
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.LabelField ("触发条件2", shortLayouts);
					EditorGUILayout.BeginHorizontal ();
					for (int i = 0; i < dg.triggerCondition.condition_2.Count; i++) {
						HLHValueWithLink vwl = dg.triggerCondition.condition_2 [i];
						EditorGUILayout.BeginHorizontal ();
						HLHConditionType type = (HLHConditionType)EditorGUILayout.EnumPopup (vwl.type, shorterLayouts);
						HLHCalculateLink calculateLink = (HLHCalculateLink)EditorGUILayout.EnumPopup (vwl.calculateLink, shorterLayouts);
						int value = EditorGUILayout.IntField (vwl.value, shorterLayouts);
						HLHValueWithLink newVwl = new HLHValueWithLink ();
						newVwl.type = type;
						newVwl.calculateLink = calculateLink;
						newVwl.value = value;
						dg.triggerCondition.condition_2 [i] = newVwl;

						EditorGUILayout.EndHorizontal ();

					}

					bool addNewVwlInCondition2 = GUILayout.Button ("添加", tinyLayouts);
					bool removeLastVwlInCondition2 = GUILayout.Button ("删除", tinyLayouts);
					if (addNewVwlInCondition2) {
						dg.triggerCondition.condition_2.Add (new HLHValueWithLink ());
					}
					if (removeLastVwlInCondition2 && dg.triggerCondition.condition_2.Count > 1) {
						dg.triggerCondition.condition_2.RemoveAt (dg.triggerCondition.condition_2.Count - 1);
					}

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

	}
}
