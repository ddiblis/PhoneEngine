using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DBUpdator : MonoBehaviour {
    
    public DBRoot DataBase = new(); 
    
    public void UpdateDB() {  
        string[] PhotoList = Directory.GetFiles("Assets/Resources/Images/Photos","*.png");
        foreach (string Photo in PhotoList) {
            DataBase.PhotoList.Add(Photo[31..^4]);
        }

        string[] ContactList = Directory.GetFiles("Assets/Resources/Images/Headshots","*.png");
        foreach (string Contact in ContactList) {
            DataBase.ContactList.Add(Contact[34..^4]);
        }

        string[] ChapterList = Directory.GetFiles("Assets/Resources/Chapters", "*.json");
        foreach(string Chapter in ChapterList) {
            DataBase.ChapterList.Add(Chapter[26..^5]);
        }

        // Midroll name should be something like "At-the-park-4" where the number at the end is the checkpoint needed
        string[] MidrollList = Directory.GetFiles("Assets/Resources/Midrolls", "*.json");
        foreach(string Midroll in MidrollList) {
            int CheckpointNumber = Midroll.LastIndexOf("noteline")+1;
            DataBase.MidrollsList.Add( new DBMidRoll {
                MidrollName = Midroll[25..^7],
                Checkpoint = int.Parse(Midroll[CheckpointNumber..^5])
            });
        }


        string DBFileName = Application.dataPath + "/Resources/DB.json";
        string jsonString = JsonUtility.ToJson(DataBase, true);
        
        File.WriteAllText(DBFileName, jsonString);
    }
}
