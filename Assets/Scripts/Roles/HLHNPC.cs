using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class HLHNPC {


		public int npcId;

		public string npcName;

		public List<HLHDialogGroup> dialogGroups = new List<HLHDialogGroup>();

		// 注意HLHTask是结构体，非引用类型
		public List<HLHTask> taskList = new List<HLHTask> ();

		public List<HLHNPCGoods> npcGoodsList = new List<HLHNPCGoods> ();

		public HLHDialogGroup regularGreeting;


		/// <summary>
		/// 查找人物当前触发的对话组
		/// </summary>
		/// <returns>The qulified dialog group.</returns>
		/// <param name="player">Player.</param>
		public HLHDialogGroup FindQulifiedDialogGroup(Player player){

			HLHDialogGroup targetDg = null;

			for (int i = 0; i < dialogGroups.Count; i++) {

				HLHDialogGroup dg = dialogGroups [i];

				for (int j = 0; j < player.inProgressTasks.Count; j++) {

					HLHTask t = player.inProgressTasks [j];

					if (t.dialogGroupId == dg.dialogGroupId) {

						targetDg = dg;

						return dg;
					}
				}
			}

			for (int i = 0; i < dialogGroups.Count; i++) {

				HLHDialogGroup dg = dialogGroups [i];

				if (!dg.isFinish && dg.triggerCondition.IsTriggered (player)) {

					// 如果符合条件的对话组不是任务触发的对话组
					if (!dg.isTaskTriggeredDg) {
						targetDg = dg;
						break;
					} else {
						// 如果符合添加的对话组是从任务触发的对话组
						bool hasPlayerReceiveTask = player.CheckTaskExistFromTriggeredDialogGroupId (dg.dialogGroupId);
						if (hasPlayerReceiveTask) {
							targetDg = dg;
							break;
						}
					}

				}

			}

			if (targetDg == null) {
				targetDg = regularGreeting;
			}

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



		/// <summary>
		/// npc卖东西给玩家(如果商品是固定数量的，则当商品卖出后该商品的数量会-1，否则商品数量不变)
		/// </summary>
		/// <param name="goodsId">Goods identifier.</param>
		/// <param name="player">Player.</param>
		public void SoldGoods(int goodsId){

			npcGoodsList.RemoveAt (goodsId);

//			for (int i = 0; i < npcGoodsList.Count; i++) {
//
//				HLHNPCGoods goods = npcGoodsList [i];
//
//				if (goods.goodsId == goodsId && !goods.isFixedCount) {
//					npcGoodsList [i].totalCount--;
//				}
//			}

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

	}


	[System.Serializable]
	public class HLHDialogGroup{

		public int dialogGroupId;

		public HLHTriggerCondition triggerCondition;

		public List<HLHDialog> dialogs;

		public List<HLHChoice> choices;

		public bool isFinish;

		public bool isTaskTriggeredDg;

		public HLHDialog GetDialog(HLHChoice choice){
			return dialogs.Find (delegate(HLHDialog obj) {
				return obj.dialogId == choice.triggerDialogId;
			});
		}

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

	}

	/// <summary>
	/// 人物隐藏属性枚举
	/// </summary>
	public enum HLHRoleHiddenProperty{
		Justice,
		Power
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

		public int triggerDialogId;

		public bool isHiddenPropertyChangeTriggered;

		public bool isReceiveTaskTriggered;

		public bool isHandInTaskTriggered;

		public bool isRewardTriggered;

		public bool isTradeTriggered;

		public bool isAddSkillTriggered;

		public bool isFightTriggered;

		public int playerJusticeChange;

		public int playerPowerChange;

		public List<HLHNPCReward> possibleRewards;

		public int triggeredTaskId;

		public bool isEnd;


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
	public class HLHNPCGoods{

		public int goodsId;

		public Item itemAsGoods;

		public Item GetGoodsItem(){
			if (itemAsGoods == null) {
				itemAsGoods = Item.NewItemWith (goodsId, 1);
			}
			return itemAsGoods;
		}

		public HLHNPCGoods(int goodsId){
			this.goodsId = goodsId;
		}

	}

	[System.Serializable]
	public struct HLHTriggerCondition{

		// 单个数组内部做与运算
		public List<HLHValueWithLink> condition;

//		public List<HLHValueWithLink> condition_2;


		public bool IsTriggered(Player player){

			bool isCondition1Triggered = true;
//			bool isCondition2Triggered = true;

			if (condition != null) {

				if (condition.Count == 0) {
					isCondition1Triggered = false;
				}

				for (int i = 0; i < condition.Count; i++) {

					HLHValueWithLink vwl = condition [i];

					bool temp = vwl.CheckAccordance (player);

					isCondition1Triggered = isCondition1Triggered && temp;

				}

			}

//			if (condition_2 != null) {
//
//				if (condition_2.Count == 0) {
//					isCondition2Triggered = false;
//				}
//
//				for (int i = 0; i < condition_2.Count; i++) {
//
//					HLHValueWithLink vwl = condition_2 [i];
//
//					bool temp = vwl.CheckAccordance (player);
//
//					isCondition2Triggered = isCondition2Triggered && temp;
//
//				}
//
//			}

//			return isCondition1Triggered || isCondition2Triggered;
			return isCondition1Triggered;
		}

	}

	/// <summary>
	/// 数值运算枚举
	/// </summary>
	public enum HLHCalculateLink{
		LessOrEqual, //  <=
		Equal, //  ==
		MoreOrEqual //  >=
	}

	/// <summary>
	/// 人物隐藏属性枚举
	/// </summary>
	public enum HLHConditionType{
		Justice,//正义度
		TotalJustice,//总正义度
		Power,//强大度
		TotalPower,//总强大度
		GameLevel//关卡所在层级
	}

	[System.Serializable]
	public struct HLHValueWithLink{

		public HLHConditionType type;

		public HLHCalculateLink calculateLink;

		public int value;

		public bool CheckAccordance(Player player){

			bool isAccord = false;

			switch (type) {
			case HLHConditionType.Justice:
				isAccord = CheckNumAccord (player.justice);
				break;
			case HLHConditionType.TotalJustice:
				isAccord = CheckNumAccord (player.totalJustice);
				break;
			case HLHConditionType.Power:
				isAccord = CheckNumAccord (player.power);
				break;
			case HLHConditionType.TotalPower:
				isAccord = CheckNumAccord (player.totalJustice);
				break;
			case HLHConditionType.GameLevel:
				isAccord = CheckNumAccord (player.maxUnlockLevelIndex);
				break;
			}

			return isAccord;
		}

		private bool CheckNumAccord(int num){

			bool isNumAccord = false;

			switch (calculateLink) {
			case HLHCalculateLink.LessOrEqual:
				isNumAccord = num <= value;
				break;
			case HLHCalculateLink.Equal:
				isNumAccord = num == value;
				break;
			case HLHCalculateLink.MoreOrEqual:
				isNumAccord = num >= value;
				break;
			}
				
			return isNumAccord;
		}

	}

}
