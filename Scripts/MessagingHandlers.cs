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
using System.Linq;
using System;


// Simple enum used for determining the type of text being sent/recieved.
public enum TypeOfText {
        sentText = 0,
        recText = 1,
        sentImage = 2,
        recImage = 3,
        sentEmoji = 4,
        recEmoji = 5,
        chapEnd = 6,
        indicateTime = 8,
    }

public class MessagingHandlers : MonoBehaviour {

    public GameObject backButton;    
    public ChapImport chap;
    public GeneralHandlers gen;
    public SharedObjects Shared;
    public PreFabs Prefabs;
    public SaveManager SM;
    public SaveFile SF;
    public DBHandler DB;
    public InstaPostsManager IPM;



    Chapter CurrChap;
    
    public void BackButton() {
        // Creates onclick handler for back button.
        Button button = backButton.GetComponent<Button>();
        button.onClick.AddListener(() => {
            gen.Hide(Shared.textingApp);
            gen.Hide(Shared.displayedList);
            SF.saveFile.selectedIndex = int.MinValue;
        });
    }

    // creates as many messageLists as needed for the contacts and hides them.
    public void GenerateMessageLists() {
        for (int i = 0; i < SF.saveFile.ContactsList.Count; i++) {
            Instantiate(Prefabs.messageList, new Vector3(0, 0, 0), Quaternion.identity, Shared.content);
            gen.Hide(Shared.content.GetChild(i));
        } 
    }

    public void SetInitialGallary() {
        SF.saveFile.Photos[0].Seen = true;
        SF.saveFile.Photos[1].Seen = true;
        SF.saveFile.Photos[2].Seen = true;
        SF.saveFile.Photos[3].Seen = true;
        SF.saveFile.PhotoCategories[0].NumberSeen += 4;
        SF.saveFile.CurrWallPaper = "bg-snowmountains";
    }

    public void NewGame() {
        SF.saveFile.selectedIndex = int.MinValue;
        DB.GeneratePhotoList();
        DB.GenerateChapterList();
        DB.GenerateMidrollsList();
        SetInitialGallary();
        gen.SetWallPaper(SF.saveFile.CurrWallPaper);
        SM.GenerateSaves(10);
        IPM.GenPostsList();
        ChapterSelect("Chapters", "chapter1");
    }

    public void ChapterSelect(string type, string Chapter, int subChapIndex = 0, int currentText = 0) {
        CurrChap = chap.GetChapter(type, Chapter);
        if (type == "Chapters") {
            SF.saveFile.AllowMidrolls = CurrChap.AllowMidrolls;
        }
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[subChapIndex], currentText));
    }
    

    // Creates and pushes the response buttons/hides them if they're not meant for currently viewed contact
    // Arguments taken from json file through StartMessagesCoroutine
    public void PopulateResps(Responses responses){
        for (int i = 0; i < responses.RespContent.Count; i++) {
            Response resp = responses.RespContent[i];
            if (resp.Type == (int)TypeOfText.sentImage){
                ImageButton(resp);
                // Sprite img = Resources.Load("Images/Photos/" + resp.TextContent, typeof(Sprite)) as Sprite;
                // ImageButton(i, resp.SubChapNum, TypeOfText.sentImage, item, img);
            } 
            else if (resp.Type == (int)TypeOfText.sentEmoji){
                ImageButton(resp);
                // Sprite img = Resources.Load("Images/Emojis/" + resp.TextContent, typeof(Sprite)) as Sprite;
                // ImageButton(i, resp.SubChapNum, TypeOfText.sentEmoji, item, img);
            }
            else {
                TextButton(resp, responses.RespTree);
                // TextButton(i, resp.SubChapNum, item, responses.RespTree);
            }
        }
        if (SF.saveFile.contactPush != SF.saveFile.selectedIndex){
            gen.Hide(Shared.choices); 
        } 
    }

    #nullable enable
    public void GenerateMessage(TypeOfText type, string textContent, string imgName, Sprite? pfp = null, Sprite? image = null) {
        bool viewingScreen = SF.saveFile.contactPush == SF.saveFile.selectedIndex;
        if (viewingScreen) {
            Shared.content.GetComponent<AudioSource>().Play();   
        }
        switch (type) {
            case TypeOfText.recText:
                pushNotification(pfp, textContent);
                MessageListLimit(TypeOfText.recText, messageContent: textContent);
            break;
            case TypeOfText.recImage:
                pushNotification(pfp, textContent);
                MessageListLimit(TypeOfText.recImage, imgName, image);
            break;
            case TypeOfText.recEmoji:
                pushNotification(pfp, textContent);
                MessageListLimit(TypeOfText.recEmoji, imgName, image);
            break;
            case TypeOfText.chapEnd:
                MessageListLimit(TypeOfText.chapEnd, messageContent: textContent);
            break;
            case TypeOfText.indicateTime:
                MessageListLimit(TypeOfText.indicateTime, messageContent: textContent);
            break;
        }
    }

    // Handles wait time for the messages recieved so they don't all display at once.
    // TypeOfText: an enum object that denotes the type of text we're sending
    // respTime: time to wait before sending the text.
    // image: optional. The image to send (emoji, photo)
    // messageContent: text of the message
    public IEnumerator MessageDelay(
        TypeOfText type,
        float respTime,
        Sprite? pfp = null,
        Sprite? image = null,
        string textContent = "Picture Message",
        string imgName = ""
    ) {
        if (SF.saveFile.Settings.FasterReplies) {
            yield return new WaitForSeconds(respTime/2);
        } else {
            yield return new WaitForSeconds(respTime);
        }
        GenerateMessage(type, textContent, imgName, pfp, image);
    }
    # nullable disable

    public IEnumerator RecieveTexts(List<TextMessage> TextList, Sprite pfp, int startingText = 0) {
        for (int i = startingText; i < TextList.Count; i++) {
            SF.saveFile.CurrStoryPoint.CurrTextIndex = i;
            TextMessage textMessage = TextList[i];
            if (textMessage.Type == (int)TypeOfText.recImage){
                int indexOfPhoto = SF.saveFile.Photos.FindIndex(x => x.ImageName == textMessage.TextContent);
                if (!SF.saveFile.Photos[indexOfPhoto].Seen == true) {
                    SF.saveFile.Photos[indexOfPhoto].Seen = true;
                    int IndexOfCategory = SF.saveFile.PhotoCategories.FindIndex(x => x.Category == textMessage.TextContent.Split("-")[0]);
                    SF.saveFile.PhotoCategories[IndexOfCategory].NumberSeen += 1;
                }
                Sprite img = Resources.Load("Images/Photos/" + textMessage.TextContent, typeof(Sprite)) as Sprite;
                yield return StartCoroutine(MessageDelay(TypeOfText.recImage, textMessage.TextDelay, pfp, img, imgName: textMessage.TextContent));
            } 
            else if (textMessage.Type == (int)TypeOfText.recEmoji){
                Sprite img = Resources.Load("Images/Emojis/" + textMessage.TextContent, typeof(Sprite)) as Sprite;
                yield return StartCoroutine(MessageDelay(TypeOfText.recEmoji, textMessage.TextDelay, pfp, img, imgName: textMessage.TextContent));
            }
            else {
                yield return StartCoroutine(MessageDelay(TypeOfText.recText, textMessage.TextDelay, pfp, textContent: textMessage.TextContent));
            }
        }
    }

    // Unlocks contact if they're not already unlocked: displays their contact card    
    public void UnlockContact(int indexOfContact) {
        if (!SF.saveFile.ContactsList[indexOfContact].Unlocked) {
            SF.saveFile.ContactsList[indexOfContact].Unlocked = true;
        }   
    }

    // Unlocks InstaPosts Profile of account
    public void UnlockInstaAccount(string InstaAccount) {
        if (InstaAccount.Length > 0) {
            SF.saveFile.InstaAccounts[SF.saveFile.InstaAccounts.FindIndex(x => x.AccountOwner == InstaAccount)].Unlocked = true;
        }
    }

    public void UnlockInstaPosts(SubChap subChap, string InstaAccount) {
        // Unlocks specified posts
        if (subChap.UnlockPosts.Count > 0) {
            for (int i = 0; i < subChap.UnlockPosts.Count; i++) {
                if(!SF.saveFile.Posts[subChap.UnlockPosts[i]].Unlocked){
                    SF.saveFile.InstaAccounts[SF.saveFile.InstaAccounts.FindIndex(x => x.AccountOwner == InstaAccount)].NumberOfPosts += 1;
                }
                SF.saveFile.Posts[subChap.UnlockPosts[i]].Unlocked = true;
            }
        }
    }

    // handles deciphering and outputting the messages from the json subchapter then calling choice buttons
    public IEnumerator StartMessagesCoroutine(SubChap subChap, int startingText = 0) {
        int indexOfContact = SF.saveFile.ContactsList.FindIndex(x => x.NameOfContact == subChap.Contact);
        string InstaAccount = subChap.UnlockInstaPostsAccount;

        // Chooses messageList parent for messages to be pushed to
        SF.saveFile.contactPush = indexOfContact;

        Sprite pfp = Resources.Load("Images/Headshots/" + subChap.Contact, typeof(Sprite)) as Sprite;

        UnlockContact(indexOfContact);

        UnlockInstaAccount(InstaAccount);

        UnlockInstaPosts(subChap, InstaAccount);

        if (!SF.saveFile.ChoiceNeeded) {
            // Sends indicator of time passed
            if (subChap.TimeIndicator.Length > 0 && SF.saveFile.CurrStoryPoint.CurrTextIndex == 0){
                yield return StartCoroutine(MessageDelay(TypeOfText.indicateTime, 1.5f, textContent: subChap.TimeIndicator));
            }
            
            if (subChap.TextList.Count == 1 || SF.saveFile.CurrStoryPoint.CurrTextIndex + 1 != subChap.TextList.Count){
                yield return StartCoroutine(RecieveTexts(subChap.TextList, pfp, startingText));
            }
        }

        if (subChap.Responses.RespContent.Count > 0){
            SF.saveFile.ChoiceNeeded = true;
            PopulateResps(subChap.Responses);
        } else {
            if (subChap.TextList.Count > 0) {
                yield return StartCoroutine(MessageDelay(TypeOfText.chapEnd, 1.0f, textContent: subChap.TextList[0].TextContent) );
            }
            SF.saveFile.CurrStoryPoint.SubChapIndex = 0;
            if (SF.saveFile.MidRollCount > 0  && SF.saveFile.AllowMidrolls) {
                SF.saveFile.PlayingMidRoll = true;
                PlayMidrolls();
            } else {
                PlayNextChapter();
            }
        }
    }

    public void PlayNextChapter() {
        SF.saveFile.CurrStoryPoint.ChapIndex += 1;
        if (SF.saveFile.CurrStoryPoint.ChapIndex <= SF.saveFile.ChapterList.Count -1) {
            ChapterSelect("Chapters", SF.saveFile.ChapterList[SF.saveFile.CurrStoryPoint.ChapIndex]);
        }
        SF.saveFile.MidRollCount = 2;
        SF.saveFile.PlayingMidRoll = false;
    }

    public void PlayMidrolls() {
        if (SF.saveFile.MidRolls.All(x => x.Seen == true)) {
            SF.saveFile.MidRollCount = 0;
            SF.saveFile.PlayingMidRoll = false;
            PlayNextChapter();
            return;
        }
        System.Random rnd = new System.Random();
        int MidRollIndex = rnd.Next(0, SF.saveFile.MidRolls.Count); 
        if (!SF.saveFile.MidRolls[MidRollIndex].Seen) {
            SF.saveFile.CurrMidRoll = MidRollIndex;
            SF.saveFile.MidRollCount -= 1;
            ChapterSelect("Midrolls", SF.saveFile.MidRolls[MidRollIndex].MidrollName);
            SF.saveFile.MidRolls[MidRollIndex].Seen = true;
        } else {
            PlayMidrolls();
        }
    }

    // Handles the building and pushing of text messages to the message list object.
    // textMessage: the prefab of the message (sent recieved)
    // messageContent: The text content of the message.
    public void TextPush(TypeOfText type, GameObject textMessage, string messageContent) {
        GameObject messageClone =
            Instantiate(textMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(SF.saveFile.contactPush));
        GameObject textContent = messageClone.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        GameObject typeOfText = messageClone.transform.GetChild(0).gameObject;
        textContent.GetComponent<TextMeshProUGUI>().text = messageContent;
        typeOfText.GetComponent<TextMeshProUGUI>().text = "" + (int)type;
    }

    // Handles the building and pushing of image messages to the message list object.
    // imageMessage: the prefab of which type of image text we're sending/recieving.
    // image: the actual image to be sent.
    public void ImagePush(TypeOfText type, string imgName, GameObject imageMessage, Sprite image) {
        GameObject messageClone =
            Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(SF.saveFile.contactPush));
        GameObject textContent = messageClone.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        GameObject imageContent = messageClone.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
        GameObject typeOfText = messageClone.transform.GetChild(0).gameObject;

        imageContent.GetComponent<Image>().sprite = image;
        textContent.GetComponent<TextMeshProUGUI>().text = imgName;
        typeOfText.GetComponent<TextMeshProUGUI>().text = "" + (int)type;

        if (type == TypeOfText.recImage || type == TypeOfText.sentImage) {
            Button button = messageClone.transform.GetChild(1).GetComponent<Button>();
            button.onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                gen.ModalWindowOpen(image, imgName[1..^1]);
            });
        }
    }

    // Handles message limits for all types of texts to the messageList
    // TypeOfText: an enum object that denotes the type of text we're sending
    // image: optional field in case we're sending an image
    // messageContent: default to "" in case you're sending an image.
    #nullable enable
    public void MessageListLimit(TypeOfText type, string imgName = "", Sprite? image = null ,string messageContent = "") {
        if (Shared.content.GetChild(SF.saveFile.contactPush).childCount >= 25){
            Destroy(Shared.content.GetChild(SF.saveFile.contactPush).GetChild(0).gameObject);
        } 
        switch(type){
            case TypeOfText.sentText:
                TextPush(TypeOfText.sentText, Prefabs.sentText, messageContent);
            break;
            case TypeOfText.recText:
                TextPush(TypeOfText.recText, Prefabs.recText, messageContent);
            break;
            case TypeOfText.sentEmoji:
                ImagePush(TypeOfText.sentEmoji, imgName, Prefabs.sentEmoji, image);
            break;
            case TypeOfText.recEmoji:
                ImagePush(TypeOfText.recEmoji, imgName,Prefabs.recEmoji, image);
            break;
            case TypeOfText.sentImage:
                ImagePush(TypeOfText.sentImage, imgName,Prefabs.sentImage, image);
            break;
            case TypeOfText.recImage:
                ImagePush(TypeOfText.recImage, imgName,Prefabs.recImage, image);
            break;
            case TypeOfText.chapEnd:
                TextPush(TypeOfText.chapEnd, Prefabs.ChapComplete, messageContent);
            break;
            case TypeOfText.indicateTime:
                TextPush(TypeOfText.indicateTime, Prefabs.TimeIndicator, messageContent);
            break;
        }
    }

    public void pushNotification(Sprite? pfp, string textContent) {
        bool viewingScreen = SF.saveFile.contactPush == SF.saveFile.selectedIndex;
        Destroy(Shared.notif);
        if (!viewingScreen) {
            Shared.notificationArea.GetComponent<AudioSource>().Play();
            gen.Show(Shared.cardsList.GetChild(SF.saveFile.contactPush).GetChild(2).transform);
            Shared.notif = Instantiate(Prefabs.Notification, new Vector3(0, 0, 0), Quaternion.identity, Shared.notificationArea);
            Shared.notif.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = textContent;
            Shared.notif.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        }
    }

    void ChoiceButtonClick(int NextChap) {
        SF.saveFile.CurrStoryPoint.SubChapIndex = NextChap;
        SF.saveFile.CurrStoryPoint.CurrTextIndex = 0;
        SF.saveFile.ChoiceNeeded = false;
        foreach (Transform Child in Shared.choices) {
            Destroy(Child.gameObject);
        }
        // Destroy(Shared.choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
        // Destroy(ChoiceClone);
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[NextChap]));
    }


    public void TextButton(Response resp, bool RespTree) {
        GameObject ChoiceClone =
            Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = resp.TextContent;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            if (!RespTree) {
                MessageListLimit(TypeOfText.sentText, messageContent: resp.TextContent);
            }
            ChoiceButtonClick(resp.SubChapNum);
        });
    }

    public void ImageButton(Response resp) {
        Sprite? image;
        GameObject ChoiceClone =
            Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
        
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        if (resp.Type == (int) TypeOfText.sentImage){
            image = Resources.Load("Images/Photos/" + resp.TextContent, typeof(Sprite)) as Sprite;
        } else {
            image = Resources.Load("Images/Emojis/" + resp.TextContent, typeof(Sprite)) as Sprite;
        }

        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        Sprite? frameEmoji = Resources.Load("Images/Emojis/photo", typeof(Sprite)) as Sprite;
        imageObject.GetComponent<Image>().sprite = resp.Type == (int)TypeOfText.sentImage ? frameEmoji : image;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            if (resp.Type == (int)TypeOfText.sentImage) {
                int indexOfPhoto = SF.saveFile.Photos.FindIndex(x => x.ImageName == resp.TextContent);
                UnlockImage(indexOfPhoto, resp.TextContent);
            }
            MessageListLimit((TypeOfText)resp.Type, resp.TextContent, image);
            ChoiceButtonClick(resp.SubChapNum);
        });
    }


    // handles the building and pushing of the text choice buttons into the choices list
    // indx: automated through forloop, handles destruction of buttons and next chap queuing
    // NextChap: list of ints, each for the next subchap to play based on click.
    // textContent: text to display and send.
    // public void TextButton(int indx, List<int> NextChap, string textContent, bool RespTree) {
    //     GameObject ChoiceClone =
    //         Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
    //     Destroy(ChoiceClone.transform.GetChild(1).gameObject);
    //     GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
    //     textObject.GetComponent<TextMeshProUGUI>().text = textContent;
    //     Button button = ChoiceClone.GetComponent<Button>();
    //     button.onClick.AddListener(() => {
    //         Shared.Wallpaper.GetComponent<AudioSource>().Play();
    //         if (!RespTree) {
    //             MessageListLimit(TypeOfText.sentText, messageContent: textContent);
    //         }
    //         ChoiceButtonClick(NextChap, indx, ChoiceClone);
    //     });
    // }

    // handles the building and pushing of the image/emoji choice buttons into the choices list
    // indx: automated through forloop, handles destruction of buttons and next chap queuing
    // NextChap: list of ints, each for the next subchap to play based on click.
    // type: type of image (emoji/photo)
    // image: sprite image to be sent (emoji/photo)
    // public void ImageButton(int indx, List<int> NextChap, TypeOfText type, string imgName, Sprite image) {
    //     GameObject ChoiceClone =
    //         Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);

    //     Destroy(ChoiceClone.transform.GetChild(0).gameObject);
    //     GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
    //     Sprite? frameEmoji = Resources.Load("Images/Emojis/photo", typeof(Sprite)) as Sprite;
    //     imageObject.GetComponent<Image>().sprite = type == TypeOfText.sentImage ? frameEmoji : image;
    //     Button button = ChoiceClone.GetComponent<Button>();
    //     button.onClick.AddListener(() => {
    //         Shared.Wallpaper.GetComponent<AudioSource>().Play();
    //         if (type == TypeOfText.sentImage) {
    //             string ImageName = imgName[1..^1];
    //             int indexOfPhoto = SF.saveFile.Photos.FindIndex(x => x.ImageName == ImageName);
    //             UnlockImage(indexOfPhoto, ImageName);
    //         }
    //         MessageListLimit(type, imgName, image);
    //         ChoiceButtonClick(NextChap, indx, ChoiceClone);
    //     });
    // }

    public void UnlockImage(int indexOfPhoto, string ImageName){
        if (!SF.saveFile.Photos[indexOfPhoto].Seen == true) {
            SF.saveFile.Photos[indexOfPhoto].Seen = true;
            int IndexOfCategory = SF.saveFile.PhotoCategories.FindIndex(x => x.Category == ImageName.Split("-")[0]);
            SF.saveFile.PhotoCategories[IndexOfCategory].NumberSeen += 1;
        }
    }
}