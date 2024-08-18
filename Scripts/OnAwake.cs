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
    public ContactsHandler CH;
    public DBHandler DB;



    void Awake() {
        DB.LoadDB();
        MH.BackButton();
        MH.GenerateContactsList();
        MH.GenerateMessageLists();
        CH.GenerateContactCards();

        if (File.Exists(Application.persistentDataPath + "/SaveInfo.json")) {
            SM.LoadSavesFile("/SaveInfo.json");
            SM.LoadMostRecent();
        } else {
            MH.NewGame();
        }

        SM.CreateSaveCards();
        StartCoroutine(SM.AutoSave());

    }

}
