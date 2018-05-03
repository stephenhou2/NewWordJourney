using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    public enum HLHSAPType{
        Sentence,
        Poem
    }

    [System.Serializable]
    public struct HLHSentenceAndPoem
    {
        // 句子或者诗歌id
        public int sapID;
        // 句子或者诗歌英文
        public string sapEN;
        // 句子或者诗歌中文
        public string sapCH;
        // 句子或者诗歌类型
        public HLHSAPType sapType;



        // 构造函数
        public HLHSentenceAndPoem(int proverbID, string proverbEN,string proverbCH,HLHSAPType type){
            this.sapID = proverbID;
            this.sapEN = proverbEN;
            this.sapCH = proverbCH;
            this.sapType = type;
        }

    }

}

