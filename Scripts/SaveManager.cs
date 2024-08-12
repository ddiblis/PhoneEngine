using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.IO;
using System;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public Transform SavesApp;
    public Transform SaveList;
    public GeneralHandlers gen;
    public PreFabs Prefabs;
    public SharedObjects Shared;
    public SavesFile SavesInfo;
    public MessagingHandlers MH;

    // string saveFile;

    void RefreshSaveList() {
        for (int i = 3; i < Shared.NumberOfSaves + 3; i++) {
            Destroy(SaveList.GetChild(i).gameObject);
        }
        CreateSaveCards();
    }

    void RefreshApps() {
        if (Shared.notificationArea.childCount > 0) {
            Destroy(Shared.notificationArea.GetChild(0).gameObject);
        }
        if (Shared.choices.childCount > 0) {
            for (int i = 0; i < Shared.choices.childCount; i++) {
                Destroy(Shared.choices.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < Shared.ContactsList.Count; i++) {
            Transform messageList = Shared.content.GetChild(i);
            if (messageList.childCount > 0) {
                for (int j = 0; j < messageList.childCount; j++) {
                    Destroy(messageList.GetChild(j).gameObject);
                }
            }
        }
    }

    public void CreateSaveCards() {

        for (int i = 0; i < Shared.NumberOfSaves; i++) {

            GameObject SaveCardClone = Instantiate(Prefabs.SaveCard, new Vector3(0, 0, 0), Quaternion.identity, SaveList);

            Transform TextContainer = SaveCardClone.transform.GetChild(3);

            // if (SavesInfo.ChapterOfSaves.Count > i && SavesInfo.ChapterOfSaves[i] != 0) {

            if (SavesInfo.ChapterOfSaves[i] != 0) {
                TextContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = SavesInfo.NameOfSaves[i];
                TextContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Chapter " + SavesInfo.ChapterOfSaves[i];
                // TextContainer.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Chapter " + Shared.TendencyOfSaves[i];
                TextContainer.GetChild(3).GetComponent<TextMeshProUGUI>().text = SavesInfo.DateTimeOfSave[i];
            }

            // Set functionality of save and load buttons
            Button SaveButton = SaveCardClone.transform.GetChild(1).GetComponent<Button>(); 
            Button LoadButton = SaveCardClone.transform.GetChild(2).GetComponent<Button>(); 
            string saveFile = Application.persistentDataPath + "/" + i + "Save" + ".json";
            string SaveInfo = Application.persistentDataPath + "/SaveInfo" + ".json";
            int indx = i;
            SaveButton.onClick.AddListener(() => {
                SavesInfo.MostRecentSaveIndex = indx;
                SavesInfo.ChapterOfSaves[indx] = Shared.CurrChapIndex + 1;
                SavesInfo.DateTimeOfSave[indx] = "" + DateTime.Now;
                createSavesFile(SaveInfo);
                SaveGame(saveFile);
                RefreshSaveList();
            });
            LoadButton.onClick.AddListener(() => {
                if (File.Exists(saveFile)) {
                    LoadSavesFile(SaveInfo);
                    RefreshApps();
                    LoadGame(saveFile);
                    for (int i = 0; i < Shared.UnlockedContacts.Count; i++) {
                        if (Shared.UnlockedContacts[i]){
                            gen.Show(Shared.cardsList.GetChild(i));
                        } else {
                            gen.Hide(Shared.cardsList.GetChild(i));
                        }
                    }
                }
            });
        }
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

    public void ReadFile(string saveFile) {
        if (File.Exists(saveFile)) {
            string fileContents = File.ReadAllText(saveFile);

            JsonUtility.FromJsonOverwrite(fileContents, Shared);
        }
    }

    public void LoadSavesFile(string SaveInfoFile) {
        
        string fileContents = File.ReadAllText(SaveInfoFile);

        JsonUtility.FromJsonOverwrite(fileContents, SavesInfo);
    }

    public void LoadGame(string saveFile) {
        string fileContents = File.ReadAllText(saveFile);

        JsonUtility.FromJsonOverwrite(fileContents, Shared);

        // Populates the messageLists with the content from the loaded Json
        for (int i = 0; i < Shared.savedMessages.Count; i++) {
            // Set the person the messages are for
            Shared.contactPush = Shared.whosTheMessageFor[i];
            LoadAllMessages(i);
        }

        Shared.savedMessages = new List<string>();
        Shared.whosTheMessageFor = new List<int>();
        Shared.typeOfText = new List<int>();

        if (Shared.CurrChapIndex < Shared.ChapterList.Count){
            MH.ChapterSelect(Shared.ChapterList[Shared.CurrChapIndex], Shared.CurrSubChapIndex, Shared.CurrText);
        }
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

    public void SaveGame(string saveFile) {   

        GetMessagesSnapshot();

        string jsonString = JsonUtility.ToJson(Shared);

        File.WriteAllText(saveFile, jsonString);


        Shared.savedMessages = new List<string>();
        Shared.whosTheMessageFor = new List<int>();
        Shared.typeOfText = new List<int>();
    }
    void createSavesFile(string SaveInfoFile) {

        string jsonString = JsonUtility.ToJson(SavesInfo);

        File.WriteAllText(SaveInfoFile, jsonString);
    }
}


