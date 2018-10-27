using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    /// <summary>
    /// npc数据类
    /// </summary>
	[System.Serializable]
	public class HLHNPC {

        // npc id
		public int npcId;
        // npc 名称
		public string npcName;
        // 是否有交谈功能
		public bool isChatTriggered;
        // 是否有交易功能
		public bool isTradeTriggered;
        // 是否有传送功能
		public bool isTransportTriggered;
        // 是否有教授技能的功能
		public bool isLearnSkillTriggered;
        // 是否有提升属性的功能【废弃】
		public bool isPropertyPromotionTriggered;
        // 是否有镶嵌宝石的功能
		public bool isAddGemStoneTriggered;
        
        // 提升的属性列表【废弃】
		public List<HLHPropertyPromotion> propertyPromotionList = new List<HLHPropertyPromotion>();

        // 指定关卡一定会出现的对话组列表
		public List<HLHDialogGroup> levelDefiniteDialogGroups = new List<HLHDialogGroup>();

        // 未指定关卡的对话组列表
		public List<HLHDialogGroup> levelUndefiniteDialogGroups = new List<HLHDialogGroup>();

        // 传送层级列表
		public List<int> transportLevelList = new List<int>();
      
        // 商品列表
		public List<HLHNPCGoodsGroup> npcGoodsGroupList = new List<HLHNPCGoodsGroup> ();

        // 
		public List<HLHDialog> regularGreetings = new List<HLHDialog> ();

		// 单词选择正确时的对话组
		//public HLHDialogGroup wordRightDialogGroup;
		// 单词选择错误时的对话组
		//public HLHDialogGroup wordWrongDialogGroup;

		// 标示当npc数据变化时是否需要保存npc数据
		public bool saveOnChange;

		// 本层触发的对话组的记录
		private HLHDialogGroup dialogGroupRecord;

        // 在售商品记录列表
		public List<HLHNPCGoods> goodsInSellRecord = new List<HLHNPCGoods>();

        // 是否已经初始化过商品数据
		public bool isGoodsInitialized;
      
        // npc技能id数组
		public List<int> npcSkillIds = new List<int>();

        // 是否已经教授过技能
		public bool hasTeachedASkill = false;
        
        /// <summary>
        /// 获取一个可用的传送层数
        /// </summary>
        /// <returns>The valid travel level identifiers.</returns>
		public List<int> GetValidTravelLevelIds(){

			List<int> validTravelLevelIds = new List<int>();

			for (int i = 0; i < transportLevelList.Count;i++){

				int levelId = transportLevelList[i];

				if(levelId >= Player.mainPlayer.maxUnlockLevelIndex - 10 
				   && levelId <= Player.mainPlayer.maxUnlockLevelIndex 
				   && levelId != Player.mainPlayer.currentLevelIndex){
					validTravelLevelIds.Add(levelId);
				}


			}

			return validTravelLevelIds;

            
		}

		/// <summary>
		/// 获取合适的对话组【本层指定必须出现的优先级高】
		/// </summary>
		/// <returns>The qulified dialog group.</returns>
		/// <param name="player">Player.</param>
		public HLHDialogGroup FindQulifiedDialogGroup(Player player){
        

			HLHDialogGroup targetDg = null;

         
            // 查询所有指定层级必须出现的对话组中，有没有指定本层必须出现的
			for (int i = 0; i < levelDefiniteDialogGroups.Count; i++) {
                
				HLHDialogGroup dg = levelDefiniteDialogGroups [i];

				if(dg.triggerLevel == player.currentLevelIndex){

                    // 如果指定本层必须出现，则检查本层是否已经完成了该对话组
					if(CheckDialogGroupFinished(dg)){
						break;
					}

					targetDg = dg;

					dialogGroupRecord = dg;
                                   
					return targetDg;
				}

			}

            // 如果没有必须出现的对话组，则随机一个对话组
			int randomSeed = Random.Range(0, levelUndefiniteDialogGroups.Count);

			targetDg = levelUndefiniteDialogGroups[randomSeed];

			dialogGroupRecord = targetDg;
                     
			return targetDg;

		}


        /// <summary>
        /// 检查传入的对话组是否已经完成全部交谈的话
        /// </summary>
        /// <returns><c>true</c>, if dialog group finished was checked, <c>false</c> otherwise.</returns>
        /// <param name="dialogGroup">Dialog group.</param>
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
        
        /// <summary>
        /// 检查商品是否已经卖完了
        /// </summary>
        /// <returns><c>true</c>, if golld sold out was checked, <c>false</c> otherwise.</returns>
		public bool CheckGolldSoldOut(){
			return isGoodsInitialized && goodsInSellRecord.Count == 0;
		}

        /// <summary>
        /// 获取本层的商品id列表
        /// </summary>
        /// <returns>The current level goods.</returns>
		public List<HLHNPCGoods> GetCurrentLevelGoods(){

            // 如果已经初始化过，则返回本层的商品记录
			if (isGoodsInitialized) {
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


        /// <summary>
        /// 从可能商品列表中产生随机商品
        /// </summary>
        /// <param name="possibleGoods">Possible goods.</param>
		private void GenerateRandomGoods(List<HLHNPCGoods> possibleGoods){
			
			if (possibleGoods.Count == 0) {
				return;
			}

			int randomSeed = Random.Range (0, possibleGoods.Count);

			HLHNPCGoods goods = possibleGoods [randomSeed];

			goodsInSellRecord.Add (goods);

		}


		/// <summary>
		/// npc卖东西给玩家
		/// </summary>
		/// <param name="goodsIndex">商品在列表中的序号</param>
		public void SoldGoods(int goodsIndex){
            
			if(goodsInSellRecord.Count > 0 && goodsIndex < goodsInSellRecord.Count){
				goodsInSellRecord.RemoveAt(goodsIndex);
			}
		}      

    }

    /// <summary>
    /// 对话组模型
    /// </summary>
	[System.Serializable]
	public class HLHDialogGroup{

        // 对话组id
		public int dialogGroupId;

		// 对话的触发关卡序号【-1代表对话不跟关卡走】
		public int triggerLevel = -1;

        // 所有对话列表
		public List<HLHDialog> dialogs;

        // 对话组结束时是否触发奖励
		public bool isRewardTriggered;

        // 奖励
		public HLHNPCReward reward;

        // 对话组是否完成
		public bool isFinish;

        // 是否是可以多次触发的对话组
		public bool isMultiTimes;
      

	}

    /// <summary>
    /// 聊天记录
    /// </summary>
	[System.Serializable]
	public struct HLHNPCChatRecord
	{
		// npc id 
		public int npcId;
        // 对话组id
		public int npcDialogGroupID;

        //构造函数
		public HLHNPCChatRecord(int npcId,int npcDialogGroupID){
			this.npcId = npcId;
			this.npcDialogGroupID = npcDialogGroupID;
		}
	}
		
    /// <summary>
    /// 对话模型
    /// </summary>
	[System.Serializable]
	public class HLHDialog{

		public int dialogId;

		public string dialogContent;

		public List<int> choiceIds;

	}
   
    /// <summary>
    /// 属性提升模型
    /// </summary>
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

    /// <summary>
    /// 奖励模型
    /// </summary>
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


    /// <summary>
    /// 商品组模型
    /// </summary>
	[System.Serializable]
	public class HLHNPCGoodsGroup{

        // 每个npc可以卖5个商品，下面分别是5个商品的备选商品id列表
		public List<HLHNPCGoods> goodsList_1= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_2= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_3= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_4= new List<HLHNPCGoods> ();
		public List<HLHNPCGoods> goodsList_5= new List<HLHNPCGoods> ();

	}
		
    /// <summary>
    /// npc商品模型
    /// </summary>
	[System.Serializable]
	public class HLHNPCGoods{

        // 商品id
		public int goodsId;
        // 商品价格浮动
		public float priceFloat;
		public int equipmentQuality;//0:灰色，1:蓝色，2:金色


		//******************************************************************************* start *****************************************************************************//

		private Item itemAsGoods;// 商品
		private int goodsPrice;// 商品价格

        /// <summary>
        /// 获取商品
        /// </summary>
        /// <returns>The goods item.</returns>
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
					case 3:
						equipment.ResetPropertiesByQuality(EquipmentQuality.Purple);
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

        // 获取商品价格
		public int GetGoodsPrice(){
			return goodsPrice;
		}
        

		//********************************************************************************* end ***************************************************************************//


	}


}
