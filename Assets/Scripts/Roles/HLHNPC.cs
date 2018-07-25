using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class HLHNPC {
		
		public int npcId;

		public string npcName;

		//public bool isExcutor;

		public bool isChatTriggered;

		public bool isTradeTriggered;

		public bool isTransportTriggered;

		public bool isLearnSkillTriggered;

		public bool isPropertyPromotionTriggered;

		public bool isAddGemStoneTriggered;
        
		public List<HLHPropertyPromotion> propertyPromotionList = new List<HLHPropertyPromotion>();

		public List<HLHDialogGroup> levelDefiniteDialogGroups = new List<HLHDialogGroup>();

		public List<HLHDialogGroup> levelUndefiniteDialogGroups = new List<HLHDialogGroup>();

		public List<int> transportLevelList = new List<int>();
      
		public List<HLHNPCGoodsGroup> npcGoodsGroupList = new List<HLHNPCGoodsGroup> ();

		public List<HLHDialog> regularGreetings = new List<HLHDialog> ();

		// 单词选择正确时的对话组
		//public HLHDialogGroup wordRightDialogGroup;
		// 单词选择错误时的对话组
		//public HLHDialogGroup wordWrongDialogGroup;

		// 标示当npc数据变化时是否需要保存npc数据
		public bool saveOnChange;

		// 本层触发的对话组的记录
		private HLHDialogGroup dialogGroupRecord;

		private List<HLHNPCGoods> goodsInSellRecord = new List<HLHNPCGoods>();

		private bool isGoodsInitialized;
      
		public List<int> npcSkillIds = new List<int>();
       

        


		/// <summary>
		/// 查找人物当前触发的对话组
		/// </summary>
		/// <returns>The qulified dialog group.</returns>
		/// <param name="player">Player.</param>
		public HLHDialogGroup FindQulifiedDialogGroup(Player player){

			if (dialogGroupRecord != null && dialogGroupRecord.isMultiTimes)
            {
                return dialogGroupRecord;            
            }

			HLHDialogGroup targetDg = null;

         
			for (int i = 0; i < levelDefiniteDialogGroups.Count; i++) {
                
				HLHDialogGroup dg = levelDefiniteDialogGroups [i];

				if(dg.triggerLevel == player.currentLevelIndex){

					if(CheckDialogGroupFinished(dg)){
						break;
					}

					targetDg = dg;

					dialogGroupRecord = dg;
                                   
					return targetDg;
				}

			}

			int randomSeed = Random.Range(0, levelUndefiniteDialogGroups.Count);

			targetDg = levelUndefiniteDialogGroups[randomSeed];

			dialogGroupRecord = targetDg;
                     
			return targetDg;

		}

		public bool CheckDialogGroupFinished(HLHDialogGroup dialogGroup){

			bool dialogGroupFinished = false;

			List<HLHNPCChatRecord> dialogRecords = GameManager.Instance.gameDataCenter.chatRecords;

			for (int i = 0; i < dialogRecords.Count;i++){

				HLHNPCChatRecord dialogRecord = dialogRecords[i];

				if(dialogRecord.npcId == npcId && dialogRecord.npcDialogGroupID == dialogGroup.dialogGroupId){
					dialogGroupFinished = true;
					break;
				}
			}

			return dialogGroupFinished;

		}
        
		public List<HLHNPCGoods> GetCurrentLevelGoods(){

			if (goodsInSellRecord.Count != 0 || isGoodsInitialized) {
				return goodsInSellRecord;
			}

			int ggIndex = Player.mainPlayer.currentLevelIndex / 5;

			goodsInSellRecord.Clear ();

			HLHNPCGoodsGroup gg = npcGoodsGroupList[ggIndex];

			GenerateRandomGoods(gg.goodsList_1);
			GenerateRandomGoods(gg.goodsList_2);
			GenerateRandomGoods(gg.goodsList_3);
			GenerateRandomGoods(gg.goodsList_4);
			GenerateRandomGoods(gg.goodsList_5);

			isGoodsInitialized = true;

			return goodsInSellRecord;
		}

		private void GenerateRandomGoods(List<HLHNPCGoods> possibleGoods){
			
			if (possibleGoods.Count == 0) {
				return;
			}

			int randomSeed = Random.Range (0, possibleGoods.Count);

			HLHNPCGoods goods = possibleGoods [randomSeed];

			goodsInSellRecord.Add (goods);

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

	}


	[System.Serializable]
	public class HLHDialogGroup{

		public int dialogGroupId;

		// 对话的触发关卡序号【-1代表对话不跟关卡走】
		public int triggerLevel = -1;

		public List<HLHDialog> dialogs;

		public bool isRewardTriggered;

		public HLHNPCReward reward;

		public bool isFinish;

		public bool isMultiTimes;
      

	}


	[System.Serializable]
	public struct HLHNPCChatRecord
	{
		public int npcId;
		public int npcDialogGroupID;

		public HLHNPCChatRecord(int npcId,int npcDialogGroupID){
			this.npcId = npcId;
			this.npcDialogGroupID = npcDialogGroupID;
		}
	}
		

	[System.Serializable]
	public class HLHDialog{

		public int dialogId;

		public string dialogContent;

		public List<int> choiceIds;

	}
   
	[System.Serializable]
	public struct HLHPropertyPromotion{
		public PropertyType propertyType;
		public int promotion;
		public int promotionPrice;
		public HLHPropertyPromotion(PropertyType propertyType,int promotion,int promotionPrice){
			this.propertyType = propertyType;
			this.promotion = promotion;
			this.promotionPrice = promotionPrice;
		}


      
		public string GetPropertyPromotionTint(){
			string propertyPromotionTint = string.Empty;
			switch(propertyType){
				case PropertyType.MaxHealth:
					propertyPromotionTint = string.Format("最大生命+{0}",promotion);
					break;
				case PropertyType.MaxMana:
					propertyPromotionTint = string.Format("最大魔法+{0}", promotion);
					break;
				case PropertyType.Attack:
					propertyPromotionTint = string.Format("物理攻击+{0}", promotion);
					break;
				case PropertyType.MagicAttack:
					propertyPromotionTint = string.Format("魔法攻击+{0}", promotion);
					break;
				case PropertyType.Armor:
					propertyPromotionTint = string.Format("护甲+{0}", promotion);
					break;
				case PropertyType.MagicResist:
					propertyPromotionTint = string.Format("抗性+{0}", promotion);
					break;
				case PropertyType.ArmorDecrease:
					propertyPromotionTint = string.Format("护甲穿透+{0}", promotion);
					break;
				case PropertyType.MagicResistDecrease:
					propertyPromotionTint = string.Format("抗性穿透+{0}", promotion);
					break;
				case PropertyType.Crit:
					propertyPromotionTint = string.Format("暴击+{0}%", ((float)promotion / 10).ToString("F1"));
					break;
				case PropertyType.Dodge:
					propertyPromotionTint = string.Format("闪避+{0}%", ((float)promotion / 10).ToString("F1"));
					break;
				case PropertyType.CritHurtScaler:
					propertyPromotionTint = string.Format("暴击倍率+{0}%", ((float)promotion / 10).ToString("F1"));
					break;
				case PropertyType.ExtraGold:
					propertyPromotionTint = string.Format("额外金钱+{0}", promotion);
					break;
				case PropertyType.ExtraExperience:
					propertyPromotionTint = string.Format("额外经验+{0}", promotion);
					break;
				case PropertyType.HealthRecovery:
					propertyPromotionTint = string.Format("生命回复+{0}", promotion);
					break;
				case PropertyType.MagicRecovery:
					propertyPromotionTint = string.Format("魔法回复+{0}", promotion);
					break;
				case PropertyType.MoveSpeed:
					propertyPromotionTint = string.Format("移动速度+{0}", promotion);
					break;               
			}

			return propertyPromotionTint;

		}
	}
   
	public enum HLHRewardType{
		Gold,
		Item
	}


	[System.Serializable]
	public struct HLHNPCReward{

		public HLHRewardType rewardType;

		// 奖励的数值（金钱-数值，物品-物品id，属性-属性类型，经验-数值）
		public int rewardValue;

		// 如果奖励的是物品，则该数据代表物品数量，如果奖励的是属性，则该数据代表属性变化值
		public int attachValue;

		public HLHNPCReward(HLHRewardType rewardType,int rewardValue,int attachValue){
			this.rewardType = rewardType;
			this.rewardValue = rewardValue;
			this.attachValue = attachValue;
		}

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

		private Item itemAsGoods;// 商品
		private int goodsPrice;// 商品价格

		public Item GetGoodsItem(){

			if(itemAsGoods != null){
				return itemAsGoods;
			}
         
			itemAsGoods = Item.NewItemWith (goodsId, 1);


			if(itemAsGoods.itemType == ItemType.Equipment){

				Equipment equipment = itemAsGoods as Equipment;

				switch (equipmentQuality)
                {
                    case 0:
						equipment.ResetPropertiesByQuality(EquipmentQuality.Gray);
                        break;
                    case 1:
						equipment.ResetPropertiesByQuality(EquipmentQuality.Blue);
                        break;
                    case 2:
						equipment.ResetPropertiesByQuality(EquipmentQuality.Gold);
                        break;
                }

			}

			if(priceFloat <= float.Epsilon){
				goodsPrice = itemAsGoods.price;
			}else{
				goodsPrice = Mathf.RoundToInt(itemAsGoods.price * priceFloat);
			}
           
			return itemAsGoods;
		}

		public int GetGoodsPrice(){
			return goodsPrice;
		}
        

		//********************************************************************************* end ***************************************************************************//


	}


}
