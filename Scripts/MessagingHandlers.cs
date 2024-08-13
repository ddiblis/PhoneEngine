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
    public SavedItems saved;


    ChapImport.Chapter CurrChap;
    
    public void BackButton() {
        // Creates onclick handler for back button.
        Button button = backButton.GetComponent<Button>();
        button.onClick.AddListener(() => {
            gen.Hide(Shared.textingApp);
            gen.Hide(Shared.displayedList);
            saved.selectedIndex = int.MinValue;
        });
    }

    public void GenerateContactsList() {
        string[] FileList = Directory.GetFiles("Assets/Resources/Images/Headshots","*.png");
        foreach (string File in FileList) {
            saved.ContactsList.Add(File[35..^4]);
        }
    }

    // creates as many messageLists as needed for the contacts and hides them.
    public void GenerateMessageLists() {
        for (int i = 0; i < saved.ContactsList.Count; i++) {
            saved.UnlockedContacts.Add(false);
            Instantiate(Prefabs.messageList, new Vector3(0, 0, 0), Quaternion.identity, Shared.content);
            gen.Hide(Shared.content.GetChild(i));
        } 
    }

    public void NewGame() {
        CurrChap = chap.GetChapter("chapter1");
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[0]));
    }

    public void ChapterSelect(string Chapter, int subChapIndex = 0, int currentText = 0) {
        CurrChap = chap.GetChapter(Chapter);
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[subChapIndex], currentText));
    }
    

    // Creates and pushes the response buttons/hides them if they're not meant for currently viewed contact
    // Arguments taken from json file through StartMessagesCoroutine
    public void PopulateResps(List<string> Resps, List<int> NextChap){
        for (int i = 0; i < Resps.Count; i++) {
            string item = Resps[i];
            if (item.Contains("{")){
                Sprite img = Resources.Load("Images/Photos/" + item[1..^1], typeof(Sprite)) as Sprite;
                ImageButton(i, NextChap, TypeOfText.sentImage, item, img);
            } 
            else if (item.Contains("[")){
                Sprite img = Resources.Load("Images/Emojis/" + item[1..^1], typeof(Sprite)) as Sprite;
                ImageButton(i, NextChap, TypeOfText.sentEmoji, item, img);
            }
            else {
                TextButton(i, NextChap, item);
            }
        }
        if (saved.contactPush != saved.selectedIndex){
            gen.Hide(Shared.choices); 
        } 
    }

    #nullable enable
    public void GenerateMessage(TypeOfText type, string textContent, string imgName, Sprite? pfp = null, Sprite? image = null) {
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

    # nullable enable
    // Handles wait time for the messages recieved so they don't all display at once.
    // TypeOfText: an enum object that denotes the type of text we're sending
    // respTime: time to wait before sending the text.
    // image: optional. The image to send (emoji, photo)
    // messageContent: text of the message
    public IEnumerator MessageDelay(TypeOfText type, float respTime, Sprite? pfp = null, Sprite? image = null, string textContent = "Picture Message", string imgName = "") {
        
        yield return new WaitForSeconds(respTime);
        GenerateMessage(type, textContent, imgName, pfp, image);

    }
    # nullable disable

    public IEnumerator RecieveTexts(List<string> TextList, List<float> RespTime, Sprite pfp, int startingText = 0) {
        for (int i = startingText; i < TextList.Count; i++) {
            saved.CurrText = i;
            string item = TextList[i];
            if (item.Contains("{")){
                string imgName = item[1..^1];
                saved.seenImages.Add(imgName);
                Sprite img = Resources.Load("Images/Photos/" + imgName, typeof(Sprite)) as Sprite;
                yield return StartCoroutine(MessageDelay(TypeOfText.recImage, RespTime[i], pfp, img, imgName: item));
            } 
            else if (item.Contains("[")){
                Sprite img = Resources.Load("Images/Emojis/" + item[1..^1], typeof(Sprite)) as Sprite;
                yield return StartCoroutine(MessageDelay(TypeOfText.recEmoji, RespTime[i], pfp, img, imgName: item));
            }
            else {
                yield return StartCoroutine(MessageDelay(TypeOfText.recText, RespTime[i], pfp, textContent: item));
            }
        }
    }

    // handles deciphering and outputting the messages from the json subchapter then calling choice buttons
    public IEnumerator StartMessagesCoroutine(ChapImport.SubChap subChap, int startingText = 0) {
        string Contact = subChap.Contact;
        string TimeIndicator = subChap.TimeIndicator;
        List<string> TextList = subChap.TextList;
        List<float> RespTime = subChap.ResponseTime;
        ChapImport.Responses Responses = subChap.Responses;
        List<string> Resps = Responses.Resps;
        List<int> NextChap = Responses.NextChap;

        // Chooses messageList parent for messages to be pushed to
        saved.contactPush = saved.ContactsList.IndexOf(Contact);
        int NumOfPerson = saved.ContactsList.IndexOf(Contact);
        Sprite pfp = Resources.Load("Images/Headshots/" + NumOfPerson + Contact, typeof(Sprite)) as Sprite;

        // Unlocks contact if they're not already unlocked: displays their contact card      
        if (!saved.UnlockedContacts[NumOfPerson]) {
            saved.UnlockedContacts[NumOfPerson] = true;
        }   


        if (!saved.ChoiceNeeded) {
            // Sends indicator of time passed
            if (TimeIndicator.Length > 0 && saved.CurrText == 0){
                yield return StartCoroutine(MessageDelay(TypeOfText.indicateTime, 1.5f, textContent: TimeIndicator));
            }
            
            if (TextList.Count == 1 || saved.CurrText+1 != TextList.Count){
                yield return StartCoroutine(RecieveTexts(TextList, RespTime, pfp, startingText));
            }
        }

        if (Resps.Count > 0){
            saved.ChoiceNeeded = true;
            PopulateResps(Resps, NextChap);
        } else {
            yield return StartCoroutine(MessageDelay(TypeOfText.chapEnd, 1.0f, textContent: TextList[0]));
            saved.CurrChapIndex += 1;
            if (saved.CurrChapIndex <= saved.ChapterList.Count -1) {
                saved.CurrSubChapIndex = 0;
                ChapterSelect(saved.ChapterList[saved.CurrChapIndex]);
            }
        }
    }

    // Handles the building and pushing of text messages to the message list object.
    // textMessage: the prefab of the message (sent recieved)
    // messageContent: The text content of the message.
    public void TextPush(TypeOfText type, GameObject textMessage, string messageContent) {
        GameObject messageClone = Instantiate(textMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(saved.contactPush));
        GameObject textContent = messageClone.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
        GameObject typeOfText = messageClone.transform.GetChild(0).gameObject;
        textContent.GetComponent<TextMeshProUGUI>().text = messageContent;
        typeOfText.GetComponent<TextMeshProUGUI>().text = "" + (int)type;
    }

    // Handles the building and pushing of image messages to the message list object.
    // imageMessage: the prefab of which type of image text we're sending/recieving.
    // image: the actual image to be sent.
    public void ImagePush(TypeOfText type, string imgName, GameObject imageMessage, Sprite image) {
        GameObject messageClone = Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(saved.contactPush));
        GameObject imageContent = messageClone.transform.GetChild(1).GetChild(1).GetChild(0).gameObject;
        GameObject textContent = messageClone.transform.GetChild(1).GetChild(1).GetChild(1).gameObject;
        GameObject typeOfText = messageClone.transform.GetChild(0).gameObject;
        imageContent.GetComponent<Image>().sprite = image;
        textContent.GetComponent<TextMeshProUGUI>().text = imgName;
        typeOfText.GetComponent<TextMeshProUGUI>().text = "" + (int)type;
        if (type == TypeOfText.recImage || type == TypeOfText.sentImage) {
            Button button = messageClone.transform.GetChild(1).GetComponent<Button>();
            gen.ModalWindowOpen(button, image, imgName);
        }
    }

    // Handles message limits for all types of texts to the messageList
    // TypeOfText: an enum object that denotes the type of text we're sending
    // image: optional field in case we're sending an image
    // messageContent: default to "" in case you're sending an image.
    #nullable enable
    public void MessageListLimit(TypeOfText type, string imgName = "", Sprite? image = null ,string messageContent = "") {
        if (Shared.content.GetChild(saved.contactPush).childCount >= 25){
            Destroy(Shared.content.GetChild(saved.contactPush).GetChild(0).gameObject);
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
        bool viewingScreen = saved.contactPush == saved.selectedIndex;
        Destroy(Shared.notif);
        if (!viewingScreen) {
            gen.Show(Shared.cardsList.GetChild(saved.contactPush).GetChild(2).transform);
            Shared.notif = Instantiate(Prefabs.Notification, new Vector3(0, 0, 0), Quaternion.identity, Shared.notificationArea);
            Shared.notif.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = textContent;
            Shared.notif.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        }
    }

    // Called on to start the next coroutine for the subchapter.
    public void responseHandle(int subChapNum) {
        if (!CurrChap.ChapComplete) {
            StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[subChapNum]));
        }
        if (subChapNum == CurrChap.SubChaps.Count - 1) {
            CurrChap.ChapComplete = true;
        }
    }

    // handles the building and pushing of the text choice buttons into the choices list
    // indx: automated through forloop, handles destruction of buttons and next chap queuing
    // NextChap: list of ints, each for the next subchap to play based on click.
    // textContent: text to display and send.
    public void TextButton(int indx, List<int> NextChap, string textContent) {
        GameObject ChoiceClone = Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = textContent;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            saved.CurrSubChapIndex = NextChap[indx];
            saved.CurrText = 0;
            saved.ChoiceNeeded = false;
            MessageListLimit(TypeOfText.sentText, messageContent: textContent);
            Destroy(Shared.choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            responseHandle(NextChap[indx]);
        });
    }

    // handles the building and pushing of the image/emoji choice buttons into the choices list
    // indx: automated through forloop, handles destruction of buttons and next chap queuing
    // NextChap: list of ints, each for the next subchap to play based on click.
    // type: type of image (emoji/photo)
    // image: sprite image to be sent (emoji/photo)
    public void ImageButton(int indx, List<int> NextChap, TypeOfText type, string imgName, Sprite image) {
        GameObject ChoiceClone = Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<Image>().sprite = image;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            saved.CurrSubChapIndex = NextChap[indx];
            saved.CurrText = 0;
            saved.ChoiceNeeded = false;
            MessageListLimit(type, imgName, image);
            Destroy(Shared.choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            responseHandle(NextChap[indx]);
        });
    }
}