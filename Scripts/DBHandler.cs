using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBHandler : MonoBehaviour
{
    [System.Serializable]
    public class DBRoot {
        public List<string> ContactList;
        public List<string> ChapterList;
        public List<string> PhotoList;
        public List<string> MidrollsList;
    }

    public DBRoot DataBase = new DBRoot(); 
    public void LoadDB() {
        TextAsset DBFile = Resources.Load<TextAsset>("DB");
        DataBase = JsonUtility.FromJson<DBRoot>(DBFile.text);
    }
}
