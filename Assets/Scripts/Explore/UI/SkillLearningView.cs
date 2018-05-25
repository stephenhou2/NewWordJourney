using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class SkillLearningView : ZoomHUD
    {

        public InstancePool npcSkillsPool;

        public Transform npcSkillsContainer;

        public SkillDetailInNPC skillDetailModel;

		private List<SkillDetailInNPC> allSkillDetails = new List<SkillDetailInNPC>();

		private Skill currentSelectedSkill;
		private List<int> skillIds;
		private CallBack quitCallBack;
        
		public TintHUD tintHUD;

		public void SetUpSkillLearningView(List<int> skillIds,CallBack quitCallBack){

			gameObject.SetActive(true);

			this.skillIds = skillIds;

			this.quitCallBack = quitCallBack;

            npcSkillsPool.AddChildInstancesToPool(npcSkillsContainer);

			for (int i = 0; i < skillIds.Count; i++)
            {

                SkillDetailInNPC skillDetail = npcSkillsPool.GetInstance<SkillDetailInNPC>(skillDetailModel.gameObject, npcSkillsContainer);

                Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find(delegate (Skill obj)
                {
					return obj.skillId == skillIds[i];               
                });

				int index = i;

				skillDetail.SetupSkillDetailInNPC(skill,SkillClickCallBack);

				allSkillDetails.Add(skillDetail);

            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);


        }
        
		private void SkillClickCallBack(Skill skill){
        
			for (int i = 0; i < skillIds.Count;i++){

				SkillDetailInNPC skillDetail = allSkillDetails[i];
               
				skillDetail.SetUpSelectedIcon(skillIds[i] == skill.skillId);
               
			}
                     
			currentSelectedSkill = skill;
		}

		private void UpdateSkillsView(){

			for (int i = 0; i < skillIds.Count;i++){

				int skillId = skillIds[i];

				bool skillLearned = Player.mainPlayer.CheckSkillHasLearned(skillId);

				allSkillDetails[i].learnedTint.enabled = skillLearned;            
			}

		}

		public void OnLearnSkillClick(){

			if (currentSelectedSkill == null)
			{
				return;
			}

			if(Player.mainPlayer.totalGold < 500)
			{
				tintHUD.SetUpSingleTextTintHUD("金币不足");               
			}else if(Player.mainPlayer.CheckSkillFull()){
				tintHUD.SetUpSingleTextTintHUD("只能学习6个技能");
			}else{
				Player.mainPlayer.totalGold -= 500;

				Player.mainPlayer.LearnSkill(currentSelectedSkill);

				UpdateSkillsView();

			}         
		}

		public void OnBackButtonClick(){

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

			if(quitCallBack != null){
				quitCallBack();
			}

		}

    }   
}
