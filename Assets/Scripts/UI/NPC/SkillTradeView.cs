using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;
	using UnityEngine.UI;

	public class SkillTradeView : MonoBehaviour
    {

        public InstancePool skillGoodsPool;

        public Transform skillGoodsContainer;

		public SkillGoodsInTrade skillGoodsModel;
       

		public Transform skillGoodsHUD;
		public Transform learndedSkillHUD;
		public Text skillNumLeftText;
		public SkillDetailInTrade skillDetailInTrade;

		public Transform forgetSkillQueryHUD;

        private List<SkillGoodsInTrade> allSkillGoods = new List<SkillGoodsInTrade>();
		public SimpleSkillDetail[] allLearnedSkillDetails;

		private HLHNPC currentNpc;

        private Skill currentSelectedSkill;
		private List<int> skillGoodsIds;
        private CallBack quitCallBack;

		public SkillLearnQuizView learnQuizView;

        public TintHUD tintHUD;

		public float flyDuration;

		public Vector3 skillGoodsPlaneStartPos;

        public Vector3 skillDetailPlaneStartPos;

        public Vector3 learnedSkillsPlaneStartPos;

        public int skillGoodsPlaneMoveEndX;

        public int skillDetailMoveEndX;

        public int learnedSkillsPlaneMoveEndY;

        
      
		public void SetUpSkillLearningView(HLHNPC npc,CallBack quitCallBack){

			this.currentNpc = npc;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.merchantAudioName);

			gameObject.SetActive(true);

			this.skillGoodsIds = npc.npcSkillIds;

			this.quitCallBack = quitCallBack;

			skillGoodsPool.AddChildInstancesToPool(skillGoodsContainer);
         
			for (int i = 0; i < skillGoodsIds.Count; i++)
            {

				SkillGoodsInTrade skillGoodsDetail = skillGoodsPool.GetInstance<SkillGoodsInTrade>(skillGoodsModel.gameObject, skillGoodsContainer);

                Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find(delegate (Skill obj)
                {
					return obj.skillId == skillGoodsIds[i];               
                });

				skillGoodsDetail.SetupSkillDetailInNPC(skill,SkillGoodsSelectCallBack);

				skillGoodsDetail.SetUpSelectedIcon(false);

				allSkillGoods.Add(skillGoodsDetail);

            }

			skillNumLeftText.text = string.Format("剩余技能点: {0}", Player.mainPlayer.skillNumLeft);

			for (int i = 0; i < allLearnedSkillDetails.Length;i++){

				SimpleSkillDetail learnedSkillDetail = allLearnedSkillDetails[i];

				if(i<Player.mainPlayer.allLearnedSkills.Count){
                    
					Skill learnedSkill = Player.mainPlayer.allLearnedSkills[i];

					learnedSkillDetail.SetUpSimpleSkillDetail(learnedSkill, LearnedSkillSelectCallBack);
				}else{
					learnedSkillDetail.ClearSimpleSkillDetail();
				}

				learnedSkillDetail.SetUpSelectedIcon(false);
                
			}

			//skillDetailInTrade.InitskilldetailInTrade(UpdateLearnedSkillsPlane);
			skillDetailInTrade.ClearSkillDetailInTrade();

			EnterSkillGoodsTradeViewDisplay();

        }
        

        /// <summary>
        /// 技能商品被选中的回调
        /// </summary>
        /// <param name="skill">Skill.</param>
		private void SkillGoodsSelectCallBack(Skill skill){
        
			for (int i = 0; i < skillGoodsIds.Count;i++){

				SkillGoodsInTrade skillDetail = allSkillGoods[i];
               
				skillDetail.SetUpSelectedIcon(skillGoodsIds[i] == skill.skillId);
               
			}
                     
			currentSelectedSkill = skill;

			for (int i = 0; i < allLearnedSkillDetails.Length;i++){
				allLearnedSkillDetails[i].SetUpSelectedIcon(false);
			}

			skillDetailInTrade.SetUpSkillDetailInTrade(skill);
		}


        /// <summary>
		/// 已学习技能的被选中的回调
        /// </summary>
        /// <param name="skill">Skill.</param>
		private void LearnedSkillSelectCallBack(Skill skill){

			for (int i = 0; i < Player.mainPlayer.allLearnedSkills.Count; i++)
            {

				SimpleSkillDetail skillDetail = allLearnedSkillDetails[i];

				skillDetail.SetUpSelectedIcon(false);

				Skill learnedSkill = Player.mainPlayer.allLearnedSkills[i];

				skillDetail.SetUpSelectedIcon(learnedSkill.skillId == skill.skillId);

            }

			for (int i = 0; i < allSkillGoods.Count;i++){
				allSkillGoods[i].SetUpSelectedIcon(false);
			}

            currentSelectedSkill = skill;

            skillDetailInTrade.SetUpSkillDetailInTrade(skill);

		}

		/// <summary>
		/// 更新人物已学习技能面板
		/// </summary>
		public void UpdateLearnedSkillsPlane(Skill skill)
		{

			if (skill == null)
			{
				return;
			}


			for (int i = 0; i < allLearnedSkillDetails.Length; i++)
			{

				SimpleSkillDetail learnedSkillDetail = allLearnedSkillDetails[i];


				if (i < Player.mainPlayer.allLearnedSkills.Count)
				{

					Skill learnedSkill = Player.mainPlayer.allLearnedSkills[i];

					learnedSkillDetail.SetUpSimpleSkillDetail(learnedSkill, LearnedSkillSelectCallBack);

					//if (learnedSkill.skillId == skill.skillId)
					//{
					learnedSkillDetail.SetUpSelectedIcon(false);
					//}
				}
				else
				{
					learnedSkillDetail.ClearSimpleSkillDetail();
				}

				skillDetailInTrade.SetUpSkillDetailInTrade(skill);

			}
		}
        

		public void OnLearnButtonClick()
        {

            if (currentSelectedSkill == null)
            {
                return;
            }

            //if (Player.mainPlayer.totalGold < currentSelectedSkill.price)
            //{
            //    tintHUD.SetUpSingleTextTintHUD("金币不足");
            //}
            else if (Player.mainPlayer.CheckSkillFull())
            {
                tintHUD.SetUpSingleTextTintHUD("只能学习6个技能");
            }
            else if (Player.mainPlayer.CheckSkillHasLearned(currentSelectedSkill))
            {
                tintHUD.SetUpSingleTextTintHUD("不能重复学习技能");
            }
            else
            {
				//            Player.mainPlayer.totalGold -= currentSelectedSkill.price;

				//Player.mainPlayer.LearnSkill(currentSelectedSkill.skillId);

				//UpdateLearnedSkillsPlane(currentSelectedSkill);

				//ExploreManager.Instance.expUICtr.UpdatePlayerGold();
                
				QuitSkilllGoodsTradeViewDisplay();

				learnQuizView.SetUpSkillLearnQuizView(currentNpc,currentSelectedSkill,delegate {
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.merchantAudioName);
					UpdateLearnedSkillsPlane(currentSelectedSkill);
					if(quitCallBack != null){
						quitCallBack();
					}               
				});

            }


        }

        public void OnForgetButtonClick()
        {

            forgetSkillQueryHUD.gameObject.SetActive(true);

        }

        public void OnConfirmForgetSkill()
        {

            forgetSkillQueryHUD.gameObject.SetActive(false);

            Skill learnedSkill = Player.mainPlayer.allLearnedSkills.Find(delegate (Skill obj)
            {
                return obj.skillId == currentSelectedSkill.skillId;
            });

            Player.mainPlayer.ForgetSkill(learnedSkill);

			UpdateLearnedSkillsPlane(currentSelectedSkill);

        }

        public void OnCancelForgetSkill()
        {

            forgetSkillQueryHUD.gameObject.SetActive(false);
        }


		public void OnBackButtonClick(){

			if(quitCallBack != null){
				quitCallBack();
			}

			QuitSkilllGoodsTradeViewDisplay();

		}

        /// <summary>
        /// 进入技能学习交易面板的动画
        /// </summary>
		public void EnterSkillGoodsTradeViewDisplay(){
            
			skillDetailInTrade.transform.localPosition = skillDetailPlaneStartPos;
			skillDetailInTrade.transform.DOLocalMoveX(skillDetailMoveEndX, flyDuration);
			skillGoodsHUD.transform.localPosition = skillGoodsPlaneStartPos;
			skillGoodsHUD.transform.DOLocalMoveX(skillGoodsPlaneMoveEndX, flyDuration);
			learndedSkillHUD.transform.localPosition = learnedSkillsPlaneStartPos;
			learndedSkillHUD.transform.DOLocalMoveY(learnedSkillsPlaneMoveEndY, flyDuration);

		}

		/// <summary>
        /// 退出技能学习交易面板的动画
        /// </summary>
		public void QuitSkilllGoodsTradeViewDisplay(){

			skillDetailInTrade.transform.DOLocalMoveX(skillDetailPlaneStartPos.x, flyDuration);
			skillGoodsHUD.transform.DOLocalMoveX(skillGoodsPlaneStartPos.x, flyDuration);
			learndedSkillHUD.transform.DOLocalMoveY(learnedSkillsPlaneStartPos.y, flyDuration);


		}

    }   
}
