using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

	using UnityEngine.UI;

	public delegate void CallBackWithTransform(Transform trans);
	
	public class MonstersDisplayView : ZoomHUD {

		private int currentDisplayIndex;

		public Transform monsterUIContainer;

		public Text monsterNameText;

		public Text monsterMaxHealthText;

		public HLHFillBar monsterAttackBar;
		public HLHFillBar monsterMagicAttackBar;
		public HLHFillBar monsterArmorBar;
		public HLHFillBar monsterMagicResistBar;

		public Text monsterAttackText;
		public Text monsterMagicAttackText;
		public Text monsterArmorText;
		public Text monsterMagicResistText;

		public Text monsterExperienceText;
		public Text monsterGoldText;
		public Text monsterCritText;
		public Text monsterDodgeText;
		public Text monsterAttackSpeedText;
		public Text monsterEvaluateText;
		public Text monsterArmorDecreaseText;
		public Text monsterMaigcResistDecreaseText;

		public Text monsterStoryText;

		public Text pageText;

		private CallBackWithTransform quitCallBack;
        
		private List<MonsterDataWithUIDipslay> monstersWithUI = new List<MonsterDataWithUIDipslay>();

		public HelpViewController propertyHelpHUD;      



		public void InitMonsterDisplayView(List<MonsterDataWithUIDipslay> monstersWithUI){

			this.monstersWithUI = monstersWithUI;
         
			for (int i = 0; i < monstersWithUI.Count; i++)
            {

                MonsterDataWithUIDipslay monsterDataWithUIDipslay = monstersWithUI[i];

                Transform monsterUI = monsterDataWithUIDipslay.monsterUI;

                monsterUI.SetParent(monsterUIContainer);

                monsterUI.localPosition = monsterDataWithUIDipslay.monsterUIInfo.localPosition;
                monsterUI.localScale = monsterDataWithUIDipslay.monsterUIInfo.localScale;
            }

		}

        
		public void SetUpmonstersDisplayView(CallBackWithTransform quitCallBack){
         
			this.quitCallBack = quitCallBack;

			currentDisplayIndex = 0;
         
			pageText.text = string.Format("{0} / {1}", 1, monstersWithUI.Count);

			for (int i = 0; i < monstersWithUI.Count; i++)
            {

                MonsterDataWithUIDipslay monsterDataWithUIDipslay = monstersWithUI[i];

                Transform monsterUI = monsterDataWithUIDipslay.monsterUI;

                monsterUI.SetParent(monsterUIContainer);

                monsterUI.localPosition = monsterDataWithUIDipslay.monsterUIInfo.localPosition;
                monsterUI.localScale = monsterDataWithUIDipslay.monsterUIInfo.localScale;
            }

			this.gameObject.SetActive(true);

			SetUpMonsterUIAndInfo(currentDisplayIndex);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();
			StartCoroutine(zoomCoroutine);
                     

		}

		private void SetUpMonsterUIAndInfo(int index){
			
			MonsterDataWithUIDipslay monsterDataWithUIDipslay = monstersWithUI[index];

         
			monsterDataWithUIDipslay.monsterUI.gameObject.SetActive(true);

			MonsterData monsterData = monsterDataWithUIDipslay.monsterData;

			monsterNameText.text = monsterData.monsterName;

			monsterMaxHealthText.text = monsterData.originalMaxHealth.ToString();

			monsterAttackText.text = monsterData.originalAttack.ToString();
			monsterMagicAttackText.text = monsterData.originalMagicAttack.ToString();
			monsterArmorText.text = monsterData.originalArmor.ToString();
			monsterMagicResistText.text = monsterData.originalMagicResist.ToString();

			int attackTransformTo = TransformToBarFillAmout(monsterData.originalAttack, 1000);
			int magicAttackTramsformTo = TransformToBarFillAmout(monsterData.originalMagicAttack, 1000);
			int armorTransformto = TransformToBarFillAmout(monsterData.originalArmor, 1000);
			int magicResistTransformTo = TransformToBarFillAmout(monsterData.originalMagicResist, 1000);

			monsterAttackBar.InitHLHFillBar(1000,attackTransformTo);
			monsterMagicAttackBar.InitHLHFillBar(1000,magicAttackTramsformTo);
			monsterArmorBar.InitHLHFillBar(1000,armorTransformto);
			monsterMagicResistBar.InitHLHFillBar(1000,magicResistTransformTo);
            
			monsterGoldText.text = monsterData.rewardGold.ToString();
			monsterExperienceText.text = monsterData.rewardExperience.ToString();         
			monsterCritText.text = string.Format("{0}%", (int)(monsterData.originalCrit * 100));
			monsterDodgeText.text = string.Format("{0}%", (int)(monsterData.originalDodge * 100));
			switch(monsterData.attackSpeedLevel){
				case 0:
					monsterAttackSpeedText.text = "极快";
					break;
				case 1:
					monsterAttackSpeedText.text = "快速";
					break;
				case 2:
					monsterAttackSpeedText.text = "中速";
					break;
				case 3:
					monsterAttackSpeedText.text = "慢速";               
					break;
				case 4:
					monsterAttackSpeedText.text = "极慢";   
					break;
			}

			switch(monsterData.mosnterEvaluate){
				case 0:
					monsterEvaluateText.text = "普通";
					break;
				case 1:
					monsterEvaluateText.text = "精英";
					break;
				case 2:
					monsterEvaluateText.text = "Boss";
					break;
			}

			monsterArmorDecreaseText.text = monsterData.originalArmorDecrease.ToString();
			monsterMaigcResistDecreaseText.text = monsterData.originalMagicResistDecrease.ToString();
            
			monsterStoryText.text = monsterData.monsterStory;


		}


        /// <summary>
        /// 将怪物属性值转换为bar的填充数
        /// </summary>
        /// <returns>The to bar fill amout.</returns>
        /// <param name="value">Value.</param>
		private int TransformToBarFillAmout(int value,int maxValue){

			return (int)((float)Mathf.Sqrt(value) * maxValue / 32);

		}



		public void OnNextButtonClick(){

			if (currentDisplayIndex >= monstersWithUI.Count - 1)
            {
                return;
            }
                 
			currentDisplayIndex++;
         
			monstersWithUI[currentDisplayIndex - 1].monsterUI.gameObject.SetActive(false);

			pageText.text = string.Format("{0} / {1}", currentDisplayIndex + 1, monstersWithUI.Count);

			SetUpMonsterUIAndInfo(currentDisplayIndex);

		}

		public void OnLastButtonClick(){

			if (currentDisplayIndex <= 0)
            {
                return;
            }

			currentDisplayIndex--;

			monstersWithUI[currentDisplayIndex + 1].monsterUI.gameObject.SetActive(false);

            pageText.text = string.Format("{0} / {1}", currentDisplayIndex + 1, monstersWithUI.Count);

            SetUpMonsterUIAndInfo(currentDisplayIndex);


		}

		public void QuitMonsterDisplayView(){

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

         
			zoomCoroutine = HUDZoomOut(delegate {
				if(quitCallBack != null){
					quitCallBack(monsterUIContainer);
                }
			});

			StartCoroutine(zoomCoroutine);

		}

		public void OnPropertyHelpButtonClick(){

			propertyHelpHUD.SetUpHelpView();

		}
        
    }
}
