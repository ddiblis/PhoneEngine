using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class ChapImport : MonoBehaviour {

    public SharedObjects Shared;
    public SavedItems saved;


    [System.Serializable]
    public class Responses
    {
        public List<string> Resps;
        public List<int> NextChap;

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
    public Chapter GetChapter(string Chapter) {
        TextAsset ChapterFile = Resources.Load<TextAsset>("Chapters/" + Chapter);
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }

    public void GenerateChapterList() {
        string[] FileList = Directory.GetFiles(Application.streamingAssetsPath + "/Chapters/","*.NA");
        if (FileList.Length != saved.ChapterList.Count) {
            for (int i = saved.ChapterList.Count; i < FileList.Length; i++) {
                saved.ChapterList.Add(FileList[i][(FileList[i].LastIndexOf("/")+1)..^3]);
            }
        }
    }
    
}
