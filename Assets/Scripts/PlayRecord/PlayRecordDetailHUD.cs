using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
   
    /// <summary>
    /// 通关记录详细界面
    /// </summary>
	public class PlayRecordDetailHUD : ZoomHUD
    {
        // 通关文本
		public Text exploreEndAt;
        // 最大探索关卡数
        public Text exploreMaxLevelText;
        // 学习单词总数文本      
        public Text totalLearnedWordCountText;
        // 探索时长文本
		public Text totalExploreTimeText;
        // 正确率文本
		public Text correctPercentageText;
        // 共计击败怪物数量文本
		public Text totalDefeatMonsterCount;
        // 通关评级文本
		public Text evaluateStringText;
        // 属性文本
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
        // 所有装备槽
		public EquipmentCellInRecord[] equipmentCells;
        // 所有技能槽
		public SkillCellInRecord[] skillCells;

        /// <summary>
        /// 初始化通关记录详细页面
        /// </summary>
        /// <param name="playRecord">Play record.</param>
		public void SetUpPlayRecordDetailHUD(PlayRecord playRecord)
		{         
         
			//if(playRecord.finishGame){
				exploreEndAt.text = "获得艾尔文的宝藏";
			//}else{
			//	exploreEndAt.text = string.Format("失败于: {0}", playRecord.dieFrom);
			//}

			exploreMaxLevelText.text = string.Format("最大探索层数: {0}", playRecord.maxExploreLevel);

			totalExploreTimeText.text = string.Format("探索时间: {0}天", playRecord.totalExploreDays);

			totalLearnedWordCountText.text = string.Format("学习单词总数: {0}", playRecord.totalLearnedWordCount);

			correctPercentageText.text = string.Format("总正确率: {0}%", playRecord.learnCorrectPercentageX100);

			totalDefeatMonsterCount.text = string.Format("总计击败怪物: {0}", playRecord.totalDefeatMonsterCount);
                     
			evaluateStringText.text = string.Format("综合评分: <color=orange>{0}</color>", playRecord.evaluateString);

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

			int equipmentCellIndex = 0;


            // 重置所有装备槽
			for (int i = 0; i < equipmentCells.Length;i++){
				
				EquipmentCellInRecord cellInRecord = equipmentCells[i];

				cellInRecord.Reset();

			}

            // 根据通关时的装备创建装备槽
			if(playRecord.equipedEquipments != null){

				for (int i = 0; i < playRecord.equipedEquipments.Count; i++)
                {
					Equipment equipment = playRecord.equipedEquipments[i];

					if(equipment.itemId < 0){
						continue;
					}

					EquipmentCellInRecord cellInRecord = equipmentCells[equipmentCellIndex];

                    // 初始化装备槽
					cellInRecord.SetUpEquipmentCellInRecord(equipment);

					equipmentCellIndex++;
               
                }            
			}
            

            // 重置所有的技能槽
			for (int i = 0; i < skillCells.Length;i++){
				SkillCellInRecord cellInRecord = skillCells[i];
				cellInRecord.Reset();
			}

            // 根据通关时的技能创建技能槽
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


					SkillCellInRecord cellInRecord = skillCells[i];
                    // 初始化技能槽
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

        /// <summary>
        /// 退出通关记录界面
        /// </summary>
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

