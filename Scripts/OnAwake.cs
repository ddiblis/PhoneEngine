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
using System.Security.Cryptography;

public class OnAwake : MonoBehaviour
{
    public MessagingHandlers MH;
    public SaveManager SM;
    public GallaryHandler GH;
    public ContactsHandler CH;
    public GeneralHandlers gen;
    public SharedObjects Shared;
    public SavesFile SavesInfo;
    public SavedItems saved;

    void Awake() {

        MH.GenerateContactsList();
        MH.GenerateMessageLists();
        MH.BackButton();
        
        // Generates contact cards for each contact based on list.
        for (int i = 0; i < saved.ContactsList.Count; i++) {
            Sprite img = Resources.Load("Images/Headshots/" + i + saved.ContactsList[i], typeof(Sprite)) as Sprite;            
            CH.addContactCard(img, saved.ContactsList[i], i);
        } 

        if (File.Exists(Application.persistentDataPath + "/SaveInfo.json")) {
            SM.LoadSavesFile("/SaveInfo.json");
            if (!SavesInfo.AutoSaveMostRecent) {
                SM.LoadGame("/" + SavesInfo.MostRecentSaveIndex + "Save.json");
            } else {
                SM.LoadGame("/AutoSave.json");
            }
            gen.SetWallPaper(saved.currWallPaper);
        } else {
            string[] FileList = Directory.GetFiles(Application.streamingAssetsPath + "/Chapters/","*.NA");
            foreach (string File in FileList) {
                saved.ChapterList.Add(File[(File.LastIndexOf("/")+1)..^3]);
            }
            saved.currWallPaper = "gf-car";
            gen.SetWallPaper(saved.currWallPaper);
            MH.NewGame();
            saved.NumberOfSaves = 5;
            for(int i = 0; i < saved.NumberOfSaves; i++) {
                SavesInfo.ChapterOfSaves.Add(0);
                SavesInfo.NameOfSaves.Add("");
                SavesInfo.DateTimeOfSave.Add("" + DateTime.Now);
            }
        }

        SM.CreateSaveCards();
        StartCoroutine(SM.AutoSave());

    }

}
