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
        if (DataBase.MidrollsList.Count != SF.saveFile.MidRolls.Count) {
            for (int i = SF.saveFile.MidRolls.Count; i < DataBase.MidrollsList.Count; i++) {
                SF.saveFile.MidRolls.Add(
                    new MidRoll{ 
                        MidrollName = DataBase.MidrollsList[i].MidrollName,
                        Checkpoint = DataBase.MidrollsList[i]. Checkpoint,
                        Seen = false
                    }
                );
            }
        }
    }

    public void GenerateContactsList() {
        if (DataBase.ContactList.Count != SF.saveFile.ContactsList.Count) {
            for (int i = SF.saveFile.ContactsList.Count; i < DataBase.ContactList.Count; i++) {
                SF.saveFile.ContactsList.Add(
                    new Contact{ 
                        NameOfContact = DataBase.ContactList[i],
                        Unlocked = false 
                    }
                );
            }
        }
    }

    public void GeneratePhotoList() {
        if (DataBase.PhotoList.Count != SF.saveFile.Photos.Count) {
            for (int i = SF.saveFile.Photos.Count; i < DataBase.PhotoList.Count; i++) {
                SF.saveFile.Photos.Add(
                    new Photo{ 
                        Category = DataBase.PhotoList[i].Split("-")[0],
                        ImageName = DataBase.PhotoList[i],
                        Seen = false 
                    }
                );
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
        if (DataBase.ChapterList.Count != SF.saveFile.ChapterList.Count) {
            for (int i = SF.saveFile.ChapterList.Count; i < DataBase.ChapterList.Count; i++) {
                SF.saveFile.ChapterList.Add(
                    DataBase.ChapterList[i]
                );
            }
        }
    }
}
