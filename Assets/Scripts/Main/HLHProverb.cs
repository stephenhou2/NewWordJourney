using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

    [System.Serializable]
    public struct HLHProverb
    {
        // 谚语id
        public int proverbID;
        // 谚语英文
        public string proverbEN;
        // 谚语中文
        public string proverbCH;

        // 构造函数
        public HLHProverb(int proverbID, string proverbEN,string proverbCH){
            this.proverbID = proverbID;
            this.proverbEN = proverbEN;
            this.proverbCH = proverbCH;
        }

    }

}

