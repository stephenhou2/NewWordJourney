using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	public class MapNPC : MapEvent {

		[HideInInspector]public HLHNPC npc;
		private bool hasNpcDataLoaded;

		private BattleMonsterController mBmCtr;
		private BattleMonsterController bmCtr{
			get{
				if (mBmCtr == null) {
					mBmCtr = GetComponent<BattleMonsterController> ();
				}

				return mBmCtr;
			}
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}


		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			bmCtr.SetSortingOrder (-(int)transform.position.y);


//				int npcId = int.Parse (KVPair.GetPropertyStringWithKey ("npcID", attachedInfo.properties));

//				int npcId = Random.Range (0, 13);
//				#warning 这里暂时使用id为0的npc作为测试数据

			if (!hasNpcDataLoaded) {
				
				int npcId = 1;

				npc = GameManager.Instance.gameDataCenter.LoadNpc (npcId);

				hasNpcDataLoaded = true;
			}



			bmCtr.PlayRoleAnim ("wait", 0, null);

			bc2d.enabled = true;

		}

		private void InitFirstThreeGoodsGroup(string goodsDataString,List<Item> goodsList){

			string[] goodsGroupStrings = goodsDataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < 3; i++) {

				string[] goodsStrings = goodsGroupStrings [i].Split (new char[]{ '/' }, System.StringSplitOptions.RemoveEmptyEntries);

				int randomSeed = Random.Range (0, goodsStrings.Length);

				int randomGoodsId = int.Parse (goodsStrings [randomSeed]);

				Item itemAsGoods = Item.NewItemWith (randomGoodsId, 1);

				goodsList.Add (itemAsGoods);

			}

		}

		private void InitLastTwoGoodsGroup(string goodsDataString,List<Item> goodsList){

			string[] goodsGroupStrings = goodsDataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < 2; i++) {

				int randomSeed = Random.Range (0, goodsGroupStrings.Length);

				int randomGoodsId = int.Parse (goodsGroupStrings [randomSeed]);

				Item itemAsGoods = Item.NewItemWith (randomGoodsId, 1);

				goodsList.Add (itemAsGoods);

			}

		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			MapEventTriggered (true, bp);
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.ShowNPCPlane (this);
		}
	}
}
