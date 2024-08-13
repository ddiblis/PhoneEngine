using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class SharedObjects : MonoBehaviour {

    // ======================= Home Screen ============================

    public GameObject Wallpaper;

    // ================================================================

    // ====================== Contacts App ============================

    public Transform cardsList;

    // ======================= Texting App ============================

    public Transform textingApp;
    public GameObject headshot;
    public RectTransform displayedList;
    public Transform scrollView;
    public Transform content;

    // Response choices box
    public Transform choices;

    // ================================================================

    // ======================= Gallary App ============================

    public Transform ImageList;

    // ================================================================

    // =================== Image Modal Window =========================

    public Transform ImageModalWindow;

    // ================================================================

    // ======================= Notif Area =============================

    public Transform notificationArea;

    // generated notification. Nullable in case it doesn't need to be generated
    #nullable enable
    public GameObject? notif;
    #nullable disable

    // ================================================================

    // ========================= Save App =============================



    // ================================================================

}
