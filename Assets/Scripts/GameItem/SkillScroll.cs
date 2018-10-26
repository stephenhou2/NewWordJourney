using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    /// <summary>
    /// 技能卷轴类
    /// </summary>
	[System.Serializable]
	public class SkillScroll : Item {

		public int skillId;

        /// <summary>
        /// 技能卷轴构造函数
        /// </summary>
        /// <param name="skillScrollModel">Skill scroll model.</param>
        /// <param name="itemCount">Item count.</param>
		public SkillScroll(SkillScrollModel skillScrollModel,int itemCount){

			InitBaseProperties (skillScrollModel);

			this.itemType = ItemType.SkillScroll;

			this.itemCount = itemCount;

			this.skillId = skillScrollModel.skillId;

		}
        
        /// <summary>
        /// 使用技能卷轴
        /// </summary>
        /// <returns>The skill scroll.</returns>
		public PropertyChange UseSkillScroll(){
            
            // 玩家使用技能卷轴学习技能
			PropertyChange propertyChange = Player.mainPlayer.LearnSkill(skillId);

            // 如果在战斗中使用了技能卷轴学习技能，则更新可用技能按钮
			if (ExploreManager.Instance.battlePlayerCtr.isInFight)
            {
                ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
            }

			return propertyChange;


		}
      
	}
}
