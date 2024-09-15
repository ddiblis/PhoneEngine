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
    public Transform SaveList;
    public GeneralHandlers gen;
    public PreFabs Prefabs;
    public SharedObjects Shared;
    public SavesFile SavesInfo;
    public MessagingHandlers MH;
    public Transform Canvas;
    public Transform AutoSaveCard;
    public ChapImport chap;
    public InstaPostsManager IPM;
    public SaveFile SF;
    public DBHandler DB;
    



    public void RefreshSaveList() {
        for (int i = 3; i < SF.saveFile.NumberOfSaves + 3; i++) {
            Destroy(SaveList.GetChild(i).gameObject);
        }
        CreateSaveCards();
    }

    public void RefreshApps() {
        if (Shared.notificationArea.childCount > 0) {
            Destroy(Shared.notificationArea.GetChild(0).gameObject);
        }
        if (Shared.choices.childCount > 0) {
            foreach (Transform child in Shared.choices) {
		        Destroy(child.gameObject);
		    }
        }
        foreach (Transform MessageList in Shared.content) {
            if (MessageList.childCount > 0) {
                foreach (Transform message in MessageList) {
                    Destroy(message.gameObject);
                }
            }   
		}
    }

    public void CreateSaveCards() {

        for (int i = 0; i < SF.saveFile.NumberOfSaves; i++) {
            string tendency = null;

            GameObject SaveCardClone =
                Instantiate(Prefabs.SaveCard, new Vector3(0, 0, 0), Quaternion.identity, SaveList);

            Transform TextContainer = SaveCardClone.transform.GetChild(3);

            if (SavesInfo.ChapterOfSaves[i] != 0) {
                TextContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = SavesInfo.NameOfSaves[i];
                TextContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    SavesInfo.ChapterOfSaves[i] > SF.saveFile.ChapterList.Count ?
                        "End Of update" :
                        "Chapter " + SavesInfo.ChapterOfSaves[i];
                switch(SavesInfo.TendencyOfSaves[i]) {
                    case (int)Tendency.Neutral:
                        tendency = "Neutral";
                    break;
                }
                TextContainer.GetChild(2).GetComponent<TextMeshProUGUI>().text = tendency;
                TextContainer.GetChild(3).GetComponent<TextMeshProUGUI>().text = SavesInfo.DateTimeOfSave[i];
            }

            // Set functionality of save and load buttons
            Button SaveButton = SaveCardClone.transform.GetChild(1).GetComponent<Button>(); 
            Button LoadButton = SaveCardClone.transform.GetChild(2).GetComponent<Button>(); 

            string saveFile = "/" + i + "Save" + ".json";
            string SaveInfo = "/SaveInfo.json";
            int indx = i;
            SaveButton.onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                OpenSaveModal(indx, SaveInfo, saveFile);
            });
            LoadButton.onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                if (File.Exists(Application.persistentDataPath + saveFile)) { 
                    OpenLoadModal(saveFile, SaveInfo);
                }
            });
        }
    }

    public void CloseApp() {
        for (int i = 3; i < SF.saveFile.NumberOfSaves + 3; i++) {
            Destroy(SaveList.GetChild(i).gameObject);
        }
    }

    void OpenSaveModal(int indx, string SaveInfo, string saveFile) {
        Transform SaveModalWindowClone =
            Instantiate(Prefabs.SaveModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);

        SaveModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");

        Transform popup = SaveModalWindowClone.GetChild(2).transform;
        TMP_InputField input = popup.GetComponent<TMP_InputField>();

        string SaveName = "";
        input.onEndEdit.AddListener(x => {SaveName = x;});

        Button confirmButton = popup.GetChild(2).GetComponent<Button>();
        Button cancelButton = popup.GetChild(1).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            Destroy(SaveModalWindowClone.gameObject);
        });
        confirmButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            SavesInfo.MostRecentSaveIndex = indx;
            SavesInfo.ChapterOfSaves[indx] = SF.saveFile.CurrStoryPoint.ChapIndex + 1;
            SavesInfo.DateTimeOfSave[indx] = "" + DateTime.Now;
            SavesInfo.NameOfSaves[indx] = SaveName.Length > 0 ? SaveName : "SaveFile " + indx;
            SavesInfo.TendencyOfSaves[indx] = SF.saveFile.Stats.Tendency;
            SavesInfo.AutoSaveMostRecent = false;
            CreateSavesFile(SaveInfo);
            SaveGame(saveFile);
            RefreshSaveList();
            Destroy(SaveModalWindowClone.gameObject);
        });
    }

    public void PopulateAutoSaveCard() {
        string Tendency = null;
        Transform TextContainer = AutoSaveCard.GetChild(2);
        TextContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            SavesInfo.AutoSaveChapter > SF.saveFile.ChapterList.Count ?
                "End Of update" :
                "Chapter " + SavesInfo.AutoSaveChapter;
        switch(SavesInfo.AutoSaveTendency) {
                    case 0:
                        Tendency = "Neutral";
                    break;
                    case 1:
                        Tendency = "Submissive";
                    break;
                    case 2:
                        Tendency = "Dominant";
                    break;
                }
        TextContainer.GetChild(2).GetComponent<TextMeshProUGUI>().text = Tendency;
        TextContainer.GetChild(3).GetComponent<TextMeshProUGUI>().text = SavesInfo.AutoSaveDateTime;
    }

    public void OpenAutoSaveModal() {
        OpenLoadModal("/AutoSave.json", "/SaveInfo.json");
    }

    public IEnumerator AutoSave() {
        string saveFile = "/AutoSave.json";
        string SaveInfo = "/SaveInfo.json";
        yield return new WaitForSeconds(180f);

        SavesInfo.AutoSaveChapter = SF.saveFile.CurrStoryPoint.ChapIndex + 1;
        SavesInfo.AutoSaveDateTime = "" + DateTime.Now;
        SavesInfo.AutoSaveMostRecent = true;
        SavesInfo.AutoSaveTendency = SF.saveFile.Stats.Tendency;

        PopulateAutoSaveCard();
        CreateSavesFile(SaveInfo);
        
        // Since auto save can happen while viewing messages, if you reload the code will think you're still viewing those messages
        // This resets it to not viewing for a fram to save, then put the corrent index back into place
        int currentlyViewing = SF.saveFile.selectedIndex;
        SF.saveFile.selectedIndex = int.MinValue;
        
        SaveGame(saveFile);
        
        SF.saveFile.selectedIndex = currentlyViewing;

        StartCoroutine(AutoSave());
    }

    void OpenLoadModal(string saveFile, string SaveInfo) {
        Transform LoadModalWindowClone = 
            Instantiate(Prefabs.LoadModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);

        LoadModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");
        Button confirmButton = LoadModalWindowClone.GetChild(3).GetComponent<Button>();
        Button cancelButton = LoadModalWindowClone.GetChild(2).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            Destroy(LoadModalWindowClone.gameObject);
        });

        confirmButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            LoadSavesFile(SaveInfo);
            RefreshApps();
            LoadGame(saveFile);
            gen.SetWallPaper(SF.saveFile.CurrWallPaper);
            Destroy(LoadModalWindowClone.gameObject);
        });
    }


    public void LoadSavesFile(string SaveInfoFile) {
        string SaveFileName = Application.persistentDataPath + SaveInfoFile;
        string fileContents = File.ReadAllText(SaveFileName);

        JsonUtility.FromJsonOverwrite(fileContents, SavesInfo);
    }

    public void LoadGame(string saveFile) {

        string SaveFileName = Application.persistentDataPath + saveFile;
        string fileContents = File.ReadAllText(SaveFileName);

        JsonUtility.FromJsonOverwrite(fileContents, SF.saveFile);

        SF.saveFile.selectedIndex = int.MinValue;
        DB.GenerateChapterList();
        DB.GeneratePhotoList();
        DB.GenerateMidrollsList();
        IPM.GenPostsList();


        // Populates the messageLists with the content from the loaded Json
        for (int i = 0; i < SF.saveFile.SavedMessages.Count; i++) {
            // Set the person the messages are for
            SF.saveFile.contactPush = SF.saveFile.SavedMessages[i].WhosIsItFor;
            MH.MessageListLimit(new TextMessage {
            Type = SF.saveFile.SavedMessages[i].TypeOfText,
            TextContent = SF.saveFile.SavedMessages[i].TextContent
        });
        }

        SF.saveFile.SavedMessages = new List<SavedMessage>();

        if (SF.saveFile.PlayingMidRoll) {
            MH.ChapterSelect(
                "Midrolls",
                SF.saveFile.MidRolls[SF.saveFile.CurrMidRoll].MidrollName,
                SF.saveFile.CurrStoryPoint.SubChapIndex,
                SF.saveFile.CurrStoryPoint.CurrTextIndex
            );
        } else {
            if (SF.saveFile.CurrStoryPoint.ChapIndex < SF.saveFile.ChapterList.Count){
                MH.ChapterSelect(
                    "Chapters",
                    SF.saveFile.ChapterList[SF.saveFile.CurrStoryPoint.ChapIndex],
                    SF.saveFile.CurrStoryPoint.SubChapIndex,
                    SF.saveFile.CurrStoryPoint.CurrTextIndex
                );
            }
        }
        PopulateAutoSaveCard();
    }

    void SaveAllMessages(Transform MessageList, int Person) {
        for(int j = 0; j < MessageList.childCount; j++) {
            SavedMessage savedMessage = new() { WhosIsItFor = Person };

            Transform messageListItem = MessageList.GetChild(j);
            int type = int.Parse(
                messageListItem.GetChild(0).GetComponent<TextMeshProUGUI>().text
            );

            savedMessage.TypeOfText = type;
            GameObject textContent = messageListItem.GetChild(1).GetChild(0).GetChild(0).gameObject;
            savedMessage.TextContent = textContent.GetComponent<TextMeshProUGUI>().text;
 
            SF.saveFile.SavedMessages.Add(savedMessage);
        }
    }

    void GetMessagesSnapshot() {
        for (int i = 0; i < SF.saveFile.ContactsList.Count; i++) {
            Transform messageList = Shared.content.GetChild(i);
            SaveAllMessages(messageList, i);
        }
    }


    public void SaveGame(string saveFile) {   

        GetMessagesSnapshot();

        string SaveFileName = Application.persistentDataPath + saveFile;
        string jsonString = JsonUtility.ToJson(SF.saveFile, true);

        File.WriteAllText(SaveFileName, jsonString);


        SF.saveFile.SavedMessages = new List<SavedMessage>();
    }

    void CreateSavesFile(string SaveInfoFile) {
        
        string SaveFileName = Application.persistentDataPath + SaveInfoFile;

        string jsonString = JsonUtility.ToJson(SavesInfo, true);

        File.WriteAllText(SaveFileName, jsonString);
    }

    public void GenerateSaves(int NumOfSaves) {
        SF.saveFile.NumberOfSaves = NumOfSaves;
        for(int i = 0; i < SF.saveFile.NumberOfSaves; i++) {
            SavesInfo.ChapterOfSaves.Add(0);
            SavesInfo.NameOfSaves.Add("");
            SavesInfo.DateTimeOfSave.Add("" + DateTime.Now);
            SavesInfo.TendencyOfSaves.Add(0);
        }
    }

    public void LoadMostRecent() {
         if (!SavesInfo.AutoSaveMostRecent) {
            LoadGame("/" + SavesInfo.MostRecentSaveIndex + "Save.json");
        } else {
            LoadGame("/AutoSave.json");
        }
        gen.SetWallPaper(SF.saveFile.CurrWallPaper);
    }
}


