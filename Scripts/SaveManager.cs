using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using TMPro;
using System.Runtime.InteropServices;

public class SaveManager : MonoBehaviour
{
    public SharedObjects Shared;
    public MessagingHandlers MH;

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

    void handleImageMessages(TypeOfText MessageType, string TextContentOfMessage) {
        if (TextContentOfMessage.Contains("{")){
            Sprite img = Resources.Load("Images/Photos/" + TextContentOfMessage[1..^1], typeof(Sprite)) as Sprite;
            MH.MessageListLimit(MessageType, imgName: TextContentOfMessage, img);
        } 
        else {
            Sprite img = Resources.Load("Images/Emojis/" + TextContentOfMessage[1..^1], typeof(Sprite)) as Sprite;
            MH.MessageListLimit(MessageType, imgName: TextContentOfMessage, img);
        }
    }

    void LoadAllMessages(int MessageIndex) {
        int type = Shared.typeOfText[MessageIndex];
        TypeOfText MessageType = (TypeOfText)type;
        string TextContentOfMessage = Shared.savedMessages[MessageIndex];
        if(type == 0 || type == 1 || type == 6 || type == 8) {
            MH.MessageListLimit(MessageType , messageContent: TextContentOfMessage);
        } else {
            handleImageMessages(MessageType, TextContentOfMessage);
        }
}

    public void LoadGame()
    {
        // Does the file exist?
        if (File.Exists(saveFile)) {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(saveFile);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            JsonUtility.FromJsonOverwrite(fileContents, Shared);
        }

        for (int i = 0; i < Shared.savedMessages.Count; i++) {
            // Set the person the messages are for
            Shared.contactPush = Shared.whosTheMessageFor[i];
            LoadAllMessages(i);
        }

        Shared.savedMessages = new List<string>();
        Shared.whosTheMessageFor = new List<int>();
        Shared.typeOfText = new List<int>();

        MH.ChapterSelect(Shared.ChapterList[Shared.CurrChapIndex], Shared.CurrSubChapIndex, Shared.CurrText);
    }

    void SaveAllMessages(Transform MessageList, int Person) {
        for(int j = 0; j < MessageList.childCount; j++) {
                Shared.whosTheMessageFor.Add(Person);

                Transform messageListItem = MessageList.GetChild(j);
                int type = int.Parse(messageListItem.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                Shared.typeOfText.Add(type);

                if(type == 0 || type == 1 || type == 6 || type == 8) {
                    GameObject textContent = messageListItem.GetChild(1).GetChild(0).GetChild(1).gameObject;
                    Shared.savedMessages.Add(textContent.GetComponent<TextMeshProUGUI>().text);
                } else {
                    GameObject textContent = messageListItem.transform.GetChild(1).GetChild(1).GetChild(1).gameObject;
                    Shared.savedMessages.Add(textContent.GetComponent<TextMeshProUGUI>().text);
                }

            }
    }

    void GetMessagesSnapshot() {
        for (int i = 0; i < Shared.ContactsList.Count; i++) {
            Transform messageList = Shared.content.GetChild(i);
            SaveAllMessages(messageList, i);
        }
    }

    public void SaveGame() {   

        GetMessagesSnapshot();

        // Serialize the object into JSON and save string.
        string jsonString = JsonUtility.ToJson(Shared);

        // Write JSON to file.
        File.WriteAllText(saveFile, jsonString);


        Shared.savedMessages = new List<string>();
        Shared.whosTheMessageFor = new List<int>();
        Shared.typeOfText = new List<int>();
    }
}


