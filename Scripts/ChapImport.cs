using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapImport : MonoBehaviour {

    public SaveFile SF;
    public DBHandler DB;




    public Chapter myChapter = new();
    [SerializeField] 
    public Chapter GetChapter(string type, string Chapter) {
        TextAsset ChapterFile = Resources.Load<TextAsset>(type + "/" + Chapter);
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }
}
