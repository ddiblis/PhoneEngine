using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Unity.VisualScripting;
using UnityEditor.VersionControl;

public class SaveManager : MonoBehaviour
{
    public SharedObjects Shared;

    // public class testStruct{
    //     List<string> seenImages;
    //     List<bool> UnlockedContacts;
    // }

    // public void test(){
    //     string saveFile = Application.persistentDataPath + "/gamedata.data";

    //     // Shared.seenImages;
    //     // Shared.UnlockedContacts;
    // }
    string saveFile;

    void Awake()
    {
        // Update the path once the persistent path exists.
        saveFile = Application.persistentDataPath + "/gamedata.json";
    }

    public void readFile()
    {
        // Does the file exist?
        if (File.Exists(saveFile))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(saveFile);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            JsonUtility.FromJsonOverwrite(fileContents, Shared);

            for (int i = 0; i < Shared.ContactsList.Count; i++) {
                Transform MessageList = Shared.content.GetChild(i);
                for (int j = 0; j < Shared.NumOfMessagesInList[i]; j++) {
                    Shared.MessageListContent[j].SetParent(MessageList);
                }
            }
        }
    }

    void GetAllMessages(Transform MessageList) {
            Shared.NumOfMessagesInList.Add(MessageList.childCount);
            for(int j = 0; j < MessageList.childCount; j++) {
                Shared.MessageListContent.Add(MessageList.GetChild(j));
            }
    }

    void GetMessagesSnapshot() {
        for (int i = 0; i < Shared.ContactsList.Count; i++) {
            GetAllMessages(Shared.content.GetChild(i));
        }
    }

    public void writeFile() {   

        GetMessagesSnapshot();

        // Serialize the object into JSON and save string.
        string jsonString = JsonUtility.ToJson(Shared);

        // Write JSON to file.
        File.WriteAllText(saveFile, jsonString);


        Shared.MessageListContent = new List<Transform>();
        Shared.NumOfMessagesInList = new List<int>();
    }
}


