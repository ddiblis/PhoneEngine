using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


    // [System.Serializable]
    // public class Responses
    // {
    //     public List<Response> RespContent;
    // }
    
    [System.Serializable]
    public class Response {
        public bool RespTree = false;
        public string TextContent;
        public int SubChapNum;
        public int Type;
    }

    [System.Serializable]
    public class Chapter
    {
        public bool AllowMidrolls;
        public List<SubChap> SubChaps;
    }

    [System.Serializable]
    public class TextMessage {
        public int Type;
        // public int Tendency;
        public string TextContent;
        public float TextDelay;
    }

    [System.Serializable]
    public class SubChap
    {
        public string Contact;
        public string TimeIndicator;
        public List<TextMessage> TextList;
        public string UnlockInstaPostsAccount;
        public List<int> UnlockPosts;
        public List<Response> Responses;
    }
public class ChapImport : MonoBehaviour {

    public SaveFile SF;
    public DBHandler DB;




    public Chapter myChapter = new Chapter();
    [SerializeField] 
    public Chapter GetChapter(string type, string Chapter) {
        TextAsset ChapterFile = Resources.Load<TextAsset>(type + "/" + Chapter);
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }
}
