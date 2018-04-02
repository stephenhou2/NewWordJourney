using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace WordJourney
{
	
//	回复类别 - 回复生命或者回复魔法的
//	点金符石 - 可以将不是金色品质的装备提升为金色属性
//	重铸符石 - 可以把装备进行重置，随机变化为灰色（50%），蓝色（30%），金色（20%）。
//	退魔卷轴 - 清洗掉装备上附加的技能
//	隐身卷轴 - 进入隐身状态持续20步，不会被怪物发现。


	public enum ConsumablesType{
		ShuXingTiSheng,
		DianJinShi,
		ChongZhuShi,
		XiaoMoJuanZhou,
		YinShenJuanZhou
	}


	[System.Serializable]
	public class Consumables : Item {


		public static int minProducableConsumablesId = 300;
		public static int maxProducableConsumablesId = 400;


		public ConsumablesType type;

		public int healthGain;
		public int manaGain;
		public int experienceGain;



//		public bool isOnlyExploreUse;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		public Consumables(ConsumablesModel consumablesModel,int itemCount){

			// 初始化物品基础属性
			InitBaseProperties (consumablesModel);

			this.itemType = ItemType.Consumables;
			this.itemCount = itemCount;

			this.healthGain = consumablesModel.healthGain;
			this.manaGain = consumablesModel.manaGain;
			this.experienceGain = consumablesModel.experienceGain;

			this.type = consumablesModel.type;

		}


		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public override string GetItemTypeString ()
		{
			return "消耗品";
		}

		void OnDestroy(){
//			attachedSkillInfos = null;
//			itemInfosForProduce = null;
		}



	}




}