using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

    public SaveFile SF;
    public DBHandler DB;
    public PreFabs preFabs;
    public Transform SettingsList;


    public void DestroySettingsList() {
        foreach(Transform child in SettingsList) {
            Destroy(child.gameObject);
        }
    }

    public void PopulateSettingsPage() {
        for (int i = 0; i < SF.saveFile.Settings.Count; i++) {
            Transform SettingsSliderCardClone = Instantiate(preFabs.SettingsSliderCard, new Vector3(0, 0, 0), Quaternion.identity, SettingsList);
            Sprite Image = Resources.Load("Images/Settings/" + SF.saveFile.Settings[i].SettingName, typeof(Sprite)) as Sprite;
            SettingsSliderCardClone.GetChild(0).GetComponent<Image>().sprite = Image;
            SettingsSliderCardClone.GetChild(1).GetComponent<TextMeshProUGUI>().text = SF.saveFile.Settings[i].SettingName;
            GameObject Slider = SettingsSliderCardClone.GetChild(2).gameObject;
            if (SF.saveFile.Settings[i].Enabled){
                Slider.GetComponent<Animator>().Play("Turn-On-Slider");
            }
            int indx = i;
            Slider.GetComponent<Button>().onClick.AddListener(() => {
                if (!SF.saveFile.Settings[indx].Enabled) {
                    SF.saveFile.Settings[indx].Enabled = true;
                    Slider.GetComponent<Animator>().Play("Turn-On-Slider");
                } else {
                    SF.saveFile.Settings[indx].Enabled = false;
                    Slider.GetComponent<Animator>().Play("Turn-Off-Slider");
                }
            });
        }
    }

    public void GenerateSettingsList() {
        if (DB.DataBase.SettingsList.Count != SF.saveFile.Settings.Count) {
            for (int i = SF.saveFile.Settings.Count; i < DB.DataBase.SettingsList.Count; i++) {
                SF.saveFile.Settings.Add(
                    new Setting{ 
                        SettingName = DB.DataBase.SettingsList[i],
                        Enabled = false 
                    }
                );
            }
        }
    }
}
