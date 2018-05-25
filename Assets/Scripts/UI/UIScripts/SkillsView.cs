using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


	public delegate void CallBackWithPropertyChange(PropertyChange propertyChange);

	public class SkillsView : MonoBehaviour
    {

		public Text leftSkillNumText;

		public Transform queryForgetSkillHUD;

		public SimpleSkillDetail simpleSkillDetailModel;

		public InstancePool simpleSkillDetailPool;

		public Transform simpleSkillDetailContainer;
        
		public SkillDetail skillDetail;

		private Skill currentSelectedSkill;

		public TintHUD tintHUD;

		private CallBackWithPropertyChange skillStatusChangeCallBack;

		public void InitSkillsView(CallBackWithPropertyChange skillStatusChangeCallBack){

			this.skillStatusChangeCallBack = skillStatusChangeCallBack;
		}

		public void SetUpSkillView(){   
			
			gameObject.SetActive(true);

			leftSkillNumText.text = Player.mainPlayer.skillNumLeft.ToString();

			simpleSkillDetailPool.AddChildInstancesToPool(simpleSkillDetailContainer);

			for (int i = 0; i < Player.mainPlayer.allLearnedSkillIds.Count;i++){

				int skillId = Player.mainPlayer.allLearnedSkillIds[i];

				Skill skill= GameManager.Instance.gameDataCenter.allSkills.Find(delegate(Skill obj)
				{
					return obj.skillId == skillId;

				});

				SimpleSkillDetail simpleSkillDetail = simpleSkillDetailPool.GetInstance<SimpleSkillDetail>(simpleSkillDetailModel.gameObject, simpleSkillDetailContainer);

				simpleSkillDetail.SetUpSimpleSkillDetail(skill,OnSkillClick);
            
			}

			if(currentSelectedSkill == null){
				skillDetail.ClearSkillDetail();  
			}else{
				skillDetail.SetupSkillDetail(currentSelectedSkill);
			}
			       

		}

		private void OnSkillClick(Skill skill){
			currentSelectedSkill = skill;
			skillDetail.SetupSkillDetail(skill);
		}
		      
		public void OnUpgradeButtonClick()
        {
			bool skillNumEnough = Player.mainPlayer.skillNumLeft >= currentSelectedSkill.upgradeNum;

			if(skillNumEnough){

				PropertyChange propertyChange = Player.mainPlayer.UpgradeSkill(currentSelectedSkill);

				SetUpSkillView();

				skillStatusChangeCallBack(propertyChange);
            
			}else{

				tintHUD.SetUpSingleTextTintHUD("剩余技能点不足");

			}

        }

        public void OnForgetButtonClick()
        {
			queryForgetSkillHUD.gameObject.SetActive(true);

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);

        }

		public void OnConfirmForgetButtonClick(){

			queryForgetSkillHUD.gameObject.SetActive(false);
            
			PropertyChange propertyChange = Player.mainPlayer.ForgetSkill(currentSelectedSkill);
                     
			SetUpSkillView();

			skillStatusChangeCallBack(propertyChange);

			skillDetail.ClearSkillDetail();

		}

		public void OnCancelForgetButtonClick(){

			queryForgetSkillHUD.gameObject.SetActive(false);


		}

		public void QuitSkillsView(){

			gameObject.SetActive(false);

		}
        

    }

}
