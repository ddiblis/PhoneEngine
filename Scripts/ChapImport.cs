using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChapImport : MonoBehaviour {

    public SaveFile SF;
    public DBHandler DB;

    public Chapter myChapter = new();
    public Chapter GetChapter(ChapterType type, string Chapter) {
        string TypeString;
        TextAsset ChapterFile;
        if (type == ChapterType.Chapter) {
            TypeString = "Chapters/";
            var match = Regex.Match(Chapter, @"\d+");
            // Add one of tendencies inside the findindex below
            int ChapterSplitOff = DB.DataBase.ChapterList.FindIndex(x => x.Contains($"{int.Parse(match.Value)}(Tendency)"));
            if (ChapterSplitOff > -1) {
                Chapter += SF.saveFile.Stats.Tendency switch
                {
                    // Add the Tendencies you've implemented with which files they should load
                    // (int)Tendency.Tendency1 => "(Tendency1)",
                    // _ is the default file to load
                    _ => "(Tendency2)",
                };
                int indexOfLastChapter = SF.saveFile.ChapterList.FindIndex(x => x.ChapterName == Chapter) - 1;
                if (indexOfLastChapter > -1) SF.saveFile.ChapterList[indexOfLastChapter].seen = true;
                ChapterFile = Resources.Load<TextAsset>(TypeString + Chapter);
                if(ChapterFile == null) {
                    return null;
                }
            } else {
                int indexOfLastChapter = SF.saveFile.ChapterList.FindIndex(x => x.ChapterName == Chapter) - 1;
                if (indexOfLastChapter > -1) SF.saveFile.ChapterList[indexOfLastChapter].seen = true;
                ChapterFile = Resources.Load<TextAsset>(TypeString + Chapter);
                if(ChapterFile == null) {
                    return null;
                }
            }
        } else {
            TypeString = "Midrolls/";
            ChapterFile = Resources.Load<TextAsset>(TypeString + Chapter);
        }
        myChapter = JsonUtility.FromJson<Chapter>(ChapterFile.text);
        return myChapter;
    }
}
