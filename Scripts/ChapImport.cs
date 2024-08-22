using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class ChapImport : MonoBehaviour {

    public SaveFile SF;
    public DBHandler DB;


    [System.Serializable]
    public class Responses
    {
        public bool RespTree;
        public List<string> Resps;
        public List<int> NextChap;

    }

    [System.Serializable]
    public class Chapter
    {
        public bool AllowMidrolls;
        public List<SubChap> SubChaps;
    }

    [System.Serializable]
    public class SubChap
    {
        public string Contact;
        public string TimeIndicator;
        public List<string> TextList;
        public List<float> ResponseTime;
        public string UnlockInstaPostsAccount;
        public List<int> UnlockPosts;
        public Responses Responses;
    }


    public Chapter myChapter = new Chapter();
    [SerializeField] 
    public Chapter GetChapter(string type, string Chapter) {
        TextAsset ChapterFile = Resources.Load<TextAsset>(type + "/" + Chapter);
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }
}
