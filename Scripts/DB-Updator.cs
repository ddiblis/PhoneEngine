using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DBUpdator : MonoBehaviour {
    public DBRoot LoadDB() {
        try {
            TextAsset DBFile = Resources.Load<TextAsset>("DB");
            return JsonUtility.FromJson<DBRoot>(DBFile.text);
        } catch {
            return new DBRoot();
        }
    }
        
    public void UpdateDB() {  
        DBRoot ReadDB = LoadDB();
        ReadDB.PhotoList = new();
        string[] PhotoList = Directory.GetFiles("Assets/Resources/Images/Photos","*.png");
        foreach (string Photo in PhotoList) {
            ReadDB.PhotoList.Add(Photo[31..^4]);
        }

        string DBFileName = $"{Application.dataPath}/Resources/DB.json";
        string jsonString = JsonUtility.ToJson(ReadDB, true);
        
        File.WriteAllText(DBFileName, jsonString);
    }
}
