using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

    public SaveFile SF;
    public SaveManager SM;
    public MessagingHandlers MH;
    public DBHandler DB;
    public GeneralHandlers gen;
    public SavesFile Saves;
    public SharedObjects Shared;
    public PreFabs preFabs;
    public Transform Canvas;
    public Transform SettingsList;
    public Transform MuteCard;
    public Transform FasterReplies;
    public Transform ResetCard;
    public Transform GameModeCard;
    public Transform FullScreenCard;


    public void PopulateSettingsList() {
        // C# not passing by reference leads to trash like this
        Transform MuteCardClone = Instantiate(MuteCard, Vector3.zero, Quaternion.identity, SettingsList);
        Transform FastRepliesCardClone = Instantiate(FasterReplies, Vector3.zero, Quaternion.identity, SettingsList);
        Transform FullScreenCardClone = Instantiate(FullScreenCard, Vector3.zero, Quaternion.identity, SettingsList);
        Transform GameModeCardClone = Instantiate(GameModeCard, Vector3.zero, Quaternion.identity, SettingsList);
        Transform ResetCardClone = Instantiate(ResetCard, Vector3.zero, Quaternion.identity, SettingsList);


        GameObject MuteSlider = MuteCardClone.GetChild(2).gameObject;
        GameObject FasterRepliesSlider = FastRepliesCardClone.GetChild(2).gameObject;
        GameObject FullScreenSlider = FullScreenCardClone.GetChild(2).gameObject;
        GameObject GameModeSlider = GameModeCardClone.GetChild(2).gameObject;

        if (SF.saveFile.Settings.FasterReplies){
            FasterRepliesSlider.GetComponent<Animator>().Play("Turn-On-Slider");
        }
        FasterRepliesSlider.GetComponent<Button>().onClick.AddListener(() => {
            SettingsList.GetComponent<AudioSource>().Play();
            if (!SF.saveFile.Settings.FasterReplies) {
                SF.saveFile.Settings.FasterReplies = true;
                FasterRepliesSlider.GetComponent<Animator>().Play("Turn-On-Slider");
            } else {
                SF.saveFile.Settings.FasterReplies = false;
                FasterRepliesSlider.GetComponent<Animator>().Play("Turn-Off-Slider");
            }
        });

        // Disables Fast replies button if chapter has not been completed yet
        #if !DEVELOPMENT_BUILD || UNITY_EDITOR
        if (!SF.saveFile.ChapterList[SF.saveFile.CurrStoryPoint.ChapIndex].seen) {
            gen.Show(FastRepliesCardClone.GetChild(3));
        }
        #endif

        if (SF.saveFile.Settings.MuteGame){
            MuteSlider.GetComponent<Animator>().Play("Turn-On-Slider");
        }
        MuteSlider.GetComponent<Button>().onClick.AddListener(() => {
            SettingsList.GetComponent<AudioSource>().Play();

            if (!SF.saveFile.Settings.MuteGame) {
                SF.saveFile.Settings.MuteGame = true;
                MuteSlider.GetComponent<Animator>().Play("Turn-On-Slider");
                MuteAudio(SF.saveFile.Settings.MuteGame);
            } else {
                SF.saveFile.Settings.MuteGame = false;
                MuteSlider.GetComponent<Animator>().Play("Turn-Off-Slider");
                MuteAudio(SF.saveFile.Settings.MuteGame);
            }
        });

        if (SF.saveFile.Settings.GameMode){
            GameModeSlider.GetComponent<Animator>().Play("Turn-On-Slider");
        }
        GameModeSlider.GetComponent<Button>().onClick.AddListener(() => {
            SettingsList.GetComponent<AudioSource>().Play();

            if (!SF.saveFile.Settings.GameMode) {
                SF.saveFile.Settings.GameMode = true;
                GameModeSlider.GetComponent<Animator>().Play("Turn-On-Slider");
            } else {
                SF.saveFile.Settings.GameMode = false;
                GameModeSlider.GetComponent<Animator>().Play("Turn-Off-Slider");
            }
        });


        if (SF.saveFile.Settings.FullScreen){
            FullScreenSlider.GetComponent<Animator>().Play("Turn-On-Slider");
        }
        FullScreenSlider.GetComponent<Button>().onClick.AddListener(() => {
            SettingsList.GetComponent<AudioSource>().Play();

            if (!SF.saveFile.Settings.FullScreen) {
                SF.saveFile.Settings.FullScreen = true;
                FullScreenSlider.GetComponent<Animator>().Play("Turn-On-Slider");
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            } else {
                SF.saveFile.Settings.FullScreen = false;
                FullScreenSlider.GetComponent<Animator>().Play("Turn-Off-Slider");
                // Get the current screen height
                int height = Screen.currentResolution.height - 30; 

                // Calculate the width based on the desired aspect ratio of 5:9
                int width = Mathf.RoundToInt(height * (5f / 9f));

                Screen.SetResolution(width, height, false);
            }
        });


        ResetCardClone.GetComponent<Button>().onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            OpenResetModal(FasterRepliesSlider.GetComponent<Animator>(), MuteSlider.GetComponent<Animator>());
        });
    }

    void OpenResetModal(Animator FasterRepliesSlider, Animator MuteSlider) {
        Transform LoadModalWindowClone = 
            Instantiate(preFabs.LoadModalWindow, Vector3.zero, Quaternion.identity, Canvas);

        LoadModalWindowClone.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Delete all progress and saves?";
        LoadModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");
        Button confirmButton = LoadModalWindowClone.GetChild(3).GetComponent<Button>();
        Button cancelButton = LoadModalWindowClone.GetChild(2).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            Destroy(LoadModalWindowClone.gameObject);
        });

        confirmButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            FasterRepliesSlider.Play("Turn-Off-Slider");
            MuteSlider.Play("Turn-Off-Slider");
            SM.RefreshApps();
            ResetSaves();
            DB.GeneratePhotoList();
            SM.RefreshSaveList();
            MH.NewGame();
            Destroy(LoadModalWindowClone.gameObject);
        });
    }

    public void MuteAudio(bool state) {
        Shared.Wallpaper.GetComponent<AudioSource>().mute = state;
        Shared.notificationArea.GetComponent<AudioSource>().mute = state;
        SettingsList.GetComponent<AudioSource>().mute = state;
        Shared.content.GetComponent<AudioSource>().mute = state;
    }

    public void ResetSaves() {
        SF.saveFile = new SaveFileRoot();
        // Since it's not a monobehaviour file this needs to be done manually
        Saves.MostRecentSaveIndex = 0;
        Saves.NameOfSaves = new List<string>();
        Saves.ChapterOfSaves = new List<int>();
        Saves.TendencyOfSaves = new List<int>();
        Saves.DateTimeOfSave = new List<string>();
        Saves.AutoSaveMostRecent = false;
        Saves.AutoSaveChapter = 0;
        Saves.AutoSaveTendency = 0;
        Saves.AutoSaveDateTime = "";

        for (int i = 0; i < SF.saveFile.NumberOfSaves; i++){
            string saveFile = $"{Application.persistentDataPath}/{i}Save.json";
            if(File.Exists(saveFile)) {
                File.Delete(saveFile);
            }
        }
        string SaveInfo = $"{Application.persistentDataPath}/SaveInfo.json";
        if(File.Exists(SaveInfo)) {
            File.Delete(SaveInfo);
        }    
    }

    public void DestroyAllList() {
        foreach (Transform child in SettingsList) {
            Destroy(child.gameObject);
        }
    }
}
