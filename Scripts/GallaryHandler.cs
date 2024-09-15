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
    public GameObject SidePanelCard;
    public Transform AllPhotosCard;
    public Transform BackgroundsCard;
    public Transform SidePanel;
    public Transform CategoryList;
    public GeneralHandlers gen;
    public SaveFile SF;

    public void OpenSidePanel() {
        int AllPossible = 0;
        int AllUnlocked = 0;

        for (int i = 0; i < SF.saveFile.PhotoCategories.Count; i++) {
            AllPossible += SF.saveFile.PhotoCategories[i].NumberAvaliable;
            AllUnlocked += SF.saveFile.PhotoCategories[i].NumberSeen;
            string Cat = SF.saveFile.PhotoCategories[i].Category;
            int indx = i;
            Sprite Image;
            if(Cat != "bg") {
                Image = Resources.Load("Images/Headshots/" + SF.saveFile.PhotoCategories[i].Category, typeof(Sprite)) as Sprite;
            } else {
                Image = Resources.Load("Images/Emojis/Photo" , typeof(Sprite)) as Sprite;
            }
            Transform ContactCard = Instantiate(SidePanelCard, new Vector3(0, 0, 0), Quaternion.identity, CategoryList.transform).transform;
            ContactCard.GetComponent<Button>().onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                DestroyGallary();
                DisplayCategoryImages(Cat);
            });
            ContactCard.GetChild(0).GetComponent<Image>().sprite = Image;
            ContactCard.GetChild(1).GetComponent<TextMeshProUGUI>().text = SF.saveFile.PhotoCategories[i].Category;
            ContactCard.GetChild(2).GetComponent<TextMeshProUGUI>().text = 
                SF.saveFile.PhotoCategories[i].NumberSeen + " / " + SF.saveFile.PhotoCategories[i].NumberAvaliable;
        }    
        AllPhotosCard.GetChild(2).GetComponent<TextMeshProUGUI>().text = AllUnlocked + " / " + AllPossible;
    }

     public void DisplayCategoryImages(string Category) {
        for (int i = SF.saveFile.Photos.Count-1; i > -1; i--) {
            if (Category == SF.saveFile.Photos[i].Category){
                GenerateImage(i);
            }
        }
    }

    public void GenerateImage(int indx) {
        // if(SF.saveFile.Photos[indx].Seen){
        Sprite photo = Resources.Load("Images/Photos/" + SF.saveFile.Photos[indx].ImageName, typeof(Sprite)) as Sprite;
        GameObject PhotoContainerClone = Instantiate(PhotoContainer, new Vector3(0, 0, 0), Quaternion.identity, Shared.ImageList);
        GameObject imageContent = PhotoContainerClone.transform.GetChild(0).gameObject;
        imageContent.GetComponent<Image>().sprite = photo;
        Button button = PhotoContainerClone.GetComponent<Button>();

        button.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            gen.ModalWindowOpen(photo, SF.saveFile.Photos[indx].ImageName);
        });
        // }
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
    public void DestroySidePanelCards() {
        for (int i = 1; i < CategoryList.childCount; i++){
			Destroy(CategoryList.GetChild(i).gameObject);
		}
    }
}
