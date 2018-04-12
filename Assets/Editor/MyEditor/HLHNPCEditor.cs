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


		private bool[] dialogGroupFoldOutInfoArray;

		private bool[,] dialogFoldOutInfoArray;

		private bool[,] choiceFoldOutInfoArray;

		private bool[] taskFoldOutInfoArray;

		private bool[] goodsFoldOutInfoArray;

		private bool[] regularGreetingDgFoldoutInfoArray;
		private bool[,] regularGreetingDialogFoldOutInfoArray;
		private bool[,] regularGreetingChoiceFoldOutInfoArray;

		private bool[,] wordRightDialogFoldoutInfoArray;
		private bool[,] wordRightChoiceFoldoutInfoArray;

		private bool[,] wordWrongDialogFoldoutInfoArray;
		private bool[,] wordWrongChoiceFoldoutInfoArray;

		private int dataResult = -1;


		[MenuItem("EditHelper/HLHNPCEditor")]
		public static void InitHLHNPCEditor(){

			HLHNPCEditor editor = HLHNPCEditor.npcEditor;
			editor.npc = new HLHNPC ();

			editor.dialogFoldOutInfoArray = new bool[50, 50];
			editor.choiceFoldOutInfoArray = new bool[50, 50];
			editor.taskFoldOutInfoArray = new bool[50];
			editor.goodsFoldOutInfoArray = new bool[10];
			editor.dialogGroupFoldOutInfoArray = new bool[50];
			editor.regularGreetingDgFoldoutInfoArray = new bool[10];
			editor.regularGreetingDialogFoldOutInfoArray = new bool[50,50];
			editor.regularGreetingChoiceFoldOutInfoArray = new bool[50,50];
			editor.wordRightDialogFoldoutInfoArray = new bool[1,50];
			editor.wordRightChoiceFoldoutInfoArray = new bool[1,50];
			editor.wordWrongDialogFoldoutInfoArray = new bool[1,50];
			editor.wordWrongChoiceFoldoutInfoArray = new bool[1,50];
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
						npc.monsterData = new MonsterData ();
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

			npc.isExcutor = EditorGUILayout.Toggle ("是否是执法者", npc.isExcutor, shortLayouts);

			DrawRegularDialogGroups ();

			DrawTasks ();

			DrawGoods ();

			DrawRegularGreetings ();

			DrawMonsterData ();

			DrawWordRightDialogGroup ();

			DrawWordWrongDialogGroup ();

			EditorGUILayout.EndScrollView ();
		}

		private void DrawWordRightDialogGroup(){
			
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑单词正确的对话组", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});
			EditorGUILayout.BeginVertical ();

			HLHDialogGroup wordRightDg = npc.wordRightDialogGroup;

			EditorGUILayout.BeginHorizontal ();
			bool addWordRightDg = GUILayout.Button ("添加单词正确的对话组", buttonLayouts);
			bool removeWordRightDg = GUILayout.Button ("删除单词正确的对话组", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (addWordRightDg && wordRightDg == null) {
				npc.wordRightDialogGroup = new HLHDialogGroup ();
				wordRightDg = npc.wordRightDialogGroup;
			}

			if (removeWordRightDg) {
				npc.wordRightDialogGroup = null;
				wordRightDg = null;
			}

			if (wordRightDg == null) {
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
				return;
			}

			wordRightDg.dialogGroupId = 0;
			wordRightDg.triggerLevel = -1;
			wordRightDg.isTaskTriggeredDg = false;

			DrawDialogs (wordRightDg, wordRightDialogFoldoutInfoArray);
			DrawChoices (wordRightDg, wordRightChoiceFoldoutInfoArray);

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
		}

		private void DrawWordWrongDialogGroup(){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑单词错误的对话组", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});
			EditorGUILayout.BeginVertical ();

			HLHDialogGroup wordWrongDg = npc.wordWrongDialogGroup;

			EditorGUILayout.BeginHorizontal ();
			bool addWordWrongDg = GUILayout.Button ("添加单词错误的对话组", buttonLayouts);
			bool removeWordWrongDg = GUILayout.Button ("删除单词错误的对话组", buttonLayouts);
			EditorGUILayout.EndHorizontal ();

			if (addWordWrongDg && wordWrongDg == null) {
				npc.wordWrongDialogGroup = new HLHDialogGroup ();
				wordWrongDg = npc.wordWrongDialogGroup;
			}

			if (removeWordWrongDg) {
				npc.wordWrongDialogGroup = null;
				wordWrongDg = null;
			}

			if (wordWrongDg == null) {
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
				return;
			}

			wordWrongDg.dialogGroupId = 0;
			wordWrongDg.triggerLevel = -1;
			wordWrongDg.isTaskTriggeredDg = false;

			DrawDialogs (wordWrongDg, wordWrongDialogFoldoutInfoArray);
			DrawChoices (wordWrongDg, wordWrongChoiceFoldoutInfoArray);


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
		}

		private void DrawRegularGreetings (){

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
				dg.choices = new List<HLHChoice> ();
				npc.regularGreetings.Add (dg);

			}
			if (removeLastDialogGroup && npc.regularGreetings.Count > 0) {
				npc.regularGreetings.RemoveAt (npc.regularGreetings.Count - 1);
			}


			for (int i = 0; i < npc.regularGreetings.Count; i++) {

				regularGreetingDgFoldoutInfoArray [i] = EditorGUILayout.Foldout (regularGreetingDgFoldoutInfoArray [i], "npc寒暄情况" + (i+1).ToString());


				if (regularGreetingDgFoldoutInfoArray [i]) {

					HLHDialogGroup dg = npc.regularGreetings [i];

					dg.dialogGroupId = i;

					dg.isTaskTriggeredDg = false;

					dg.triggerLevel = -1;

					DrawDialogs (dg,regularGreetingDialogFoldOutInfoArray);

					DrawChoices (dg,regularGreetingChoiceFoldOutInfoArray);
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



		private void DrawRegularDialogGroups(){

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

					dg.isTaskTriggeredDg = EditorGUILayout.Toggle ("是否任务触发的对话", dg.isTaskTriggeredDg, shortLayouts);

					dg.triggerLevel = EditorGUILayout.IntField ("触发关卡", dg.triggerLevel, middleLayouts);

					DrawDialogs (dg,dialogFoldOutInfoArray);

					DrawChoices (dg,choiceFoldOutInfoArray);


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

		private void DrawTasks(){

			if (npc.taskList == null) {
				npc.taskList = new List<HLHTask> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑任务信息", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
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

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("\t\t\t\t", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (20)
					});
					EditorGUILayout.BeginVertical ();

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

					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				}

			}


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);

		}

		private void DrawChoices(HLHDialogGroup dg,bool[,] choiceFoldoutInfoArray){

			if (dg.choices == null) {
				dg.choices = new List<HLHChoice> ();
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***编辑人物选择", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
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

				HLHChoice c = dg.choices [j];

				c.choiceId = j;
				string foldContent = string.Format ("编辑选择	\t\t***选择ID:   {0}\t\t\t***选择内容:   {1}", c.choiceId, c.choiceContent);

				choiceFoldoutInfoArray [dg.dialogGroupId, j] = EditorGUILayout.Foldout (choiceFoldoutInfoArray [dg.dialogGroupId, j], foldContent);

				if (choiceFoldoutInfoArray [dg.dialogGroupId, j]) {

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("\t\t\t\t", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (20)
					});
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.LabelField ("选择ID:"+c.choiceId.ToString(), shortLayouts);

					c.choiceContent = EditorGUILayout.TextField ("选择的内容", c.choiceContent, longLayouts);

					c.nextDialogId = EditorGUILayout.IntField ("npc下一句话id", c.nextDialogId, shortLayouts);

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("触发事件类型:", new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(100)});
					c.isTradeTriggered = EditorGUILayout.ToggleLeft ("交易", c.isTradeTriggered, shorterLayouts);
					c.isFightTriggered = EditorGUILayout.ToggleLeft ("战斗", c.isFightTriggered, shorterLayouts);
					c.isWeaponChangeTriggered = EditorGUILayout.ToggleLeft ("武器更换", c.isWeaponChangeTriggered, shorterLayouts);
					c.isEquipmentLoseTriggered = EditorGUILayout.ToggleLeft ("装备丢失", c.isEquipmentLoseTriggered, shorterLayouts);
					c.isRewardTriggered = EditorGUILayout.ToggleLeft ("奖励", c.isRewardTriggered, shorterLayouts);
					c.isWordLearningTriggered = EditorGUILayout.ToggleLeft ("单词", c.isWordLearningTriggered, shorterLayouts);
					c.isReceiveTaskTriggered = EditorGUILayout.ToggleLeft ("接受任务", c.isReceiveTaskTriggered, shorterLayouts);
					c.isHandInTaskTriggered = EditorGUILayout.ToggleLeft ("提交任务", c.isHandInTaskTriggered, shorterLayouts);
					c.isAddSkillTriggered = EditorGUILayout.ToggleLeft ("添加技能", c.isAddSkillTriggered, shorterLayouts);
					c.isRobTriggered = EditorGUILayout.ToggleLeft ("掠夺", c.isRobTriggered, shorterLayouts);
					EditorGUILayout.EndHorizontal ();



					if (c.isRewardTriggered) {

						EditorGUILayout.LabelField ("可能的奖励(属性 0:最大血量，1:最大魔法，2:攻击，3:魔法攻击，4:移速，5:护甲，6:抗性，" +
							"7:护甲穿透，8:魔法穿透，9:暴击，10:闪避，11:暴击倍率，12:额外金钱，13:额外经验，14:生命回复，15:魔法回复,16:实际血量,17:骑士血量惩罚,18:骑士攻击惩罚)", new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(1500)});
						EditorGUILayout.LabelField ("物品---id---品质", new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(1500)});
						
						if (c.rewards == null) {
							c.rewards = new List<HLHNPCReward> ();
							for (int k = 0; k < 10; k++) {
								HLHNPCReward reward = new HLHNPCReward ();
								c.rewards.Add (reward);
							}
						} else if (c.rewards.Count == 0) {
							for (int k = 0; k < 10; k++) {
								HLHNPCReward reward = new HLHNPCReward ();
								c.rewards.Add (reward);
							}
						}

						for (int k = 0; k < 10; k++) {
							HLHNPCReward reward = c.rewards [k];
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.LabelField (string.Format ("{0}-{1}层奖励", k * 5, (k + 1) * 5), middleLayouts);
							HLHRewardType rewardType = (HLHRewardType)EditorGUILayout.EnumPopup (reward.rewardType, tinyLayouts);
							int rewardValue = EditorGUILayout.IntField (reward.rewardValue, tinyLayouts);
							int attachValue = EditorGUILayout.IntField (reward.attachValue, tinyLayouts);
							EditorGUILayout.EndHorizontal ();
							HLHNPCReward newReward = new HLHNPCReward ();
							newReward.rewardType = rewardType;
							newReward.attachValue = attachValue;
							newReward.rewardValue = rewardValue;
							c.rewards [k] = newReward;
						}
					} else {
						c.rewards = null;
					}
						

					c.triggeredTaskId = EditorGUILayout.IntField ("触发的任务id", c.triggeredTaskId, shortLayouts);

					EditorGUILayout.BeginHorizontal ();
					c.isEnd = EditorGUILayout.Toggle ("是否退出当前对话组",c.isEnd, shortLayouts);
					c.finishCurrentDialog = EditorGUILayout.Toggle ("是否将本对话组标记为已结束", c.finishCurrentDialog, shortLayouts);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.Separator ();
					EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				}

			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
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

			dg.isMultiOff = EditorGUILayout.Toggle ("是否可以反复触发", dg.isMultiOff, middleLayouts);

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
			
		public void DrawMonsterData(){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("***属性", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (120)
			});
			EditorGUILayout.BeginVertical ();

			MonsterData md = npc.monsterData;


			if (md == null) {
				npc.monsterData = new MonsterData ();
				md = npc.monsterData;
			}

			if (md.monsterPropertyGainList.Count == 0) {
				for (int i = 0; i < 10; i++) {
					md.monsterPropertyGainList.Add (new MonsterPropertyGain ());
				}
			}

			md.monsterName = npc.npcName;
			md.monsterId = npc.npcId;

			EditorGUILayout.LabelField ("基础属性", middleLayouts);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("\t\t\t", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (20)
			});
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("血量", tinyLayouts);
			EditorGUILayout.LabelField ("魔法", tinyLayouts);
			EditorGUILayout.LabelField ("攻击", tinyLayouts);
			EditorGUILayout.LabelField ("魔攻", tinyLayouts);
			EditorGUILayout.LabelField ("护甲", tinyLayouts);
			EditorGUILayout.LabelField ("抗性", tinyLayouts);
			EditorGUILayout.LabelField ("护甲穿透", tinyLayouts);
			EditorGUILayout.LabelField ("魔法穿透", tinyLayouts);
			EditorGUILayout.LabelField ("移动速度", tinyLayouts);
			EditorGUILayout.LabelField ("暴击", tinyLayouts);
			EditorGUILayout.LabelField ("闪避", tinyLayouts);
			EditorGUILayout.LabelField ("暴击倍率", tinyLayouts);
			EditorGUILayout.LabelField ("额外金币", tinyLayouts);
			EditorGUILayout.LabelField ("额外经验", tinyLayouts);
			EditorGUILayout.LabelField ("生命回复", tinyLayouts);
			EditorGUILayout.LabelField ("魔法回复", tinyLayouts);
			EditorGUILayout.LabelField ("攻击间隔", tinyLayouts);
			EditorGUILayout.LabelField ("奖励金币", tinyLayouts);
			EditorGUILayout.LabelField ("奖励经验", tinyLayouts);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			md.originalMaxHealth = EditorGUILayout.IntField (md.originalMaxHealth, tinyLayouts);
			md.originalMaxMana = EditorGUILayout.IntField (md.originalMaxMana, tinyLayouts);
			md.originalAttack = EditorGUILayout.IntField (md.originalAttack, tinyLayouts);
			md.originalMagicAttack = EditorGUILayout.IntField (md.originalMagicAttack, tinyLayouts);
			md.originalArmor = EditorGUILayout.IntField (md.originalArmor, tinyLayouts);
			md.originalMagicResist = EditorGUILayout.IntField (md.originalMagicResist, tinyLayouts);
			md.originalArmorDecrease = EditorGUILayout.IntField (md.originalArmorDecrease, tinyLayouts);
			md.originalMagicResistDecrease = EditorGUILayout.IntField (md.originalMagicResistDecrease, tinyLayouts);
			md.originalMoveSpeed = EditorGUILayout.IntField (md.originalMoveSpeed, tinyLayouts);
			md.originalCrit = EditorGUILayout.FloatField (md.originalCrit, tinyLayouts);
			md.originalDodge = EditorGUILayout.FloatField (md.originalDodge, tinyLayouts);
			md.originalCritHurtScaler = EditorGUILayout.FloatField (md.originalCritHurtScaler, tinyLayouts);
			md.originalExtraGold = EditorGUILayout.IntField (md.originalExtraGold, tinyLayouts);
			md.originalExtraExperience = EditorGUILayout.IntField (md.originalExtraExperience, tinyLayouts);
			md.originalHealthRecovery = EditorGUILayout.IntField (md.originalHealthRecovery, tinyLayouts);
			md.originalMagicRecovery = EditorGUILayout.IntField (md.originalMagicRecovery, tinyLayouts);
			md.attackInterval = EditorGUILayout.FloatField (md.attackInterval, tinyLayouts);
			md.rewardGold = EditorGUILayout.IntField (md.rewardGold, tinyLayouts);
			md.rewardExperience = EditorGUILayout.IntField (md.rewardExperience, tinyLayouts);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.LabelField ("属性增长", middleLayouts);
			for (int i = 0; i < 10; i++) {
				MonsterPropertyGain mpg = md.monsterPropertyGainList [i];
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("\t\t\t", new GUILayoutOption[] {
					GUILayout.Height (20),
					GUILayout.Width (20)
				});
				EditorGUILayout.BeginVertical ();
				EditorGUILayout.BeginHorizontal ();

				EditorGUILayout.LabelField (string.Format ("{0}-{1}层", i * 5, (i + 1) * 5), tinyLayouts);

				EditorGUILayout.LabelField ("血量", tinyLayouts);
				EditorGUILayout.LabelField ("魔法", tinyLayouts);
				EditorGUILayout.LabelField ("攻击", tinyLayouts);
				EditorGUILayout.LabelField ("魔攻", tinyLayouts);
				EditorGUILayout.LabelField ("护甲", tinyLayouts);
				EditorGUILayout.LabelField ("抗性", tinyLayouts);
				EditorGUILayout.LabelField ("护甲穿透", tinyLayouts);
				EditorGUILayout.LabelField ("魔法穿透", tinyLayouts);
				EditorGUILayout.LabelField ("移动速度", tinyLayouts);
				EditorGUILayout.LabelField ("暴击", tinyLayouts);
				EditorGUILayout.LabelField ("闪避", tinyLayouts);
				EditorGUILayout.LabelField ("暴击倍率", tinyLayouts);
				EditorGUILayout.LabelField ("额外金币", tinyLayouts);
				EditorGUILayout.LabelField ("额外经验", tinyLayouts);
				EditorGUILayout.LabelField ("生命回复", tinyLayouts);
				EditorGUILayout.LabelField ("魔法回复", tinyLayouts);
				EditorGUILayout.LabelField ("攻击间隔", tinyLayouts);
				EditorGUILayout.LabelField ("奖励金币", tinyLayouts);
				EditorGUILayout.LabelField ("奖励经验", tinyLayouts);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("\t\t\t\t\t", tinyLayouts);
				mpg.maxHealthGain = EditorGUILayout.IntField (mpg.maxHealthGain, tinyLayouts);
				mpg.maxManaGain = EditorGUILayout.IntField (mpg.maxManaGain, tinyLayouts);
				mpg.attackGain = EditorGUILayout.IntField (mpg.attackGain, tinyLayouts);
				mpg.magicAttackGain = EditorGUILayout.IntField (mpg.magicAttackGain , tinyLayouts);
				mpg.armorGain = EditorGUILayout.IntField (mpg.armorGain, tinyLayouts);
				mpg.magicResistGain = EditorGUILayout.IntField (mpg.magicResistGain, tinyLayouts);
				mpg.armorDecreaseGain = EditorGUILayout.IntField (mpg.armorDecreaseGain, tinyLayouts);
				mpg.magicResistDecreaseGain = EditorGUILayout.IntField (mpg.magicResistDecreaseGain, tinyLayouts);
				mpg.moveSpeedGain = EditorGUILayout.IntField (mpg.moveSpeedGain, tinyLayouts);
				mpg.critGain = EditorGUILayout.FloatField (mpg.critGain, tinyLayouts);
				mpg.dodgeGain = EditorGUILayout.FloatField (mpg.dodgeGain, tinyLayouts);
				mpg.critHurtScalerGain = EditorGUILayout.FloatField (mpg.critHurtScalerGain, tinyLayouts);
				mpg.extraGoldGain = EditorGUILayout.IntField (mpg.extraGoldGain, tinyLayouts);
				mpg.extraExperienceGain = EditorGUILayout.IntField (mpg.extraExperienceGain, tinyLayouts);
				mpg.healthRecoveryGain = EditorGUILayout.IntField (mpg.healthRecoveryGain, tinyLayouts);
				mpg.magicRecoveryGain = EditorGUILayout.IntField (mpg.magicRecoveryGain, tinyLayouts);
				mpg.attackIntervalGain = EditorGUILayout.FloatField (mpg.attackIntervalGain, tinyLayouts);
				mpg.rewardGoldGain = EditorGUILayout.IntField (mpg.rewardGoldGain, tinyLayouts);
				mpg.rewardExperienceGain = EditorGUILayout.IntField (mpg.rewardExperienceGain, tinyLayouts);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndHorizontal ();
			}


			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("================================================================", seperatorLayouts);
		}
	}

}
