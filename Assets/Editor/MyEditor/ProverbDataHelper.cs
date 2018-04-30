using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    using UnityEditor;

    public class ProverbDataHelper 
    {

        public static string proverbOriFilePath = "/Users/houlianghong/Desktop/MyGameData/谚语短句原始数据/谚语.csv";
        public static string proverbTargetFilePath = CommonData.originDataPath + "/ProverbData.json";

        [MenuItem("EditHelper/ProverbDataHelper")]
        public static void LoadAllProverbData(){

            List<HLHProverb> proverbs = new List<HLHProverb>();

            string oriProverbData = DataHandler.LoadDataString(proverbOriFilePath);

            string[] proverbDatasArray = oriProverbData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < proverbDatasArray.Length; i++){

                string proverbData = proverbDatasArray[i];

                HLHProverb proverb = GenerateHLHProverb(proverbData);

                proverbs.Add(proverb);

            }

           //string formatData = JsonHelper.ToJson<HLHProverb>(proverbs.ToArray());

            DataHandler.SaveInstanceListToFile<HLHProverb>(proverbs, proverbTargetFilePath);

        }

        private static HLHProverb GenerateHLHProverb(string proverbData){

            string[] dataArray = proverbData.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            int proverbID = int.Parse(dataArray[0]);
            string proverbEN = dataArray[1].Replace('+',',');
            string proverbCH = dataArray[2].Replace('+',',');

            return new HLHProverb(proverbID, proverbEN, proverbCH);

        }
       
    }

}

