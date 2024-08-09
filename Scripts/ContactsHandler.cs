using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using System;

public class ContactsHandler : MonoBehaviour {

    public Transform contactsApp;
    public Transform cardsList;


    public GeneralHandlers gen;
    public SharedObjects Shared;
    public PreFabs Prefabs;



    public void addContactCard(Sprite pfp, string name, int indx) {
        GameObject ChoiceClone = Instantiate(Prefabs.contactButton, new Vector3(0, 0, 0), Quaternion.identity, cardsList.transform);
        ChoiceClone.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        ChoiceClone.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = name;
        ChoiceClone.GetComponent<Button>().onClick.AddListener(() => {

            Shared.selectedIndex = indx;  // sets index to be used for showing or hiding choices in messagingHandler while viewing contact
            Shared.headshot.GetComponent<Image>().sprite = pfp;
            
            Shared.displayedList = Shared.content.GetChild(indx) as RectTransform;
            Shared.scrollView.GetComponent<ScrollRect>().content = Shared.displayedList;
            gen.Show(Shared.displayedList.transform);
            Shared.displayedList.anchoredPosition = Vector3.zero;   

            // This is important for showing or hiding contact choices if you're not viewing a contact
            if (Shared.contactPush != indx){
                gen.Hide(Shared.choices); 
            } else {
                gen.Show(Shared.choices);
            }

            gen.Show(Shared.textingApp);
        

        });
    }

    void Start() {
        
        gen.Hide(contactsApp);
        
        // Generates contact cards for each contact based on list.
        for (int i = 0; i < Shared.contactsList.Count; i++) {
            Sprite img = Resources.Load(Shared.contactsList[i], typeof(Sprite)) as Sprite;            

            addContactCard(img, Shared.contactsList[i], i);
        } 
        

    }
}
