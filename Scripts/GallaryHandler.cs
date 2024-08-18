using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.IO;

public class GallaryHandler : MonoBehaviour
{
    public SharedObjects Shared;
    public GameObject PhotoContainer;
    public Transform SidePanel;
    public GeneralHandlers gen;
    public SaveFile SF;

    public void OpenSidePanel() {
        // string[] FileList = Directory.GetFiles(Application.streamingAssetsPath + "/Images/Photos","*.NA");
        // ImgName.Split('-')[0]
    }
    public void GenerateImage(int indx) {
        if(SF.saveFile.Photos[indx].Seen){
            Sprite photo = Resources.Load("Images/Photos/" + SF.saveFile.Photos[indx].ImageName, typeof(Sprite)) as Sprite;
            GameObject PhotoContainerClone = Instantiate(PhotoContainer, new Vector3(0, 0, 0), Quaternion.identity, Shared.ImageList);
            GameObject imageContent = PhotoContainerClone.transform.GetChild(0).gameObject;
            imageContent.GetComponent<Image>().sprite = photo;
            Button button = PhotoContainerClone.GetComponent<Button>();

            button.onClick.AddListener(() => {
                gen.ModalWindowOpen(photo, SF.saveFile.Photos[indx].ImageName);
            });
        }
    }

    public void DisplayImages() {
        for (int i = SF.saveFile.Photos.Count-1; i > -1; i--) {
            GenerateImage(i);
        }
    }

    public void DestroyGallary() {
        foreach (Transform child in Shared.ImageList) {
			Destroy(child.gameObject);
		}
    }
}
