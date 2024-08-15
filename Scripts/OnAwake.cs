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
    public ChapImport chap;


    void Awake() {

        MH.BackButton();
        MH.GenerateContactsList();
        MH.GenerateMessageLists();
        CH.GenerateContactCards();

        if (File.Exists(Application.persistentDataPath + "/SaveInfo.json")) {
            SM.LoadSavesFile("/SaveInfo.json");
            if (!SavesInfo.AutoSaveMostRecent) {
                SM.LoadGame("/" + SavesInfo.MostRecentSaveIndex + "Save.json");
            } else {
                SM.LoadGame("/AutoSave.json");
            }
            gen.SetWallPaper(saved.currWallPaper);
        } else {
            chap.GenerateChapterList();
            saved.currWallPaper = "gf-car";
            gen.SetWallPaper(saved.currWallPaper);
            SM.GenerateSaves(5);
            MH.NewGame();
        }

        SM.CreateSaveCards();
        StartCoroutine(SM.AutoSave());

    }

}
