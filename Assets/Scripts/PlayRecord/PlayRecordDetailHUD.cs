using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
   
	public class PlayRecordDetailHUD : ZoomHUD
    {
		public Text recordIndexText;

		public Text exploreEndAt;

        public Text exploreMaxLevelText;
              
        public Text totalLearnedWordCountText;

		public Text totalExploreTimeText;

		public Text correctPercentageText;

		public Text totalDefeatMonsterCount;

		public Text evaluateStringText;

		public Text maxHealtRecordText;

		public Text maxManaRecordText;

		public Text attackRecordText;

		public Text magicAttackRecordText;

		public Text armorRecordText;

		public Text magicResistRecordText;

		public Text armorDecreaseRecordText;

		public Text magicResistDecreaseRecordText;

		public Text dodgeRecordText;

		public Text critRecordText;

		public Text extraGoldRecordText;

		public Text extraExperienceRecordText;

		public Text healthRecoveryRecordText;

		public Text magicRecoveryRecordText;

		public EquipmentCellInRecord equipmentCellModel;

		public InstancePool equipmentCellPool;

		public Transform equipmentCellContainer;

		public SkillCellInRecord skillCellModel;

		public InstancePool skillCellPool;

		public Transform skillCellContainer;

		public void SetUpPlayRecordDetailHUD(PlayRecord playRecord,int recordIndex)
		{         
			recordIndexText.text = (recordIndex + 1).ToString();
         
			if(playRecord.finishGame){
				exploreEndAt.text = "获得艾尔文的宝藏";
			}else{
				exploreEndAt.text = string.Format("失败于:{0}", playRecord.dieFrom);
			}

			exploreMaxLevelText.text = string.Format("最大探索层数:{0}", playRecord.maxExploreLevel);

			totalExploreTimeText.text = string.Format("探索时间:{0}天", playRecord.totalExploreDays);

			totalLearnedWordCountText.text = string.Format("总计学习单词数:{0}", playRecord.totalLearnedWordCount);

			correctPercentageText.text = string.Format("正确率:{0}%", playRecord.learnCorrectPercentageX100);

			totalDefeatMonsterCount.text = string.Format("总计击败怪物数:{0}", playRecord.totalDefeatMonsterCount);
                     
			evaluateStringText.text = string.Format("评级:{0}", playRecord.evaluateString);

			maxHealtRecordText.text = playRecord.maxHealth.ToString();

			maxManaRecordText.text = playRecord.maxMana.ToString();

			attackRecordText.text = playRecord.attack.ToString();

			magicAttackRecordText.text = playRecord.magicAttack.ToString();

			armorRecordText.text = playRecord.armor.ToString();

			magicResistRecordText.text = playRecord.magicResist.ToString();

			armorDecreaseRecordText.text = playRecord.armorDecrease.ToString();

			magicResistDecreaseRecordText.text = playRecord.magicResistDecrease.ToString();

			dodgeRecordText.text = string.Format("{0}%",(playRecord.dodge * 100).ToString("0.0"));

			critRecordText.text = string.Format("{0}%", (playRecord.crit * 100).ToString("0.0"));

			extraGoldRecordText.text = playRecord.extraGold.ToString();

			extraExperienceRecordText.text = playRecord.extraExperience.ToString();

			healthRecoveryRecordText.text = playRecord.healthRecovery.ToString();

			magicRecoveryRecordText.text = playRecord.magicRecovery.ToString();

			equipmentCellPool.AddChildInstancesToPool(equipmentCellContainer);

			if(playRecord.equipedEquipments != null){

				for (int i = 0; i < playRecord.equipedEquipments.Count; i++)
                {
					Equipment equipment = playRecord.equipedEquipments[i];

					if(equipment.itemId < 0){
						continue;
					}

					EquipmentCellInRecord cellInRecord = equipmentCellPool.GetInstance<EquipmentCellInRecord>(equipmentCellModel.gameObject,
																											  equipmentCellContainer);

					cellInRecord.SetUpEquipmentCellInRecord(equipment);
               
                }            
			}

			skillCellPool.AddChildInstancesToPool(skillCellContainer);

			if(playRecord.learnedSkillRecords != null){

				for (int i = 0; i < playRecord.learnedSkillRecords.Count;i++){

					SkillModel skillModel = playRecord.learnedSkillRecords[i];

					int skillLevel = skillModel.skillLevel;

					Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find(delegate (Skill obj)
					{
						return obj.skillId == skillModel.skillId;
					});

					if(skill == null){
						continue;
					}

					SkillCellInRecord cellInRecord = skillCellPool.GetInstance<SkillCellInRecord>(skillCellModel.gameObject, skillCellContainer);

					cellInRecord.SetUpSkillCellInRecord(skill,skillLevel);

				}

			}

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

         
		}


		public void QuitPlayRecordDetailView(){

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}
        
    }
}

