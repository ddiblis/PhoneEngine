using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedObjects : MonoBehaviour {

    public List<string> contactsList = new List<string>() {
        "gf", "blonde"
    };

    public GameObject headshot;
    public Transform textingApp;
    public RectTransform displayedList;

    // Response choices box
    public Transform choices;
    public Transform content;
    public Transform scrollView;

    // The int of which message list to send the messages to based on their name and their position in the contactsList
    public int contactPush;
    
    // The int of which messageList you're currently in.
    public int selectedIndex;


}
