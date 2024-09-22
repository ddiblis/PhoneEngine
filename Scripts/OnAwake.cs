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
using System.Drawing;

public class OnAwake : MonoBehaviour
{   
    public MessagingHandlers MH;
    public SaveManager SM;
    public ContactsHandler CH;
    public DBHandler DB;
    public SaveFile SF;
    public SettingsManager Settings;
    public DBUpdator DBUpdate;

    void Awake() {
        // These functions belong here, stop trying to move them
        #if UNITY_EDITOR 
            DBUpdate.UpdateDB();
        #endif
        DB.LoadDB();

        if (File.Exists($"{Application.persistentDataPath}/SaveInfo.json")) {
            SM.LoadSavesFile("/SaveInfo.json");
            SM.LoadMostRecent();
        } else {
            MH.NewGame();
        }

        Settings.MuteAudio(SF.saveFile.Settings.MuteGame);

        if (SF.saveFile.Settings.FullScreen) {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        } else {
            // Get the current screen height
            int height = Screen.currentResolution.height - 30; 

            // Calculate the width based on the desired aspect ratio of 5:9
            int width = Mathf.RoundToInt(height * (5f / 9f));

            Screen.SetResolution(width, height, false);
        }
        
        StartCoroutine(SM.AutoSave());

    }

}
