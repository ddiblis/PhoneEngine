using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using System;

public class ContactsHandler : MonoBehaviour
{

    public GameObject contactsApp;
    public GameObject contactButton;
    public Transform cardsList;

    public GeneralHandlers gen;
    public MessagingHandlers MH;

    List<string> contactsList = new List<string>() {
        "gf", "blonde"
    };


    public void addContactCard(Sprite pfp, string name, int indx){
        GameObject ChoiceClone = Instantiate(contactButton, new Vector3(0, 0, 0), Quaternion.identity, cardsList.transform);
        GameObject picField = ChoiceClone.transform.GetChild(0).gameObject;
        GameObject nameField = ChoiceClone.transform.GetChild(1).GetChild(0).gameObject;
        picField.GetComponent<Image>().sprite = pfp;
        nameField.GetComponent<TextMeshProUGUI>().text = name;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MH.headshot.GetComponent<Image>().sprite = pfp;

            MH.msgList = MH.content.GetChild(indx) as RectTransform;
            MH.scrollView.GetComponent<ScrollRect>().content = MH.msgList;
            MH.content.GetChild(indx).localScale = new Vector3(1, 1, 1);

            gen.foregroundScreen(MH.textingApp);
        });
    }

    void Start()
    {
        gen.HideScreen(contactsApp);

        

        for (int i = 0; i < contactsList.Count; i++) {
            Debug.Log(contactsList[i]);
            Sprite img = Resources.Load(contactsList[i], typeof(Sprite)) as Sprite;            

            addContactCard(img, contactsList[i], i);
        } 
        

    }
}
