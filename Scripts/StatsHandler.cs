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
using System.Diagnostics;

public class StatsHandler : MonoBehaviour {
    public SaveFile SF;
    public DBHandler DB;
    public Transform StatList;
    public Transform TimerCard;
    public Transform ImagesUnlockedCard;
    public Transform ChapterImagesUnlockedCard;
    public Transform StatCard;
    public int minutes;
    public int hours;
    public InstaPostsManager IP;

    void Start() {
        StartCoroutine("PlayTimer");
    }
    private IEnumerator PlayTimer() {
        while (true) {
            yield return new WaitForSeconds(1);
            SF.saveFile.playTime += 1;
            minutes = (SF.saveFile.playTime / 60) % 60;
            hours = (SF.saveFile.playTime / 3600) % 60;
        }
    }

    public void OpenApp() {
        Transform TimerCardClone = Instantiate(TimerCard, Vector3.zero, Quaternion.identity, StatList);
        TimerCardClone.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{hours}H:{minutes}M";

        Transform ImagesUnlockedCardClone = Instantiate(ImagesUnlockedCard, Vector3.zero, Quaternion.identity, StatList);
        int TotalAvaliable = DB.DataBase.PhotoList.Count;
        int TotalObtained = 0;
        foreach(string photo in DB.DataBase.PhotoList) {
            if (SF.saveFile.Photos.FindIndex(x => x.ImageName == photo) > -1) {
                TotalObtained += 1;
            }
        }
        ImagesUnlockedCard.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Total Images: {TotalObtained}/{TotalAvaliable}";

        int TotalFromAllChapters = 0;
        int TotalObtainedFromChapters = 0;
        for (int i = 0; i < DB.DataBase.ChapterImages.Count; i ++) {
            int TotalAvaliableInChapter = DB.DataBase.ChapterImages[i].ImagesList.Count;
            TotalFromAllChapters += TotalAvaliableInChapter;
            int TotalObtainedInChapter = 0;
            Transform ChapterImagesCard = Instantiate(ChapterImagesUnlockedCard, Vector3.zero, Quaternion.identity, StatList);
            foreach (string Image in DB.DataBase.ChapterImages[i].ImagesList) {
                if (SF.saveFile.Photos.FindIndex(x => x.ImageName == Image) > -1) { 
                    TotalObtainedInChapter += 1;
                    TotalObtainedFromChapters += 1;
                }
            }
            ChapterImagesCard.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Chapter {i+1} Images: {TotalObtainedInChapter} / {TotalAvaliableInChapter}";
        }

        Transform MidrollImagesCard = Instantiate(ChapterImagesUnlockedCard, Vector3.zero, Quaternion.identity, StatList);
        int TotalNumberOfMidRollImages = TotalAvaliable - TotalFromAllChapters - 4 - IP.postsList.Posts.Count;
        int NumOfMidrollImagesObtained = SF.saveFile.Photos.Count - TotalObtainedFromChapters - 4;
        MidrollImagesCard.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Midrolls Images: {NumOfMidrollImagesObtained} / {TotalNumberOfMidRollImages}";

        if (SF.saveFile.CurrStoryPoint.ChapIndex > 0) {
            Transform StatCardClone = Instantiate(StatCard, Vector3.zero, Quaternion.identity, StatList);
            StatCardClone.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"You chose the {(Tendency)SF.saveFile.Stats.Tendency} Route";
        }
    }

    public void CloseApp() {
        foreach (Transform card in StatList) {
            Destroy(card.gameObject);
        }
    }
}