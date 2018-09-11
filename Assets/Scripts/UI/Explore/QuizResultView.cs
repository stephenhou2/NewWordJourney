using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class QuizResultView : ZoomHUD
    {

		public Text generalInfoText;

		public Text correctPercentageText;

		public Text healthGainText;

		public Text manaGainText;

		public Text maxContinousHitText;

		public Text attackGainText;

		public Text magicAttackGainText;

		public Text armorGainText;

		public Text magicResistGainText;

		public Text dodgeGainText;

		public Text critGainText;

		private CallBack confirmResultCallBack;

		public void SetUpQuizResultView(WordRecordQuizResult result,CallBack confirmResultCallBack){

			this.confirmResultCallBack = confirmResultCallBack;

			int quizWordCount = result.correctWordsCount + result.wrongWordsCount;
            
			generalInfoText.text = string.Format("<color=orange>复习单词数: {0}个</color>\n正确单词数: {1}个\n错误单词数: {2}个",
			                                     quizWordCount,result.correctWordsCount,result.wrongWordsCount);

            
			correctPercentageText.text = string.Format("  正确率:\n<color=white><size=70>{0}%</size></color>",
			                                           result.correctWordsCount * 100 / quizWordCount);

			healthGainText.text = string.Format("+{0}",result.healthGainTotal);

			manaGainText.text = string.Format("+{0}",result.manaGainTotal);

			maxContinousHitText.text = string.Format("最高连击数: {0}",result.maxContinousHitCount);

			int attackGain = 0;
			int magicAttackGain = 0;
			int armorGain = 0;
			int magicResistGain = 0;
			float dodgeGain = 0;
			float critGain = 0;

			for (int i = 0; i < result.extraPropertySets.Count;i++){
				PropertySet ps = result.extraPropertySets[i];

				switch(ps.type){
					case PropertyType.Attack:
						attackGain += Mathf.RoundToInt(ps.value);
						break;
					case PropertyType.MagicAttack:
						magicAttackGain += Mathf.RoundToInt(ps.value);
						break;
					case PropertyType.Armor:
						armorGain += Mathf.RoundToInt(ps.value);
						break;
					case PropertyType.MagicResist:
						magicResistGain += Mathf.RoundToInt(ps.value);
						break;
					case PropertyType.Dodge:
						dodgeGain += ps.value;
						break;
					case PropertyType.Crit:
						critGain += ps.value;
						break;
				}
			}

			Player.mainPlayer.health += result.healthGainTotal;
			Player.mainPlayer.mana += result.manaGainTotal;

			ExploreManager.Instance.UpdatePlayerStatusPlane();



			if(attackGain > 0){
				attackGainText.text = string.Format("+{0}",attackGain);
				Player.mainPlayer.attack += attackGain;
				Player.mainPlayer.originalAttack += attackGain;
			}else{
				attackGainText.text = "--";
			}
            
			if(magicAttackGain > 0){
				magicAttackGainText.text = string.Format("+{0}",magicAttackGain);
				Player.mainPlayer.magicAttack += magicAttackGain;
				Player.mainPlayer.originalMagicAttack += magicAttackGain;
			}else{
				magicAttackGainText.text = "--";
			}

			if(armorGain > 0){
				armorGainText.text = string.Format("+{0}",armorGain);
				Player.mainPlayer.armor += armorGain;
				Player.mainPlayer.originalArmor += armorGain;
			}else{
				armorGainText.text = "--";
			}

			if(magicResistGain > 0){
				magicResistGainText.text = string.Format("+{0}",magicResistGain);
				Player.mainPlayer.magicResist += magicResistGain;
				Player.mainPlayer.originalMagicResist += magicResistGain;
			}else{
				magicResistGainText.text = "--";
			}

			if(dodgeGain > float.Epsilon){
				dodgeGainText.text = string.Format("+{0}%",((dodgeGain + float.Epsilon) * 100).ToString("0.0"));
				Player.mainPlayer.dodge += dodgeGain;
				Player.mainPlayer.originalDodge += dodgeGain;
			}else{
				dodgeGainText.text = "--";
			}

			if(critGain > float.Epsilon){
				critGainText.text = string.Format("+{0}%",((critGain + float.Epsilon) * 100).ToString("0.0"));
				Player.mainPlayer.crit += critGain;
				Player.mainPlayer.originalCrit += critGain;
			}else{
				critGainText.text = "--";
			}

			this.gameObject.SetActive(true);

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);


			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);


		}


		public void OnConfirmQuizResultButtonClick(){

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);


			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(confirmResultCallBack);

			StartCoroutine(zoomCoroutine);

		}
        
    }

}
