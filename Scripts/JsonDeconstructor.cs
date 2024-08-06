using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonDeconstructor : MonoBehaviour
{
    [System.Serializable]
    public class Responses
    {
        public List<string> Resps;
        public int SubchapForTop;
        public int SubchapForBottom;
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
        public List<string> TextList;
        public List<string> ImageList;
        public List<object> SubList;
        public List<object> DomList;
        public List<double> ResponseTime;
        public Responses Responses;
    }

    public TextAsset chapterJSON;



    public Chapter myChapter = new Chapter();

    void Start() {
        myChapter = JsonUtility.FromJson<Chapter>(chapterJSON.text);
    }
}
