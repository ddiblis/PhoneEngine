using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class GeneralHandlers : MonoBehaviour {

    public SharedObjects Shared;
    public PreFabs preFabs;
    public SaveFile SF;
    public Transform Canvas;

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

    public void ModalWindowOpen(Sprite photo, string ImageName) {
        Transform ImageModalWindowClone =
            Instantiate(preFabs.ImageModalWindow, new Vector3(0, 0, 0), Quaternion.identity, Canvas);

        ImageModalWindowClone.GetChild(0).GetComponent<Image>().sprite = photo;
        Button setWallpaperButton = ImageModalWindowClone.GetChild(1).GetComponent<Button>();
        Button xButton = ImageModalWindowClone.GetChild(2).GetComponent<Button>();

        xButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            Destroy(ImageModalWindowClone.gameObject);
        });
        setWallpaperButton.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            Destroy(ImageModalWindowClone.gameObject);
            SF.saveFile.CurrWallPaper = ImageName;
            SetWallPaper(SF.saveFile.CurrWallPaper);
        });
        
        ImageModalWindowClone.GetComponent<Animator>().Play("Open-Image-Modal");
    }

    public void PlayclickSound() {
        Shared.Wallpaper.AddComponent<AudioSource>().Play();
    }

}
