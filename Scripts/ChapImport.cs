using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapImport : MonoBehaviour {
    
    [System.Serializable]
    public class Responses
    {
        public List<string> Resps;
        public List<int> SubChaps;

    }

    [System.Serializable]
    public class Chapter
    {
        public bool ChapComplete;
        public List<SubChap> SubChaps;
    }

    [System.Serializable]
    public class SubChap
    {
        public int SubChapNum;
        public string Contact;
        public List<string> TextList;
        public List<float> ResponseTime;
        public Responses Responses;
    }

    public TextAsset chapterJSON;



    public Chapter myChapter = new Chapter();
    [SerializeField] 
    public Chapter getChapter() {
        myChapter = JsonUtility.FromJson<Chapter>(chapterJSON.text);
        return myChapter;
    }
    
}
