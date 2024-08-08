using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHandlers : MonoBehaviour
{
    public void HideScreen(GameObject Screen){
        Screen.transform.localScale = new Vector3(0, 0, 0);
    }
    public void foregroundScreen(GameObject Screen){
        Screen.transform.localScale = new Vector3(1, 1, 1);
    }
}
