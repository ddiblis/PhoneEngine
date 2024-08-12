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

    void Awake() {

        MH.GenerateContactsList();
        MH.GenerateMessageLists();
        MH.BackButton();


        
        // Generates contact cards for each contact based on list.
        for (int i = 0; i < Shared.ContactsList.Count; i++) {
            Sprite img = Resources.Load("Images/Headshots/" + i + Shared.ContactsList[i], typeof(Sprite)) as Sprite;            
            CH.addContactCard(img, Shared.ContactsList[i], i);
        } 

        string[] SaveFiles = Directory.GetFiles(Application.persistentDataPath,"*Save.json");
        if (SaveFiles.Length > 0) {
            string SaveInfoFile = File.ReadAllText(Application.persistentDataPath + "/SaveInfo" + ".json");
            SM.LoadSavesFile(SaveInfoFile);
            SM.LoadGame(SaveFiles[SavesInfo.MostRecentSaveIndex]);
        } else {
            MH.NewGame();
            Shared.NumberOfSaves = 5;
            for(int i = 0; i < Shared.NumberOfSaves; i++) {
                SavesInfo.ChapterOfSaves.Add(0);
                SavesInfo.NameOfSaves.Add("");
                SavesInfo.DateTimeOfSave.Add("" + DateTime.Now);
            }
        }

        SM.CreateSaveCards();


        gen.Hide(CH.contactsApp);
        gen.Hide(Shared.textingApp);
        gen.Hide(GH.GallaryApp);
        gen.Hide(Shared.ModalWindow);
        gen.Hide(SM.SavesApp);  

    }

}
