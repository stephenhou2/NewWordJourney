using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Data;

namespace WordJourney{
	

	public enum ItemType{
		Equipment,
		Consumables,
        PropertyGemstone,
        SkillScroll,
        SpecialItem,
        SpellItem
	}

	[System.Serializable]
	public abstract class Item {

		public string itemName;
		public string itemDescription;
		public string spriteName;
		public int itemId;
		public ItemType itemType;

		public int itemCount;
		public int price;

		public bool isNewItem = true;


		/// <summary>
		/// 空构造函数
		/// </summary>
		public Item(){ 
		
		}




		/// <summary>
		/// 通过物品id和数量初始化物品
		/// </summary>
		public static Item NewItemWith(int itemId,int itemCount){

			Item newItem = null;

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
//			itemPropertyDescription = itemModel.itemPropertyDescription;
			spriteName = itemModel.spriteName;
			itemType = itemModel.itemType;
			price = itemModel.price;

		}
			

		public static Item GetRandomItem(){

			int typeSeed = Random.Range (0, 4);

			int randomItemId = 0;

			switch (typeSeed) {
			case 0:
				randomItemId = Random.Range (0, 60);
				break;
			case 1:
				randomItemId = Random.Range (100, 120);
				break;
			case 2:
				randomItemId = Random.Range (200, 260);
				break;
			case 3:
				randomItemId = Random.Range (400, 460);
				break;
			}
			return NewItemWith (randomItemId, 1);
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
			



		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		//public abstract string GetItemTypeString ();

		/// <summary>
		/// 获取物品属性字符串
		/// </summary>
		/// <returns>The item properties string.</returns>
//		public abstract string GetItemBasePropertiesString ();


		public override string ToString ()
		{
			return string.Format ("[Item]:" + itemName + "[\nItemDesc]:" + itemDescription);
		}

	}

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
