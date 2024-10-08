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

    public void SetContactPage(Sprite pfp, int indx) {
        Shared.headshot.GetComponent<Image>().sprite = pfp;
            
        Shared.displayedList = Shared.content.GetChild(indx) as RectTransform;
        Shared.scrollView.GetComponent<ScrollRect>().content = Shared.displayedList;
        gen.Show(Shared.displayedList.transform);
        Shared.displayedList.anchoredPosition = Vector3.zero;
    }

    public void AddContactCard(Sprite pfp, Contact Contact, int indx) {
        GameObject ChoiceClone = Instantiate(Prefabs.contactButton, Vector3.zero, Quaternion.identity, Shared.cardsList);
        ChoiceClone.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        ChoiceClone.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = Contact.NameOfContact;
        if(Contact.NewTexts) {
            gen.Show(ChoiceClone.transform.GetChild(2).transform);
        }

        ChoiceClone.GetComponent<Button>().onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();

            SF.saveFile.selectedIndex = indx;  // sets index to be used for showing or hiding choices in messagingHandler while viewing contact

            bool viewingScreen = SF.saveFile.contactPush == SF.saveFile.selectedIndex;

            SetContactPage(pfp, indx);

            // This is important for showing or hiding contact choices if you're not viewing who the choices are for
            if (SF.saveFile.contactPush != indx) {
                gen.Hide(Shared.choices);
            } else {
                gen.Show(Shared.choices);
            }
            
            // Removes the new texts indicator when opening messagesList
            SF.saveFile.ContactsList[indx].NewTexts = false;
            gen.Hide(ChoiceClone.transform.GetChild(2).transform);

            // Hides the text Indicator and resets it to 0 when you've viewed the messages you'd missed
            SF.saveFile.NumOfNewMessages = 0;
            Shared.MessagesIndicator.GetChild(0).GetComponent<TextMeshProUGUI>().text = SF.saveFile.NumOfNewMessages.ToString();
            gen.Hide(Shared.MessagesIndicator);

            // It bugs me if the notif is still on after I open the contact, this removes it when you do if it's the right contact
            if (viewingScreen) {
                Destroy(Shared.notif);
            }


            Shared.textingApp.GetComponent<Animator>().Play("Open-Texts-App");
            CloseApp();
        });
    }

    public void GenerateContactCards() {
        // Generates contact cards for each contact based on list.
        for (int i = 0; i < SF.saveFile.ContactsList.Count; i++) {
            Sprite img = Resources.Load<Sprite>($"Images/Headshots/{SF.saveFile.ContactsList[i].NameOfContact}");            
            AddContactCard(img, SF.saveFile.ContactsList[i], i);
        } 
    }

    public void CloseApp() {
        foreach (Transform child in Shared.cardsList) {
			Destroy(child.gameObject);
		}
    }
}
