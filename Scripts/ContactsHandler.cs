using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using System;


public class ContactsHandler : MonoBehaviour {

    public GeneralHandlers gen;
    public SharedObjects Shared;
    public SaveFile SF;
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

            SF.saveFile.selectedIndex = indx;  // sets index to be used for showing or hiding choices in messagingHandler while viewing contact

            bool viewingScreen = SF.saveFile.contactPush == SF.saveFile.selectedIndex;

            setContactPage(pfp, indx);

            // This is important for showing or hiding contact choices if you're not viewing who the choices are for
            if (SF.saveFile.contactPush != indx) {
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

    public void GenerateContactCards() {
        // Generates contact cards for each contact based on list.
        for (int i = 0; i < SF.saveFile.ContactsList.Count; i++) {
            Sprite img = Resources.Load("Images/Headshots/" + SF.saveFile.ContactsList[i].NameOfContact, typeof(Sprite)) as Sprite;            
            addContactCard(img, SF.saveFile.ContactsList[i].NameOfContact, i);
        } 
    }

    // This only runs to check if it's been unlocked or not on button clicks to display the ones that are unlocked
    public void UnlockContactCard() {
        for (int i = 0; i < SF.saveFile.ContactsList.Count; i++) {
            if (SF.saveFile.ContactsList[i].Unlocked) {
                gen.Show(Shared.cardsList.GetChild(i));
            } else {
                gen.Hide(Shared.cardsList.GetChild(i));
            }
        }
    }
}
