using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


	public class PuzzleView : MonoBehaviour
    {


		public Image lockIcon;

		public Sprite lockSprite;
		public Sprite unlockSprite;

		public Text puzzleText;

		public Text[] choicesText;
      
		private int answerIndex;
        
		private CallBack answerRightCallBack;

		private CallBack answerWrongCallBack;

        
		public void SetUpMonsterPuzzleView(CallBack answerRightCallBack,CallBack answerWrongCallBack){

			this.answerRightCallBack = answerRightCallBack;

			this.answerWrongCallBack = answerWrongCallBack;

			this.gameObject.SetActive(true);

			lockIcon.sprite = lockSprite;

			Puzzle puzzle = GameManager.Instance.gameDataCenter.GetARandomPuzzle();

			puzzleText.text = puzzle.question;

			for (int i = 0; i < choicesText.Length; i++)
			{
				choicesText[i].color = Color.white;
			}

			List<int> indexGrid = new List<int> { 0, 1, 2 };

			int randomSeed = Random.Range(0, 3);

			answerIndex = indexGrid[randomSeed];

			indexGrid.RemoveAt(randomSeed);

			randomSeed = Random.Range(0, 2);

			int confusion_1_index = indexGrid[randomSeed];

			indexGrid.RemoveAt(randomSeed);

			randomSeed = 0;

			int confusion_2_index = indexGrid[randomSeed];

			choicesText[answerIndex].text = puzzle.answer;

			choicesText[confusion_1_index].text = puzzle.confusion_1;

			choicesText[confusion_2_index].text = puzzle.confusion_2;
   

		}

		public void OnChoiceMake(int index){

			bool answerRight = index == answerIndex;

			if(answerRight){
				choicesText[index].color = Color.green;
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.lockOffAudioName);
				lockIcon.sprite = unlockSprite;
			}else{
				choicesText[index].color = Color.red;
			}
                     
			IEnumerator answerTintCoroutine = ShowRightOrWrongForAWhileAndQuit(answerRight);

			StartCoroutine(answerTintCoroutine);

		}

		private IEnumerator ShowRightOrWrongForAWhileAndQuit(bool answerRight){
			
			yield return new WaitForSeconds(1f);

			QuitPuzzleView();

			if(answerRight && answerRightCallBack != null){
				answerRightCallBack();
			}else if(!answerRight && answerWrongCallBack != null){
				answerWrongCallBack();
			}


		}


		public void QuitPuzzleView(){
			this.gameObject.SetActive(false);
		}



        
    }
}

