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
    public Transform GallaryApp;
    public GeneralHandlers gen;
    public SavedItems saved;

    public void DisplayImages() {
        for (int i = 0; i < saved.seenImages.Count; i++) {
            Sprite photo = Resources.Load("Images/Photos/" + saved.seenImages[i], typeof(Sprite)) as Sprite;
            GameObject PhotoContainerClone = Instantiate(PhotoContainer, new Vector3(0, 0, 0), Quaternion.identity, Shared.ImageList);
            GameObject imageContent = PhotoContainerClone.transform.GetChild(0).gameObject;
            imageContent.GetComponent<Image>().sprite = photo;
            Button button = PhotoContainerClone.GetComponent<Button>();
            int indx = i;
            button.onClick.AddListener(() => {
                gen.ModalWindowOpen(photo, saved.seenImages[indx]);
            });
        }
    }

    public void DestroyGallary() {
        foreach (Transform child in Shared.ImageList) {
			Destroy(child.gameObject);
		}
    }
}
