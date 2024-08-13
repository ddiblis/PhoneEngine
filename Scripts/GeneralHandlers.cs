using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class GeneralHandlers : MonoBehaviour {

    public SharedObjects Shared;
    public SavedItems saved;

    public void Hide(Transform Object){
        Object.localScale = new Vector3(0, 0, 0);
    }
    public void Show(Transform Object){
        Object.localScale = new Vector3(1, 1, 1);
    }

    public void QuitGame() {
            Application.Quit();
    }

    public void SetWallPaper(string imgName) {
        Sprite LoadedWallpaper = Resources.Load("Images/Photos/" + imgName, typeof(Sprite)) as Sprite;
        Shared.Wallpaper.GetComponent<Image>().sprite = LoadedWallpaper;
    }

    // figure out why you're getting the pictures in 2 different formats, with and without braces, and fix line 34 object Object reference not set to an instance of an object

    public void ModalWindowOpen(Button button, Sprite photo, string ImageName) {
        Debug.Log(ImageName);
        button.onClick.AddListener(() => {
            Shared.ImageModalWindow.GetChild(0).GetComponent<Image>().sprite = photo;
            Shared.ImageModalWindow.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<Image>().sprite = photo;
                saved.currWallPaper = ImageName;
            });
            Shared.ImageModalWindow.GetComponent<Animator>().Play("Open-Image-Modal");
        });
    }

}
