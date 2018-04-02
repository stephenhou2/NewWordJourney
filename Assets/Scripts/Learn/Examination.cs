using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class Examination{

		// 测试类型
		public enum ExaminationType
		{
			EngToChn,//给拼写，选释义
			ChnToEng,//给释义，选拼写
			Both
		}
		// 测试题目（测试的单词）
		public LearnWord question;
		// 题目备选答案（测试单词+2个混淆单词）
		public LearnWord[] answers;
		// 正确答案在答案中的序号
		public int correctAnswerIndex;
		// 当前测试的测试类型
		private ExaminationType currentExamType;
		// 测试类型列表（中-英&英-中）
		private List<ExaminationType> examTypeList; 
//		= new List<ExaminationType>(){ExaminationType.EngToChn,ExaminationType.ChnToEng};

		// 测试备选单词数组
		private LearnWord[] wordsArray;

		public ExaminationType GetCurrentExamType(){

			int examTypeIndex = Random.Range (0, examTypeList.Count);

			currentExamType = examTypeList [examTypeIndex];

			return currentExamType;

		}

		/// <summary>
		/// 查询当前单词的测试是否完全完成（中-英和英-中两种测试已经全部答对）
		/// </summary>
		/// <returns><c>true</c>, if current exam finished was checked, <c>false</c> otherwise.</returns>
		public bool CheckCurrentExamFinished(){
			return examTypeList.Count <= 0;
		}

		/// <summary>
		/// 从单词测试类型列表中移除测试单词的当前测试类型
		/// </summary>
		public void RemoveCurrentExamType(){
			examTypeList.Remove (currentExamType);
			//如果还有测试未完成，则重新生成备选答案
			if (examTypeList.Count > 0) {
				RandomAnswersFromLearningWords (wordsArray);
			}
		}

		/// <summary>
		/// 从当前学习中的所有单词列表中生成备选答案（当前学习的单词+2个混淆单词）
		/// </summary>
		private void RandomAnswersFromLearningWords(LearnWord[] wordsArray){

			answers = new LearnWord[3];

			List<int> indexList = new List<int>{ 0, 1, 2 };

			int questionWordIndex = Random.Range (0, indexList.Count);

			answers [questionWordIndex] = question;

			indexList.Remove (questionWordIndex);

			LearnWord confuseWord1 = GetConfuseWordFromArray (wordsArray, new LearnWord[]{ question });

			int confuseWord1Index = indexList [Random.Range (0, indexList.Count)];

			answers [confuseWord1Index] = confuseWord1;

			indexList.Remove (confuseWord1Index);

			int confuseWord2Index = indexList [Random.Range (0, indexList.Count)];

			LearnWord confuseWord2 = GetConfuseWordFromArray (wordsArray, new LearnWord[]{ question, confuseWord1 });

			answers [confuseWord2Index] = confuseWord2;

			this.correctAnswerIndex = questionWordIndex;

		}


		/// <summary>
		/// 初始化测试数据
		/// </summary>
		/// <param name="question">Question.</param>
		/// <param name="answers">Answers.</param>
		/// <param name="correctAnswerIndex">Correct answer index.</param>
		/// <param name="examType">Exam type.</param>
		public Examination(LearnWord questionWord, LearnWord[] choiceWordsArray,ExaminationType examType){

			this.question = questionWord;
			this.wordsArray = choiceWordsArray;

			switch (examType) {
			case ExaminationType.EngToChn:
				examTypeList = new List<ExaminationType>{ ExaminationType.EngToChn };
				break;
			case ExaminationType.ChnToEng:
				examTypeList = new List<ExaminationType>{ ExaminationType.ChnToEng };
				break;
			case ExaminationType.Both:
				examTypeList = new List<ExaminationType>{ ExaminationType.EngToChn, ExaminationType.ChnToEng };
				break;
			}

			RandomAnswersFromLearningWords (choiceWordsArray);

		}



		private LearnWord GetConfuseWordFromArray(LearnWord[] wordsToLearnArray,LearnWord[] existWords){

			LearnWord learnWord = null;

			int randomWordId = Random.Range (0, wordsToLearnArray.Length);

			learnWord = wordsToLearnArray [randomWordId];

			for (int i = 0; i < existWords.Length; i++) {
				if (learnWord.wordId == existWords [i].wordId) {
					return GetConfuseWordFromArray (wordsToLearnArray, existWords);
				}
			}

			return learnWord;

		}


		public void Clear(){
//			question = null;
//			answers = null;
//			wordsArray = null;
		}

	}
}
