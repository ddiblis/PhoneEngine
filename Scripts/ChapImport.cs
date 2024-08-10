using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class ChapImport : MonoBehaviour {

    public SharedObjects Shared;


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
        public int SubChapNum;
        public string Contact;
        public string TimeIndicator;
        public List<string> TextList;
        public List<float> ResponseTime;
        public Responses Responses;
    }

    void Awake(){
        string[] FileList = Directory.GetFiles("Assets/Resources/Chapters/","*.json");
        foreach (string File in FileList) {
            Shared.ChapterList.Add(File[26..^5]);
        }
    }


    public Chapter myChapter = new Chapter();
    [SerializeField] 
    public Chapter GetChapter(string Chapter) {
        TextAsset ChapterFile = Resources.Load<TextAsset>("Chapters/" + Chapter);
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }
    
}
