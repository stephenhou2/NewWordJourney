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

		public Text learnedSkillCountText;

		public Transform queryForgetSkillHUD;
        
        public SimpleSkillDetail[] simpleSkillDetails;

		public SkillDetail skillDetail;
            
		private Skill currentSelectedSkill;

		public TintHUD tintHUD;

		private CallBackWithPropertyChange skillStatusChangeCallBack;

		public void InitSkillsView(CallBackWithPropertyChange skillStatusChangeCallBack){

			this.skillStatusChangeCallBack = skillStatusChangeCallBack;
		}

		public void SetUpSkillView(){   
			
			gameObject.SetActive(true);

			leftSkillNumText.text = string.Format("剩余技能点: {0}",Player.mainPlayer.skillNumLeft);

			learnedSkillCountText.text = string.Format("已学习技能数量: {0}", Player.mainPlayer.allLearnedSkills.Count);
                     
			for (int i = 0; i < simpleSkillDetails.Length; i++){

				SimpleSkillDetail simpleSkillDetail = simpleSkillDetails[i];

				bool selectedIconEnable = false;

				if(i<Player.mainPlayer.allLearnedSkills.Count){
					Skill learnedSkill = Player.mainPlayer.allLearnedSkills[i];
					simpleSkillDetail.SetUpSimpleSkillDetail(learnedSkill, OnSkillClick);
					selectedIconEnable = currentSelectedSkill != null && currentSelectedSkill.skillId == learnedSkill.skillId;
				}else{
					simpleSkillDetail.ClearSimpleSkillDetail();
				}


				simpleSkillDetail.SetUpSelectedIcon(selectedIconEnable);
			}

			if(currentSelectedSkill == null){
				skillDetail.ClearSkillDetail();    
			}else{
				skillDetail.SetupSkillDetail(currentSelectedSkill);

			}         
		}

        /// <summary>
		/// 技能被点击回调（点击时将所有的技能选中框都禁用，SimpleSkillDetail里在回调之后会将自身选中框启用）
        /// </summary>
        /// <param name="skill">Skill.</param>
		private void OnSkillClick(Skill skill){
			currentSelectedSkill = skill;
			skillDetail.SetupSkillDetail(skill);
			for (int i = 0; i < simpleSkillDetails.Length;i++){
				SimpleSkillDetail simpleSkillDetail = simpleSkillDetails[i];
				simpleSkillDetail.SetUpSelectedIcon(false);
			}
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
			currentSelectedSkill = null;
			gameObject.SetActive(false);         
		}
        

    }

}
