using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEditor;

	public class PuzzleHelper 
    {
		[MenuItem("EditHelper/InitPuzzleDatas")]
		public static void InitPuzzleDatas(){

			string sourceDataFilePath = "/Users/houlianghong/Desktop/MyGameData/PuzzlesData.csv";

			string sourcePuzzleData = DataHandler.LoadDataString(sourceDataFilePath);

			List<Puzzle> puzzles = new List<Puzzle>();

			string[] puzzleDataArray = sourcePuzzleData.Split(new char[] { '\n' });

			for (int i = 1; i < puzzleDataArray.Length;i++){
				Puzzle puzzle = GeneratePuzzleFromDataString(puzzleDataArray[i]);
				Debug.Log(puzzle.question);
				puzzles.Add(puzzle);
			}

			string targetFilePath = CommonData.originDataPath + "/PuzzleDatas.json";

			DataHandler.SaveInstanceListToFile<Puzzle>(puzzles,targetFilePath);

			Debug.Log("puzzles finish！");
         
		}

		private static Puzzle GeneratePuzzleFromDataString(string PuzzleData){

			string[] dataArray = PuzzleData.Split(new char[] { ',' });

			int puzzleId = int.Parse(dataArray[0]);

			string question = dataArray[1].Replace('+', ',');

			string answer = dataArray[2].Replace('+', ',');

			string confusion_1 = dataArray[3].Replace('+', ',');

			string confusion_2 = dataArray[4].Replace('+', ',');

			Puzzle puzzle = new Puzzle(puzzleId, question, answer, confusion_1, confusion_2);

			return puzzle;

		}
    }
}

