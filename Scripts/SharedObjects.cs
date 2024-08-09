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
    public Transform notificationArea;
    public Transform cardsList;

    // Response choices box
    public Transform choices;
    public Transform content;
    public Transform scrollView;

    // The int of which message list to send the messages to based on their name and their position in the contactsList
    public int contactPush;

    // The index of which messageList you're currently viewing, derived from the contact card you click.
    public int selectedIndex;

    // generated notification. Nullable in case it doesn't need to be generated
    #nullable enable
    public GameObject? notif;
    #nullable disable

}
