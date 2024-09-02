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
    public Transform StatList;
    public Transform TimerCard;
    public Transform StatCard;
    public int minutes;
    public int hours;
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
        Transform TimerCardClone = Instantiate(TimerCard, new Vector3(0, 0, 0), Quaternion.identity, StatList);
        TimerCardClone.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{hours}H:{minutes}M";
        Transform StatCardClone = Instantiate(StatCard, new Vector3(0, 0, 0), Quaternion.identity, StatList);
        StatCardClone.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"You chose the {(Tendency) SF.saveFile.Stats.Tendency} Route";
    }

    public void CloseApp() {
        foreach (Transform card in StatList) {
            Destroy(card.gameObject);
        }
    }
    public void PlayTimeHandler() {

    }
}