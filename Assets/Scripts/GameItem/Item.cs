using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Data;

namespace WordJourney{
	
    /// <summary>
    /// 物品类型
    /// </summary>
	public enum ItemType{
		Equipment,//装备
		Consumables,//消耗品
        PropertyGemstone,//属性宝石
        SkillScroll,//技能卷轴
        SpecialItem,//特殊物品【点金石，重铸石，退魔卷轴，开关等等】
        SpellItem//拼写物品
	}

    /// <summary>
    /// 物品类
    /// </summary>
	[System.Serializable]
	public abstract class Item {

        // 物品名称
		public string itemName;
        // 物品描述
		public string itemDescription;
        // 物品图片名称
		public string spriteName;
        // 物品id
		public int itemId;
        // 物品类型
		public ItemType itemType;

        // 物品数量
		public int itemCount;
        // 物品价格
		public int price;
        // 标记是否是新获得的物品
		public bool isNewItem = true;


		/// <summary>
		/// 空构造函数
		/// </summary>
		public Item(){ 
		
		}




		/// <summary>
		/// 通过物品id和数量初始化物品
		/// 【0-299】装备
		/// 【300-399】消耗品
		/// 【400-499】属性宝石
		/// 【500-599】技能卷轴
		/// 【600-699】特殊物品
		/// </summary>
		public static Item NewItemWith(int itemId,int itemCount){

			Item newItem = null;

            // 逻辑上相同：寻找数据模型->使用数据模型创建新物品

			if (itemId < 300)
			{
				EquipmentModel equipmentModel = GameManager.Instance.gameDataCenter.allEquipmentModels.Find(delegate (EquipmentModel obj)
				{
					return obj.itemId == itemId;
				});

				if (equipmentModel == null)
				{
					string error = string.Format("未找到id为{0}的物品", itemId);
					Debug.LogError(error);
				}

				newItem = new Equipment(equipmentModel, itemCount);

			}
			else if (itemId >= 300 && itemId < 400)
			{

				ConsumablesModel cm = GameManager.Instance.gameDataCenter.allConsumablesModels.Find(delegate (ConsumablesModel obj)
				{
					return obj.itemId == itemId;
				});

				if (cm == null)
				{
					string error = string.Format("未找到id为{0}的物品", itemId);
					Debug.LogError(error);
				}

				newItem = new Consumables(cm, itemCount);
			}
			else if (itemId >= 400 && itemId < 500)
			{

				PropertyGemstoneModel propertyGemstoneModel = GameManager.Instance.gameDataCenter.allPropertyGemstoneModels.Find(delegate (PropertyGemstoneModel obj)
				{
					return obj.itemId == itemId;
				});

				if (propertyGemstoneModel == null)
				{
					string error = string.Format("未找到id为{0}的物品", itemId);
					Debug.LogError(error);
				}

				newItem = new PropertyGemstone(propertyGemstoneModel, itemCount);

			}else if(itemId >= 500 && itemId < 600){

				SkillScrollModel skillScrollModel = GameManager.Instance.gameDataCenter.allSkillScrollModels.Find(delegate (SkillScrollModel obj)
				{
					return obj.itemId == itemId;
				});

				if (skillScrollModel == null)
                {
                    string error = string.Format("未找到id为{0}的物品", itemId);
                    Debug.LogError(error);
                }

				newItem = new SkillScroll(skillScrollModel, itemCount);
			}else if(itemId >= 600 && itemId < 700){

				SpecialItemModel specialItemModel = GameManager.Instance.gameDataCenter.allSpecialItemModels.Find(delegate (SpecialItemModel obj)
				{               
					return obj.itemId == itemId;
				});
				if(specialItemModel == null){
					string error = string.Format("未找到id为{0}的物品", itemId);
                    Debug.LogError(error);
				}
				newItem = new SpecialItem(specialItemModel, itemCount);
			}

			return newItem;

		}


		/// <summary>
		/// 初始化基础属性
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		protected void InitBaseProperties(ItemModel itemModel){         
			itemId = itemModel.itemId;
			itemName = itemModel.itemName;
			itemDescription = itemModel.itemDescription;
			spriteName = itemModel.spriteName;
			itemType = itemModel.itemType;
			price = itemModel.price;         
		}
			
      
		/// <summary>
		/// 获取所有游戏物品中的武器类物品
		/// </summary>
		public static List<Equipment> GetAllEquipments(){

			List<EquipmentModel> allItemModels = GameManager.Instance.gameDataCenter.allEquipmentModels;

			List<Equipment> allEquipment = new List<Equipment> ();

			for (int i = 0; i < allItemModels.Count; i++) {

				EquipmentModel itemModel = allItemModels [i];

				if (itemModel.itemType == ItemType.Equipment) {
					Equipment equipment = new Equipment (itemModel,1);
					allEquipment.Add (equipment);
				}
					
			}
			return allEquipment;
		}
			


		public override string ToString ()
		{
			return string.Format ("[Item]:" + itemName);
		}

	}

    /// <summary>
    /// 属性+属性值组合
    /// </summary>
	[System.Serializable]
	public struct PropertySet{

		public PropertyType type;
		public float value;

		public PropertySet(PropertyType type,float value){
			this.type = type;
			this.value = value;
		}
	
	}



}
