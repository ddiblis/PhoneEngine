using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralHandlers : MonoBehaviour {

    public SharedObjects Shared;

    public void Hide(Transform Object){
        Object.localScale = new Vector3(0, 0, 0);
    }
    public void Show(Transform Object){
        Object.localScale = new Vector3(1, 1, 1);
    }

    public void QuitGame() {
            Application.Quit();
    }

    public void ModalWindowOpen(Button button, Sprite photo) {
        button.onClick.AddListener(() => {
            Shared.ModalWindow.GetChild(0).GetComponent<Image>().sprite = photo;
            Shared.ModalWindow.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<Image>().sprite = photo;
            });
            Show(Shared.ModalWindow);
        });
    }

}
