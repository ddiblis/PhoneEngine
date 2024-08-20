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
    public SavesFile Saves;
    public SharedObjects Shared;
    public PreFabs preFabs;
    public Transform Canvas;
    public Transform SettingsList;
    public Transform MuteCard;
    public Transform FasterReplies;
    public Transform ResetCard;
    public Transform FullScreenCard;


    public void PopulateSettingsList() {
        // C# not passing by reference leads to trash like this
        Transform MuteCardClone = Instantiate(MuteCard, new Vector3(0, 0, 0), Quaternion.identity, SettingsList);
        Transform FastRepliesCardClone = Instantiate(FasterReplies, new Vector3(0, 0, 0), Quaternion.identity, SettingsList);
        Transform FullScreenCardClone = Instantiate(FullScreenCard, new Vector3(0, 0, 0), Quaternion.identity, SettingsList);
        Transform ResetCardClone = Instantiate(ResetCard, new Vector3(0, 0, 0), Quaternion.identity, SettingsList);

        GameObject MuteSlider = MuteCardClone.GetChild(2).gameObject;
        GameObject FasterRepliesSlider = FastRepliesCardClone.GetChild(2).gameObject;
        GameObject FullScreenSlider = FullScreenCardClone.GetChild(2).gameObject;

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

        if (SF.saveFile.Settings.MuteGame){
            FasterRepliesSlider.GetComponent<Animator>().Play("Turn-On-Slider");
        }
        MuteSlider.GetComponent<Button>().onClick.AddListener(() => {
            SettingsList.GetComponent<AudioSource>().Play();

            if (!SF.saveFile.Settings.MuteGame) {
                SF.saveFile.Settings.MuteGame = true;
                MuteSlider.GetComponent<Animator>().Play("Turn-On-Slider");
            } else {
                SF.saveFile.Settings.MuteGame = false;
                MuteSlider.GetComponent<Animator>().Play("Turn-Off-Slider");
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
            openResetModal(FasterRepliesSlider.GetComponent<Animator>(), MuteSlider.GetComponent<Animator>());
        });
    }

    void openResetModal(Animator FasterRepliesSlider, Animator MuteSlider) {
        Transform LoadModalWindowClone = 
            Instantiate(preFabs.LoadModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);

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
            for (int i = 0; i < SF.saveFile.NumberOfSaves; i++){
                string saveFile = Application.persistentDataPath + "/" + i + "Save" + ".json";
                if(File.Exists(saveFile)) {
                    File.Delete(saveFile);
                }
            }
            string SaveInfo = Application.persistentDataPath + "/SaveInfo.json";
            if(File.Exists(SaveInfo)) {
                File.Delete(SaveInfo);
            }    
            FasterRepliesSlider.Play("Turn-Off-Slider");
            MuteSlider.Play("Turn-Off-Slider");
            SM.RefreshApps();
            ResetSaves();
            MH.GenerateContactsList();
            MH.GeneratePhotoList();
            SM.RefreshSaveList();
            MH.NewGame();
            Destroy(LoadModalWindowClone.gameObject);
        });
    }

    public void ResetSaves() {
        SF.saveFile = new SaveFileRoot();
        Saves.MostRecentSaveIndex = 0;
        Saves.NameOfSaves = new List<string>();
        Saves.ChapterOfSaves = new List<int>();
        Saves.TendencyOfSaves = new List<string>();
        Saves.DateTimeOfSave = new List<string>();
        Saves.AutoSaveMostRecent = false;
        Saves.AutoSaveChapter = 0;
        Saves.AutoSaveTendency = "";
        Saves.AutoSaveDateTime = "";
    }

    public void DestroyAllList() {
        foreach (Transform child in SettingsList) {
            Destroy(child.gameObject);
        }
    }
}
