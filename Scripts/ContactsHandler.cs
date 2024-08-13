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
    public SavedItems saved;
    public PreFabs Prefabs;

    public void setContactPage(Sprite pfp, int indx) {
        Shared.headshot.GetComponent<Image>().sprite = pfp;
            
        Shared.displayedList = Shared.content.GetChild(indx) as RectTransform;
        Shared.scrollView.GetComponent<ScrollRect>().content = Shared.displayedList;
        gen.Show(Shared.displayedList.transform);
        Shared.displayedList.anchoredPosition = Vector3.zero;   
    }

    public void addContactCard(Sprite pfp, string name, int indx) {
        GameObject ChoiceClone = Instantiate(Prefabs.contactButton, new Vector3(0, 0, 0), Quaternion.identity, Shared.cardsList);
        ChoiceClone.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        ChoiceClone.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = name;
        ChoiceClone.GetComponent<Button>().onClick.AddListener(() => {

            saved.selectedIndex = indx;  // sets index to be used for showing or hiding choices in messagingHandler while viewing contact

            bool viewingScreen = saved.contactPush == saved.selectedIndex;

            setContactPage(pfp, indx);

            // This is important for showing or hiding contact choices if you're not viewing a contact
            if (saved.contactPush != indx) {
                gen.Hide(Shared.choices); 
            } else {
                gen.Show(Shared.choices);
            }

            gen.Hide(Shared.cardsList.GetChild(indx).GetChild(2).transform);

            // It bugs me if the notif is still on after I open the contact, this removes it when you do if it's the right contact
            if (viewingScreen) {
                Destroy(Shared.notif);
            }


            Shared.textingApp.GetComponent<Animator>().Play("Open-Texts-App");
        });
        gen.Hide(Shared.cardsList.GetChild(indx));
    }

    public void UnlockContactCard() {
        for (int i = 0; i < saved.ContactsList.Count; i++) {
            if (saved.UnlockedContacts[i]) {
                gen.Show(Shared.cardsList.GetChild(i));
            }
        }
    }
}
