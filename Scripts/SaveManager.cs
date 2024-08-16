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
    public Transform Canvas;
    public SavedItems saved;
    public Transform AutoSaveCard;
    public ChapImport chap;
    public ContactsHandler CH;
    public InstaPostsManager IPM;



    void RefreshSaveList() {
        for (int i = 3; i < saved.NumberOfSaves + 3; i++) {
            Destroy(SaveList.GetChild(i).gameObject);
        }
        CreateSaveCards();
    }

    public void RefreshApps() {
        if (Shared.notificationArea.childCount > 0) {
            Destroy(Shared.notificationArea.GetChild(0).gameObject);
        }
        if (Shared.choices.childCount > 0) {
            for (int i = 0; i < Shared.choices.childCount; i++) {
                Destroy(Shared.choices.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < saved.ContactsList.Count; i++) {
            Transform messageList = Shared.content.GetChild(i);
            if (messageList.childCount > 0) {
                for (int j = 0; j < messageList.childCount; j++) {
                    Destroy(messageList.GetChild(j).gameObject);
                }
            }
        }
    }

    public void CreateSaveCards() {

        for (int i = 0; i < saved.NumberOfSaves; i++) {

            GameObject SaveCardClone = Instantiate(Prefabs.SaveCard, new Vector3(0, 0, 0), Quaternion.identity, SaveList);

            Transform TextContainer = SaveCardClone.transform.GetChild(3);

            // if (SavesInfo.ChapterOfSaves.Count > i && SavesInfo.ChapterOfSaves[i] != 0) {

            if (SavesInfo.ChapterOfSaves[i] != 0) {
                TextContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = SavesInfo.NameOfSaves[i];
                TextContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = SavesInfo.ChapterOfSaves[i] > saved.ChapterList.Count ? "End Of update" : "Chapter " + SavesInfo.ChapterOfSaves[i];
                // TextContainer.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Chapter " + Shared.TendencyOfSaves[i];
                TextContainer.GetChild(3).GetComponent<TextMeshProUGUI>().text = SavesInfo.DateTimeOfSave[i];
            }

            // Set functionality of save and load buttons
            Button SaveButton = SaveCardClone.transform.GetChild(1).GetComponent<Button>(); 
            Button LoadButton = SaveCardClone.transform.GetChild(2).GetComponent<Button>(); 

            string saveFile = "/" + i + "Save" + ".json";
            string SaveInfo = "/SaveInfo.json";
            int indx = i;
            SaveButton.onClick.AddListener(() => {
                openSaveModal(indx, SaveInfo, saveFile);
            });
            LoadButton.onClick.AddListener(() => {
                if (File.Exists(Application.persistentDataPath + saveFile)) { 
                    openLoadModal(saveFile, SaveInfo);
                }
            });
        }
    }

    void openSaveModal(int indx, string SaveInfo, string saveFile) {
        Transform SaveModalWindowClone = Instantiate(Prefabs.SaveModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);
        SaveModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");
        Transform popup = SaveModalWindowClone.GetChild(2).transform;
        // InputField input = popup.GetComponent<InputField>();
        Button confirmButton = popup.GetChild(2).GetComponent<Button>();
        Button cancelButton = popup.GetChild(1).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => {
            Destroy(SaveModalWindowClone.gameObject);
        });
        confirmButton.onClick.AddListener(() => {
            SavesInfo.MostRecentSaveIndex = indx;
            SavesInfo.ChapterOfSaves[indx] = saved.CurrChapIndex + 1;
            SavesInfo.DateTimeOfSave[indx] = "" + DateTime.Now;
            // SavesInfo.NameOfSaves[indx] = input.text;
            SavesInfo.AutoSaveMostRecent = false;
            createSavesFile(SaveInfo);
            SaveGame(saveFile);
            RefreshSaveList();
            Destroy(SaveModalWindowClone.gameObject);
        });
    }

    public void populateAutoSaveCard() {
        Transform TextContainer = AutoSaveCard.GetChild(2);
        TextContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = SavesInfo.AutoSaveChapter > saved.ChapterList.Count ? "End Of update" : "Chapter " + SavesInfo.AutoSaveChapter;
        // TextContainer.GetChild(2).GetComponent<TextMeshProUGUI>().text = Shared.TendencyOfSaves[i];
        TextContainer.GetChild(3).GetComponent<TextMeshProUGUI>().text = SavesInfo.AutoSaveDateTime;
    }

    public void OpenAutoSaveModal() {
        openLoadModal("/AutoSave.json", "/SaveInfo.json");
    }

    public IEnumerator AutoSave() {
        string saveFile = "/AutoSave.json";
        string SaveInfo = "/SaveInfo.json";
        yield return new WaitForSeconds(180f);

        SavesInfo.AutoSaveChapter = saved.CurrChapIndex + 1;
        SavesInfo.AutoSaveDateTime = "" + DateTime.Now;
        SavesInfo.AutoSaveMostRecent = true;

        populateAutoSaveCard();
        createSavesFile(SaveInfo);
        
        // Since auto save can happen while viewing messages, if you reload the code will think you're still viewing those messages
        // This resets it to not viewing for a fram to save, then put the corrent index back into place
        int currentlyViewing = saved.selectedIndex;
        saved.selectedIndex = int.MinValue;
        
        SaveGame(saveFile);
        
        saved.selectedIndex = currentlyViewing;

        StartCoroutine(AutoSave());
    }

    void openLoadModal(string saveFile, string SaveInfo) {
        Transform LoadModalWindowClone = Instantiate(Prefabs.LoadModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);
        LoadModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");
        Button confirmButton = LoadModalWindowClone.GetChild(3).GetComponent<Button>();
        Button cancelButton = LoadModalWindowClone.GetChild(2).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => {
            Destroy(LoadModalWindowClone.gameObject);
        });

        confirmButton.onClick.AddListener(() => {
            LoadSavesFile(SaveInfo);
            RefreshApps();
            LoadGame(saveFile);
            gen.SetWallPaper(saved.currWallPaper);
            Destroy(LoadModalWindowClone.gameObject);
        });
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
        int type = saved.typeOfText[MessageIndex];
        TypeOfText MessageType = (TypeOfText)type;
        string TextContentOfMessage = saved.savedMessages[MessageIndex];
        if(type == 0 || type == 1 || type == 6 || type == 8) {
            MH.MessageListLimit(MessageType , messageContent: TextContentOfMessage);
        } else {
            handleImageMessages(MessageType, TextContentOfMessage);
        }
    }

    public void LoadSavesFile(string SaveInfoFile) {
        string SaveFileName = Application.persistentDataPath + SaveInfoFile;

        string fileContents = File.ReadAllText(SaveFileName);

        JsonUtility.FromJsonOverwrite(fileContents, SavesInfo);
    }

    public void LoadGame(string saveFile) {

        string SaveFileName = Application.persistentDataPath + saveFile;
        string fileContents = File.ReadAllText(SaveFileName);

        JsonUtility.FromJsonOverwrite(fileContents, saved);

        
        chap.GenerateChapterList();
        IPM.GenPostsList();


        // Populates the messageLists with the content from the loaded Json
        for (int i = 0; i < saved.savedMessages.Count; i++) {
            // Set the person the messages are for
            saved.contactPush = saved.whosTheMessageFor[i];
            LoadAllMessages(i);
        }

        saved.savedMessages = new List<string>();
        saved.whosTheMessageFor = new List<int>();
        saved.typeOfText = new List<int>();

        if (saved.CurrChapIndex < saved.ChapterList.Count){
            MH.ChapterSelect(saved.ChapterList[saved.CurrChapIndex], saved.CurrSubChapIndex, saved.CurrText);
        }
        populateAutoSaveCard();
    }

    void SaveAllMessages(Transform MessageList, int Person) {
        for(int j = 0; j < MessageList.childCount; j++) {
                saved.whosTheMessageFor.Add(Person);

                Transform messageListItem = MessageList.GetChild(j);
                int type = int.Parse(messageListItem.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                saved.typeOfText.Add(type);

                if(type == 0 || type == 1 || type == 6 || type == 8) {
                    GameObject textContent = messageListItem.GetChild(1).GetChild(0).GetChild(0).gameObject;
                    saved.savedMessages.Add(textContent.GetComponent<TextMeshProUGUI>().text);
                } else {
                    GameObject textContent = messageListItem.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
                    saved.savedMessages.Add(textContent.GetComponent<TextMeshProUGUI>().text);
                }

            }
    }

    void GetMessagesSnapshot() {
        for (int i = 0; i < saved.ContactsList.Count; i++) {
            Transform messageList = Shared.content.GetChild(i);
            SaveAllMessages(messageList, i);
        }
    }

    public void SaveGame(string saveFile) {   

        GetMessagesSnapshot();

        string SaveFileName = Application.persistentDataPath + saveFile;

        string jsonString = JsonUtility.ToJson(saved);

        File.WriteAllText(SaveFileName, jsonString);


        saved.savedMessages = new List<string>();
        saved.whosTheMessageFor = new List<int>();
        saved.typeOfText = new List<int>();
    }
    void createSavesFile(string SaveInfoFile) {
        
        string SaveFileName = Application.persistentDataPath + SaveInfoFile;

        string jsonString = JsonUtility.ToJson(SavesInfo);

        File.WriteAllText(SaveFileName, jsonString);
    }

    public void GenerateSaves(int NumOfSaves) {
        saved.NumberOfSaves = NumOfSaves;
        for(int i = 0; i < saved.NumberOfSaves; i++) {
            SavesInfo.ChapterOfSaves.Add(0);
            SavesInfo.NameOfSaves.Add("");
            SavesInfo.DateTimeOfSave.Add("" + DateTime.Now);
        }
    }

    public void LoadMostRecent() {
         if (!SavesInfo.AutoSaveMostRecent) {
            LoadGame("/" + SavesInfo.MostRecentSaveIndex + "Save.json");
        } else {
            LoadGame("/AutoSave.json");
        }
        gen.SetWallPaper(saved.currWallPaper);
    }
}


