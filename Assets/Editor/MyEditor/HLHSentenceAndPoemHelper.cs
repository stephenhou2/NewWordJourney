using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    using UnityEditor;

    public class HLHSentenceAndPoemHelper 
    {

        public static string sapOriFilePath = "/Users/houlianghong/Desktop/MyGameData/短句和诗歌原始数据.csv";
        public static string sapTargetFilePath = CommonData.originDataPath + "/HLHSentenceAndPoemData.json";


        [MenuItem("EditHelper/SAPDataHelper")]
        public static void LoadAllProverbData(){

            List<HLHSentenceAndPoem> saps = new List<HLHSentenceAndPoem>();

            string oriProverbData = DataHandler.LoadDataString(sapOriFilePath);

            string[] sapbDatasArray = oriProverbData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < sapbDatasArray.Length; i++){

                string sapData = sapbDatasArray[i];

                HLHSentenceAndPoem sap = GenerateHLHProverb(sapData);

                saps.Add(sap);

            }

           //string formatData = JsonHelper.ToJson<HLHProverb>(proverbs.ToArray());

            DataHandler.SaveInstanceListToFile<HLHSentenceAndPoem>(saps, sapTargetFilePath);

        }

        private static HLHSentenceAndPoem GenerateHLHProverb(string sapData){

            Debug.Log(sapData);

            string[] dataArray = sapData.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            int proverbID = int.Parse(dataArray[0]);
            string proverbEN = dataArray[1].Replace('+',',').Replace('#','\n').Replace('^', '\"');
            string proverbCH = dataArray[2].Replace('+',',').Replace('#','\n').Replace('^', '\"');
            HLHSAPType type = (HLHSAPType)int.Parse(dataArray[3]);

            return new HLHSentenceAndPoem(proverbID, proverbEN, proverbCH,type);

        }
       
    }

}

