using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHandlers : MonoBehaviour
{
    public void Hide(Transform Object){
        Object.localScale = new Vector3(0, 0, 0);
    }
    public void Show(Transform Object){
        Object.localScale = new Vector3(1, 1, 1);
    }

}
