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

    public Transform contactsApp;
    public GameObject contactButton;
    public Transform cardsList;
    public GameObject scrollBarVerticle;
    public int selectedIndex;

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
            selectedIndex = indx;
            MH.headshot.GetComponent<Image>().sprite = pfp;
            
            MH.displayedList = MH.content.GetChild(indx) as RectTransform;
            MH.scrollView.GetComponent<ScrollRect>().content = MH.displayedList;
            gen.Show(MH.displayedList.transform);
            MH.displayedList.anchoredPosition = Vector3.zero;   

            if (MH.contactPush != indx){
                gen.Hide(MH.choices); 
            } else {
                gen.Show(MH.choices);
            }

            gen.Show(MH.textingApp);
        

        });
    }

    void Start()
    {
        gen.Hide(contactsApp);

        

        for (int i = 0; i < contactsList.Count; i++) {
            Sprite img = Resources.Load(contactsList[i], typeof(Sprite)) as Sprite;            

            addContactCard(img, contactsList[i], i);
        } 
        

    }
}
