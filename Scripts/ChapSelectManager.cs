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

namespace PhoneEngine {
    public class ChapSelectManager : MonoBehaviour
    {
        public Transform ChaptersList;
        public Transform Canvas;
        public SaveFile SF;
        public SaveManager SM;
        public MessagingHandlers MH;
        public PreFabs preFabs;
        public SharedObjects Shared;
        public GeneralHandlers gen;
        public DBHandler DB;

        public void GenerateChapterList(int i) {
            Transform ChapterCardClone = Instantiate(preFabs.ChapterCard, Vector3.zero, Quaternion.identity, ChaptersList);
            Sprite img = Resources.Load<Sprite>($"Images/ChapImages/{SF.saveFile.ChapterList[i].ChapterName}");
            ChapterCardClone.GetChild(0).GetChild(0).GetComponent<Image>().sprite = img;
            ChapterCardClone.GetChild(2).GetComponent<TextMeshProUGUI>().text = SF.saveFile.ChapterList[i].ChapterName;
            int indx = i;
            ChapterCardClone.GetComponent<Button>().onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                OpenChapModal(indx);
            });
        }

        public void OpenApp() {
            for (int i = 0; i < SF.saveFile.ChapterList.Count; i++) {
                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                    GenerateChapterList(i);
                #endif
                #if !DEVELOPMENT_BUILD
                if (SF.saveFile.ChapterList[i].seen) {
                    GenerateChapterList(i);
                }
                #endif
            }
        }

            void OpenChapModal(int indx) {
            Transform LoadModalWindowClone = 
                Instantiate(preFabs.LoadModalWindow, Vector3.zero, Quaternion.identity, Canvas);

            LoadModalWindowClone.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Loading chapter will not lose photos, but will reset choices";
            LoadModalWindowClone.GetComponent<Animator>().Play("Open-Save-Modal");
            Button confirmButton = LoadModalWindowClone.GetChild(3).GetComponent<Button>();
            Button cancelButton = LoadModalWindowClone.GetChild(2).GetComponent<Button>();
            cancelButton.onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                Destroy(LoadModalWindowClone.gameObject);
            });

            confirmButton.onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                SM.RefreshApps();
                FixSaveFileForChap(indx);
                DB.UnlockInstaPostsForChapter(indx);
                MH.ChapterSelect(ChapterType.Chapter, SF.saveFile.ChapterList[indx].ChapterName);
                Destroy(LoadModalWindowClone.gameObject);
            });
        }

        void FixSaveFileForChap(int indx) {
            SF.saveFile.ContactsList = new();
            SF.saveFile.CurrStoryPoint.ChapIndex = indx;
            SF.saveFile.CurrStoryPoint.SubChapIndex = 0;
            SF.saveFile.CurrStoryPoint.CurrTextIndex = 0;
            SF.saveFile.NumOfNewMessages = 0;
            SF.saveFile.NumOfNewPosts = 0;
            gen.Hide(Shared.InstaPostsIndicator);
            gen.Hide(Shared.MessagesIndicator);
            foreach(SavedPost p in SF.saveFile.Posts){
                p.Unlocked = false;
            }
            foreach (InstaAccount ia in SF.saveFile.InstaAccounts) {
                ia.NumberOfPosts = 0;
            }
            SF.saveFile.ChoiceNeeded = false;
        }

        public void DestroyAllList() {
            foreach (Transform child in ChaptersList) {
                Destroy(child.gameObject);
            }
        }
        
    }
}
