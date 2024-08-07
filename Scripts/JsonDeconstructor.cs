using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonDeconstructor : MonoBehaviour
{
    [System.Serializable]
    public class Responses
    {
        public List<string> TextResps;
        public List<string> ImageResps;
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
        public List<string> TextList;
        public List<string> ImageList;
        public List<object> SubList;
        public List<object> DomList;
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
