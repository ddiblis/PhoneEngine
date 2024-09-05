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
        ChapterSelect("Chapters", "Prelude");
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
    public void PopulateResps(List<Response> responses){
        for (int i = 0; i < responses.Count; i++) {
            Response resp = responses[i];
            switch (resp.Type) {
                case (int)TypeOfText.sentText:
                    TextButton(resp); 
                break;
                default:
                    ImageButton(resp);
                break;
            }
        }
        if (SF.saveFile.contactPush != SF.saveFile.selectedIndex){
            gen.Hide(Shared.choices); 
        } 
    }

    #nullable enable
    public void GenerateMessage(TextMessage textMessage, Sprite? pfp = null) {
        bool viewingScreen = SF.saveFile.contactPush == SF.saveFile.selectedIndex;
        if (viewingScreen) {
            Shared.content.GetComponent<AudioSource>().Play();   
        }
        switch ((TypeOfText) textMessage.Type) {
            case TypeOfText.recText:
                PushNotification(pfp, textMessage.TextContent);
            break;
            case TypeOfText.recImage:
                PushNotification(pfp);
            break;
            case TypeOfText.recEmoji:
                PushNotification(pfp);
            break;
        }
        MessageListLimit(textMessage);
    }

    // Handles wait time for the messages recieved so they don't all display at once.
        public IEnumerator MessageDelay(
        TextMessage textMessage,
        Sprite? pfp = null
    ) {
        if (SF.saveFile.Settings.FasterReplies) {
            yield return new WaitForSeconds(textMessage.TextDelay/2);
        } else {
            yield return new WaitForSeconds(textMessage.TextDelay);
        }
        GenerateMessage(textMessage, pfp);
    }
    # nullable disable

    public IEnumerator RecieveTexts(List<TextMessage> TextList, string Contact, int startingText = 0) {
        Sprite pfp = Resources.Load("Images/Headshots/" + Contact, typeof(Sprite)) as Sprite;

        for (int i = startingText; i < TextList.Count; i++) {
            SF.saveFile.CurrStoryPoint.CurrTextIndex = i;
            TextMessage textMessage = TextList[i];
            if (textMessage.Tendency == SF.saveFile.Stats.Tendency || textMessage.Tendency == (int)Tendency.Neutral) {
                yield return StartCoroutine(MessageDelay(textMessage, pfp));
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

        UnlockContact(indexOfContact);

        UnlockInstaAccount(InstaAccount);

        UnlockInstaPosts(subChap, InstaAccount);

        if (!SF.saveFile.ChoiceNeeded) {
            // Sends indicator of time passed
            if (subChap.TimeIndicator.Length > 0 && SF.saveFile.CurrStoryPoint.CurrTextIndex == 0){
                yield return StartCoroutine(MessageDelay(new TextMessage {
                    Type = (int) TypeOfText.indicateTime,
                    TextDelay = 1.5f,
                    TextContent = subChap.TimeIndicator
                }));
            }
            
            if (subChap.TextList.Count == 1 || SF.saveFile.CurrStoryPoint.CurrTextIndex + 1 != subChap.TextList.Count){
                yield return StartCoroutine(RecieveTexts(subChap.TextList, subChap.Contact, startingText));
            }
        }

        if (subChap.Responses.Count > 0){
            SF.saveFile.ChoiceNeeded = true;
            PopulateResps(subChap.Responses);
        } else {
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
        System.Random rnd = new();
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
    public void ImagePush(TypeOfText type, string imgName, GameObject imageMessage) {
        Sprite image;
        GameObject messageClone =
            Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(SF.saveFile.contactPush));
        GameObject textContent = messageClone.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        GameObject imageContent = messageClone.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
        GameObject typeOfText = messageClone.transform.GetChild(0).gameObject;

        if(type == TypeOfText.recImage || type == TypeOfText.sentImage) {
            gen.UnlockImage(imgName);
            image = Resources.Load("Images/Photos/" + imgName, typeof(Sprite)) as Sprite;
            Button button = messageClone.transform.GetChild(1).GetComponent<Button>();
            button.onClick.AddListener(() => {
                Shared.Wallpaper.GetComponent<AudioSource>().Play();
                gen.ModalWindowOpen(image, imgName);
            });
        } 
        else if (type == TypeOfText.recContact) {
            image = Resources.Load("Images/Headshots/" + imgName, typeof(Sprite)) as Sprite;
        }
        else {
            image = Resources.Load("Images/Emojis/" + imgName, typeof(Sprite)) as Sprite;
        }

        imageContent.GetComponent<Image>().sprite = image;
        textContent.GetComponent<TextMeshProUGUI>().text = imgName;
        typeOfText.GetComponent<TextMeshProUGUI>().text = "" + (int)type;
    }

    // Handles message limits for all types of texts to the messageList
    // TypeOfText: an enum object that denotes the type of text we're sending
    // image: optional field in case we're sending an image
    // messageContent: default to "" in case you're sending an image.
    #nullable enable
    public void MessageListLimit(TextMessage textMessage) {
        if (Shared.content.GetChild(SF.saveFile.contactPush).childCount >= 25){
            Destroy(Shared.content.GetChild(SF.saveFile.contactPush).GetChild(0).gameObject);
        } 
        switch((TypeOfText) textMessage.Type){
            case TypeOfText.sentText:
                TextPush(TypeOfText.sentText, Prefabs.sentText, textMessage.TextContent);
            break;
            case TypeOfText.recText:
                if (!SF.saveFile.Settings.GameMode) {
                    TextPush(TypeOfText.recText, Prefabs.recText, textMessage.TextContent);
                    break;
                }
                switch (textMessage.Tendency) {
                    case (int)Tendency.Neutral:
                        TextPush(TypeOfText.recText, Prefabs.recText, textMessage.TextContent);
                    break;
                    case (int)Tendency.Dominant:
                        TextPush(TypeOfText.recText, Prefabs.DomText, textMessage.TextContent);
                    break;
                    case (int)Tendency.Submissive:
                        TextPush(TypeOfText.recText, Prefabs.SubText, textMessage.TextContent);
                    break;
                }
            break;
            case TypeOfText.sentEmoji:
                ImagePush(TypeOfText.sentEmoji, textMessage.TextContent, Prefabs.sentEmoji);
            break;
            case TypeOfText.recEmoji:
                ImagePush(TypeOfText.recEmoji, textMessage.TextContent, Prefabs.recEmoji);
            break;
            case TypeOfText.sentImage:
                ImagePush(TypeOfText.sentImage, textMessage.TextContent, Prefabs.sentImage);
            break;
            case TypeOfText.recImage:
                ImagePush(TypeOfText.recImage, textMessage.TextContent, Prefabs.recImage);
            break;
            case TypeOfText.chapEnd:
                TextPush(TypeOfText.chapEnd, Prefabs.ChapComplete, textMessage.TextContent);
            break;
            case TypeOfText.indicateTime:
                TextPush(TypeOfText.indicateTime, Prefabs.TimeIndicator, textMessage.TextContent);
            break;
            case TypeOfText.recContact:
                ImagePush(TypeOfText.recContact, textMessage.TextContent, Prefabs.recContact);
                int indexOfContact = SF.saveFile.ContactsList.FindIndex(x => x.NameOfContact == textMessage.TextContent);
                UnlockContact(indexOfContact);
            break;
        }
    }

    public void PushNotification(Sprite? pfp, string textContent = "Photo Message") {
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
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[NextChap]));
    }

    // Add all handlers for stat changes here
    // Unfortunately I can't think of a better way to handle custom options than this
    private void HandleStatChange(string TextContent) {
        if (TextContent.Contains("Tendency")) {
            if(TextContent.Contains("Neutral")) {
                SF.saveFile.Stats.Tendency = (int)Tendency.Neutral;
            }
            else if (TextContent.Contains("Submissive")) {
                SF.saveFile.Stats.Tendency = (int)Tendency.Submissive;
            } 
            else if (TextContent.Contains("Dominant")) {
                SF.saveFile.Stats.Tendency = (int)Tendency.Dominant;
            }
        }
    }

    // handles the building and pushing of the text choice buttons into the choices list
    public void TextButton(Response resp) {
        // Logic for parsing the response string and determining if there is a stat change then displaying based
        // on game mode
        bool statChange = resp.TextContent.Contains("<");
        string response = "";
        if (statChange) {
            int indexofString = resp.TextContent.IndexOf("<");
            if (!SF.saveFile.Settings.GameMode) {
                response = resp.TextContent[0..indexofString];
            } else {
                response = resp.TextContent;
            }
        }

        GameObject ChoiceClone =
            Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = !statChange ? resp.TextContent : response;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            Shared.Wallpaper.GetComponent<AudioSource>().Play();
            if (statChange) {
                HandleStatChange(resp.TextContent);
            }
            if (!resp.RespTree) {
                MessageListLimit( new TextMessage {
                    Type = (int)TypeOfText.sentText,
                    TextContent = !statChange ? resp.TextContent : response
                });
            }
            ChoiceButtonClick(resp.SubChapNum);
        });
    }

    // handles the building and pushing of the image/emoji choice buttons into the choices list
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
            MessageListLimit(new TextMessage {
                Type = resp.Type,
                TextContent = resp.TextContent
            });
            ChoiceButtonClick(resp.SubChapNum);
        });
    }
}