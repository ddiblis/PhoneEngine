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
            {"gf", "Blondie"};


    public GameObject contactButton;
    public Transform cardsList;


    public void addContactCard(Sprite pfp, string name){
        GameObject ChoiceClone = Instantiate(contactButton, new Vector3(0, 0, 0), Quaternion.identity, cardsList.transform);
        GameObject picField = ChoiceClone.transform.GetChild(0).gameObject;
        GameObject nameField = ChoiceClone.transform.GetChild(1).GetChild(0).gameObject;
        picField.GetComponent<Image>().sprite = pfp;
        nameField.GetComponent<TextMeshProUGUI>().text = name;
    }

    // Start is called before the first frame update
    void Start()
    {
        Emojis emojis = new Emojis();

        // Debug.Log(emojis.blackHeart);

        //        List<Sprite> pfpList = new List<Sprite>
        //         {emojis.gfHeadshot, emojis.blondeHeadshot};

        // for (int i = 0; i < contactsList.Count; i++) {
        //     addContactCard(pfpList[i], contactsList[i]);
        // } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
