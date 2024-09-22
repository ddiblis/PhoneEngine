using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

public class DBHandler : MonoBehaviour {
    public SaveFile SF;

    public DBRoot DataBase = new(); 
    public void LoadDB() {
        TextAsset DBFile = Resources.Load<TextAsset>("DB");
        DataBase = JsonUtility.FromJson<DBRoot>(DBFile.text);
    }
    
    public void GenerateMidrollsList() {
        foreach(var midroll in DataBase.MidrollsList) {
            int midrollIndex = SF.saveFile.MidRolls.FindIndex(x => x.MidrollName == midroll.MidrollName);
            if (midrollIndex == -1) {
                SF.saveFile.MidRolls.Add(
                    new MidRoll{ 
                        MidrollName = midroll.MidrollName,
                        Checkpoint = midroll. Checkpoint,
                        Seen = false
                    }
                );
            }
        }
    }

    public void GeneratePhotoList() {
        if (DataBase.PhotoList.Count != SF.saveFile.Photos.Count) {
            for (int i = SF.saveFile.Photos.Count; i < DataBase.PhotoList.Count; i++) {
                if (!SF.saveFile.PhotoCategories.Any(x => x.Category == DataBase.PhotoList[i].Split("-")[0])) {
                    SF.saveFile.PhotoCategories.Add(
                        new PhotoCategory{
                            Category = DataBase.PhotoList[i].Split("-")[0],
                            NumberSeen = 0,
                            NumberAvaliable = 1
                        }
                    );
                } else {
                    int Category = SF.saveFile.PhotoCategories.FindIndex(x => x.Category == DataBase.PhotoList[i].Split("-")[0]);
                    SF.saveFile.PhotoCategories[Category].NumberAvaliable += 1;
                }
            }
        }
    }

    public void GenerateChapterList() {
        foreach(var chapter in DataBase.ChapterList) {
            int ChapterIndex = SF.saveFile.ChapterList.FindIndex(x => x.ChapterName == chapter);
            if (ChapterIndex == -1) {
                SF.saveFile.ChapterList.Add(
                    new AvaliableChapters {
                        ChapterName = chapter,
                        seen = false
                    }
                );
            }
        }
    }
    public void UnlockInstaPostsForChapter(int index) {
        for (int i = 0; i < index; i++) {
            for (int j = 0; j < DataBase.ChapterInstaPosts[i].InstaPostsList.Count; j++) {
                SF.saveFile.Posts[DataBase.ChapterInstaPosts[i].InstaPostsList[j]].Unlocked = true;
            }
        }
    }
}
