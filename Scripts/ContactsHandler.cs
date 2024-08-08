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

    List<string> contactsList = new List<string>
            {"gf", "Blonde"};

    List<Transform> MessageLists = new List<Transform>();

    public GameObject textingApp;
    public GameObject contactsApp;
    public Transform messageList;
    public GameObject contactButton;
    public Transform cardsList;
    public GameObject Pfp;


    public void addContactCard(Sprite pfp, string name){
        GameObject ChoiceClone = Instantiate(contactButton, new Vector3(0, 0, 0), Quaternion.identity, cardsList.transform);
        GameObject picField = ChoiceClone.transform.GetChild(0).gameObject;
        GameObject nameField = ChoiceClone.transform.GetChild(1).GetChild(0).gameObject;
        picField.GetComponent<Image>().sprite = pfp;
        nameField.GetComponent<TextMeshProUGUI>().text = name;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            textingApp.transform.localScale = new Vector3(1, 1, 1);
            Pfp.GetComponent<Image>().sprite = pfp;
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        // hides screen
        contactsApp.transform.localScale = new Vector3(0, 0, 0);

        for (int i = 0; i < contactsList.Count; i++) {
            Sprite img = Resources.Load(contactsList[i], typeof(Sprite)) as Sprite;
            MessageLists.Add(Instantiate(messageList, new Vector3(0, 0, 0), Quaternion.identity));

            addContactCard(img, contactsList[i]);
        } 

    }
}
