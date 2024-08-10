using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using System;


public class ContactsHandler : MonoBehaviour {

    public Transform contactsApp;

    public GeneralHandlers gen;
    public SharedObjects Shared;
    public PreFabs Prefabs;

    public void setContactPage(Sprite pfp, int indx) {
        Shared.headshot.GetComponent<Image>().sprite = pfp;
            
        Shared.displayedList = Shared.content.GetChild(indx) as RectTransform;
        Shared.scrollView.GetComponent<ScrollRect>().content = Shared.displayedList;
        gen.Show(Shared.displayedList.transform);
        Shared.displayedList.anchoredPosition = Vector3.zero;   
    }

    public void addContactCard(Sprite pfp, string name, int indx) {
        GameObject ChoiceClone = Instantiate(Prefabs.contactButton, new Vector3(0, 0, 0), Quaternion.identity, Shared.cardsList.transform);
        ChoiceClone.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        ChoiceClone.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = name;
        ChoiceClone.GetComponent<Button>().onClick.AddListener(() => {

            Shared.selectedIndex = indx;  // sets index to be used for showing or hiding choices in messagingHandler while viewing contact

            bool viewingScreen = Shared.contactPush == Shared.selectedIndex;

            setContactPage(pfp, indx);

            // This is important for showing or hiding contact choices if you're not viewing a contact
            if (Shared.contactPush != indx) {
                gen.Hide(Shared.choices); 
            } else {
                gen.Show(Shared.choices);
            }

            gen.Hide(Shared.cardsList.GetChild(indx).GetChild(2).transform);

            // It bugs me if the notif is still on after I open the contact, this removes it when you do if it's the right contact
            if (viewingScreen) {
                Destroy(Shared.notif);
            }

            gen.Show(Shared.textingApp);
        });
    }

    void Start() {
        
        
        gen.Hide(contactsApp);
        
        // Generates contact cards for each contact based on list.
        for (int i = 0; i < Shared.ContactsList.Count; i++) {
            int NumOfPerson = i+1;
            Sprite img = Resources.Load("Images/Headshots/" + NumOfPerson + Shared.ContactsList[i], typeof(Sprite)) as Sprite;            

            addContactCard(img, Shared.ContactsList[i], i);
        } 
        

    }
}
