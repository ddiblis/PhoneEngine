using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapImport : MonoBehaviour {

    public SaveFile SF;
    public DBHandler DB;

    public Chapter myChapter = new();
    public Chapter GetChapter(ChapterType type, string Chapter, Tendency tendency = Tendency.Neutral) {
        string TypeString;
        TextAsset ChapterFile;
        if (type == ChapterType.Chapter) {
            TypeString = "Chapters/";
            ChapterFile = Resources.Load<TextAsset>(TypeString + Chapter);
        } else {
            TypeString = "Midrolls/";
            ChapterFile = Resources.Load<TextAsset>(TypeString + Chapter);
        }
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }
}
