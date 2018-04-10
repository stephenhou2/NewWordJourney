using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class HLHNPC {
		
		public int npcId;

		public string npcName;

		public bool isExcutor;

		public List<HLHDialogGroup> dialogGroups = new List<HLHDialogGroup>();

		// 注意HLHTask是结构体，非引用类型
		public List<HLHTask> taskList = new List<HLHTask> ();

		public List<HLHNPCGoodsGroup> npcGoodsGroupList = new List<HLHNPCGoodsGroup> ();

		public List<HLHDialogGroup> regularGreetings = new List<HLHDialogGroup> ();

		// 标示当npc数据变化时是否需要保存npc数据
		public bool saveOnChange;

		// 本层触发的对话组的记录
		private HLHDialogGroup dialogGroupRecord;

		private List<HLHNPCGoods> goodsInSellRecord = new List<HLHNPCGoods>();

		public MonsterData monsterData;

		//******************************************************************************** start *************************************************************************//

		/// <summary>
		/// 查找人物当前触发的对话组
		/// </summary>
		/// <returns>The qulified dialog group.</returns>
		/// <param name="player">Player.</param>
		public HLHDialogGroup FindQulifiedDialogGroup(Player player){

			HLHDialogGroup targetDg = null;

			if (dialogGroupRecord != null) {
				if (!dialogGroupRecord.isFinish) {
					return dialogGroupRecord;
				} else {
					int randomSeed = Random.Range (0, regularGreetings.Count);
					targetDg = regularGreetings[randomSeed];
					dialogGroupRecord = targetDg;
					return targetDg;
				}
			}

			for (int i = 0; i < dialogGroups.Count; i++) {

				HLHDialogGroup dg = dialogGroups [i];

				for (int j = 0; j < player.inProgressTasks.Count; j++) {

					HLHTask t = player.inProgressTasks [j];

					if (t.dialogGroupId == dg.dialogGroupId) {

						targetDg = dg;
						dialogGroupRecord = dg;

						return dg;
					}
				}
			}

			List<HLHDialogGroup> possibleDialogGroups = new List<HLHDialogGroup> ();

			for (int i = 0; i < dialogGroups.Count; i++) {

				HLHDialogGroup dg = dialogGroups [i];

				if (dg.isFinish) {
					continue;
				}
					
				if (dg.triggerLevel == -1) {

					// 如果符合条件的对话组不是任务触发的对话组
					if (!dg.isTaskTriggeredDg) {
						possibleDialogGroups.Add (dg);
					} else {
						// 如果符合添加的对话组是从任务触发的对话组
						bool hasPlayerReceiveTask = player.CheckTaskExistFromTriggeredDialogGroupId (dg.dialogGroupId);
						if (hasPlayerReceiveTask) {
							targetDg = dg;
							break;
						}
					}
				} else if(dg.triggerLevel == player.currentLevelIndex && !dg.isFinish){
					targetDg = dg;
					dialogGroupRecord = dg;
					return targetDg;
				}
			}

			if (targetDg == null && possibleDialogGroups.Count > 0) {
				int randomSeed = Random.Range (0, possibleDialogGroups.Count);
				targetDg = possibleDialogGroups [randomSeed];
			}


			if (targetDg == null) {
				int randomSeed = Random.Range (0, regularGreetings.Count);
				targetDg = regularGreetings[randomSeed];
				dialogGroupRecord = targetDg;
			}

			dialogGroupRecord = targetDg;

			return targetDg;

		}

		public HLHTask GetTask(int taskId){

			HLHTask task = taskList.Find (delegate(HLHTask obj) {
				return obj.taskId == taskId;
			});

			return task;

		}


		/// <summary>
		/// 查找玩家所有在当前npc处接受的任务
		/// </summary>
		/// <returns>The qulified task.</returns>
		/// <param name="taskId">Task identifier.</param>
		public List<HLHTask> FindAllTasksReceiveFromCurrentNpc(Player player){

			List<HLHTask> allTasksReceiveFromCurrentNpc = player.inProgressTasks.FindAll (delegate(HLHTask obj) {
				return obj.npcId == npcId;
			});

			return allTasksReceiveFromCurrentNpc;
		}

		public List<HLHNPCGoods> GetCurrentLevelGoods(){

			if (goodsInSellRecord.Count != 0) {
				return goodsInSellRecord;
			}

			int ggIndex = Player.mainPlayer.currentLevelIndex / 5;

			goodsInSellRecord.Clear ();

			HLHNPCGoodsGroup gg = npcGoodsGroupList[ggIndex];

			goodsInSellRecord.Add (GetRandomGoods(gg.goodsList_1));
			goodsInSellRecord.Add (GetRandomGoods(gg.goodsList_2));
			goodsInSellRecord.Add (GetRandomGoods(gg.goodsList_3));
			goodsInSellRecord.Add (GetRandomGoods(gg.goodsList_4));
			goodsInSellRecord.Add (GetRandomGoods(gg.goodsList_5));

			return goodsInSellRecord;
		}

		private HLHNPCGoods GetRandomGoods(List<HLHNPCGoods> possibleGoods){

			int randomSeed = Random.Range (0, possibleGoods.Count);

			return possibleGoods [randomSeed];
		}


		/// <summary>
		/// npc卖东西给玩家(如果商品是固定数量的，则当商品卖出后该商品的数量会-1，否则商品数量不变)
		/// </summary>
		/// <param name="goodsId">Goods identifier.</param>
		/// <param name="player">Player.</param>
		public void SoldGoods(int goodsId){

			int goodsDisplayIndex = goodsInSellRecord.FindIndex (delegate(HLHNPCGoods obj) {
				return obj.goodsId == goodsId;
			});

			goodsInSellRecord.RemoveAt (goodsDisplayIndex);

		}



		/// <summary>
		/// 玩家交付任务
		/// </summary>
		/// <returns>如果任务完成，则返回完后任务后的对话组，否则返回null</returns>
		/// <param name="player">Player.</param>
		public HLHDialogGroup PlayerHandInTask(Player player){

			HLHDialogGroup taskFinishDialogGroup = null;

			List<HLHTask> allTasksQualified = FindAllTasksReceiveFromCurrentNpc (player);

			for (int i = 0; i < allTasksQualified.Count; i++) {

				HLHTask task = allTasksQualified [i];

				if (player.CheckTaskFinish (task)) {

					player.FinishTask (task);

					taskFinishDialogGroup = dialogGroups.Find (delegate(HLHDialogGroup obj) {
						return obj.dialogGroupId == task.dialogGroupId;
					});

					break;

				}

			}

			return taskFinishDialogGroup;
		}


		//***************************************************************************** end ****************************************************************************//

	}


	[System.Serializable]
	public class HLHDialogGroup{

		public int dialogGroupId;

		// 对话的触发关卡序号【-1代表对话不跟关卡走】
		public int triggerLevel;

		public List<HLHDialog> dialogs;

		public List<HLHChoice> choices;

		public bool isFinish;

		// 是否可以反复触发
		public bool isMultiOff;

		public bool isTaskTriggeredDg;


		public HLHDialog GetDialog(HLHChoice choice){
			return dialogs.Find (delegate(HLHDialog obj) {
				return obj.dialogId == choice.nextDialogId;
			});
		}

		//***************************************************************************** start *******************************************************************************//

		public HLHChoice[] GetChoices(HLHDialog dialog){

			HLHChoice[] choicesArray = new HLHChoice[dialog.choiceIds.Count];

			for (int i = 0; i < choicesArray.Length; i++) {

				int choiceId = dialog.choiceIds [i];

				HLHChoice choice = choices.Find (delegate (HLHChoice obj) {
					return obj.choiceId == choiceId;
				});

				choicesArray [i] = choice;

			}

			return choicesArray;

		}
		//****************************************************************************** end *******************************************************************************//

	}

		

	[System.Serializable]
	public class HLHDialog{

		public int dialogId;

		public string dialogContent;

		public List<int> choiceIds;

	}

	[System.Serializable]
	public class HLHChoice{

		public int choiceId;

		public string choiceContent;

		public int nextDialogId;

		public bool isTradeTriggered;

		public bool isFightTriggered;

		public bool isWeaponChangeTriggered;

		public bool isEquipmentLoseTriggered;

		public bool isRewardTriggered;

		public bool isWordLearningTriggered;

		public bool isReceiveTaskTriggered;

		public bool isHandInTaskTriggered;

		public bool isAddSkillTriggered;

		public bool isRobTriggered;

		public List<HLHNPCReward> rewards;

		public int triggeredTaskId;

		// 标示是否退出对话
		public bool isEnd;

		// 标示该对话组是否标记为结束【该项应该只在isEnd = true的情况下进行设定】
		public bool finishCurrentDialog;


	}

	[System.Serializable]
	public struct HLHTask{

		public int npcId;

		public int taskId;

		public int taskItemId;

		public int taskItemCount;// 需要交付的任务物品数量

		public int dialogGroupId;

		public string taskDescription;

		public bool isCurrentLevelTask;// 是否是本层任务

		public HLHTask(int npcId,int taskId,int taskItemId,int taskItemCount,int dialogGroupId,string taskDescription,bool isCurrentTask){
			this.npcId = npcId;
			this.taskId = taskId;
			this.taskItemId = taskItemId;
			this.taskItemCount = taskItemCount;
			this.dialogGroupId = dialogGroupId;
			this.taskDescription = taskDescription;
			this.isCurrentLevelTask = isCurrentTask;
		}


		
	}

	public enum HLHRewardType{
		Gold,
		Item,
		Property,
		Experience
	}


	[System.Serializable]
	public struct HLHNPCReward{

		public HLHRewardType rewardType;

		// 奖励的数值（金钱-数值，物品-物品id，属性-属性类型，经验-数值）
		public int rewardValue;

		// 如果奖励的是物品，则该数据代表物品数量，如果奖励的是属性，则该数据代表属性变化值
		public int attachValue;

	}

	[System.Serializable]
	public class HLHNPCGoodsGroup{

		public List<HLHNPCGoods> goodsList_1= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_2= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_3= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_4= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_5= new List<HLHNPCGoods> ();

	}
		

	[System.Serializable]
	public class HLHNPCGoods{

		public int goodsId;
		public float priceFloat;
		public int equipmentQuality;//0:灰色，1:蓝色，2:金色


		//******************************************************************************* start *****************************************************************************//

		public Item itemAsGoods;

		public Item GetGoodsItem(){
			if (itemAsGoods == null) {
				itemAsGoods = Item.NewItemWith (goodsId, 1);
			}
			return itemAsGoods;
		}

		//********************************************************************************* end ***************************************************************************//


	}


}
