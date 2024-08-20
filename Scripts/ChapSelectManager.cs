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
using System.Linq;
using System;


public class ChapSelectManager : MonoBehaviour
{
    public Transform ChaptersList;
    public Transform Canvas;
    public SaveFile SF;
    public SaveManager SM;
    public MessagingHandlers MH;
    public PreFabs preFabs;
    public SharedObjects Shared;


    public void openApp() {
        for (int i = 0; i < SF.saveFile.ChapterList.Count; i++) {
            Transform ChapterCardClone = Instantiate(preFabs.ChapterCard, new Vector3(0, 0, 0), Quaternion.identity, ChaptersList);
            Sprite img = Resources.Load("Images/ChapImages/" + SF.saveFile.ChapterList[i], typeof(Sprite)) as Sprite;
            ChapterCardClone.GetChild(0).GetChild(0).GetComponent<Image>().sprite = img;
            ChapterCardClone.GetChild(2).GetComponent<TextMeshProUGUI>().text = SF.saveFile.ChapterList[i];
            int indx = i;
            ChapterCardClone.GetComponent<Button>().onClick.AddListener(() => {
                if(!SF.saveFile.Settings.MuteGame) {
                    Shared.Wallpaper.GetComponent<AudioSource>().Play();
                }
                openChapModal(indx);
            });
        }
    }

        void openChapModal(int indx) {
        Transform LoadModalWindowClone = 
            Instantiate(preFabs.LoadModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);

        LoadModalWindowClone.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Loading chapter will not lose photos, but will reset choices";
        LoadModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");
        Button confirmButton = LoadModalWindowClone.GetChild(3).GetComponent<Button>();
        Button cancelButton = LoadModalWindowClone.GetChild(2).GetComponent<Button>();
        cancelButton.onClick.AddListener(() => {
            if(!SF.saveFile.Settings.MuteGame) {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
            }
            Destroy(LoadModalWindowClone.gameObject);
        });

        confirmButton.onClick.AddListener(() => {
            if(!SF.saveFile.Settings.MuteGame) {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
            }
            SM.RefreshApps();
            FixSaveFileForChap(indx);
            MH.ChapterSelect(SF.saveFile.ChapterList[indx]);
            Destroy(LoadModalWindowClone.gameObject);
        });
    }

    void FixSaveFileForChap(int indx) {
        foreach(Contact c in SF.saveFile.ContactsList){
            c.Unlocked = false;
        }
        SF.saveFile.CurrStoryPoint.ChapIndex = indx;
        SF.saveFile.CurrStoryPoint.SubChapIndex = 0;
        SF.saveFile.CurrStoryPoint.CurrTextIndex = 0;
        foreach(SavedPost p in SF.saveFile.Posts){
            p.Unlocked = false;
        }
        SF.saveFile.ChoiceNeeded = false;
    }

    public void DestroyAllList() {
        foreach (Transform child in ChaptersList) {
            Destroy(child.gameObject);
        }
    }
    
}
