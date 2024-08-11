using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedObjects : MonoBehaviour {

    // ======================= Home Screen ============================

    public GameObject Wallpaper;

    // ================================================================

    // ====================== Contacts App ============================

    public List<string> ContactsList;
    public Transform cardsList;

    // ======================= Texting App ============================
    public List<string> ChapterList;

    // The int of which message list to send the messages to based on their name and their position in the contactsList
    public int contactPush;

    // The index of which messageList you're currently viewing, derived from the contact card you click.
    public int selectedIndex;
    public Transform textingApp;
    public GameObject headshot;
    public RectTransform displayedList;
    public Transform scrollView;

    // Response choices box
    public Transform choices;
    public Transform content;

    // ================================================================

    // ======================= Gallary App ============================

    public List<string> seenImages;
    public Transform ImageList;

    // ================================================================

    // ====================== Modal Window ============================

    public Transform ModalWindow;

    // ================================================================

    // ======================= Notif Area =============================

    public Transform notificationArea;

    // generated notification. Nullable in case it doesn't need to be generated
    #nullable enable
    public GameObject? notif;
    #nullable disable

    // ================================================================




}
