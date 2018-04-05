using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class BattleMonsterUIController: BattleAgentUIController {


		public Text monsterNameText;

		public Monster monster;

		public Transform monsterStatusPlane;

		public Text attackText;
		public Text magicAttackText;
		public Text armorText;
		public Text magicResistText;
	

		public void SetUpMonsterStatusPlane(Monster monster){
			
			monsterNameText.text = monster.agentName;

			healthBar.InitHLHFillBar (monster.maxHealth, monster.health);

			attackText.text = monster.attack.ToString ();
			magicAttackText.text = monster.magicAttack.ToString ();
			armorText.text = monster.armor.ToString ();
			magicResistText.text = monster.magicResist.ToString ();

			monsterStatusPlane.gameObject.SetActive (true);
		}





		public override void UpdateAgentStatusPlane (){

			UpdateHealthBarAnim (monster);

			UpdateSkillStatusPlane (monster);

			attackText.text = monster.attack.ToString ();
			magicAttackText.text = monster.magicAttack.ToString ();
			armorText.text = monster.armor.ToString ();
			magicResistText.text = monster.magicResist.ToString ();

		}



		public override void QuitFightPlane ()
		{

			statusTintPool.AddChildInstancesToPool (statusTintContainer);
			monsterStatusPlane.gameObject.SetActive (false);
		}
//		public void PlayMonsterDieAnim(BattleAgentController baCtr,CallBack<Transform> cb,Transform[] transArray){
//
//			baCtr.GetComponent<SpriteRenderer> ().DOFade (0, 0.5f).OnComplete(()=>{
//				baCtr.gameObject.SetActive(false);
//
//				if(cb != null){
//					cb(transArray);
//				}
//			});
//		}


		void OnDestroy(){
//			fightTextManager = null;
//			fightTextPool = null;
//			statusTintPool = null;
		}

	}
}
