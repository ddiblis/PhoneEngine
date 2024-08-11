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

public class MessagingHandlers : MonoBehaviour {

    public GameObject backButton;    
    
    public ChapImport chap;
    public GeneralHandlers gen;
    public SharedObjects Shared;
    public PreFabs Prefabs;

    int CurrChapIndex;

    ChapImport.Chapter CurrChap;

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
    

    void Awake() {

        string[] FileList = Directory.GetFiles("Assets/Resources/Images/Headshots","*.png");
        foreach (string File in FileList) {
            Shared.ContactsList.Add(File[35..^4]);
        }
        
        // Creates onclick handler for back button.
        Button button = backButton.GetComponent<Button>();
        button.onClick.AddListener(() => {
            gen.Hide(Shared.textingApp);
            gen.Hide(Shared.displayedList.transform);
            Shared.selectedIndex = int.MinValue;
        });
        
        // creates as many messageLists as needed for the contacts and hides them.
        for (int i = 0; i < Shared.ContactsList.Count; i++) {
            Instantiate(Prefabs.messageList, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.transform);
            Shared.content.GetChild(i).localScale = new Vector3(0, 0, 0);
        } 


        gen.Hide(Shared.textingApp);


        
        NewGame();
    }

    public void NewGame() {
        CurrChap = chap.GetChapter("chapter1");
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[0]));
    }

    public void ChapterSelect(string Chapter) {
        CurrChap = chap.GetChapter(Chapter);
        StartCoroutine(StartMessagesCoroutine(CurrChap.SubChaps[0]));
    }
    

    // Creates and pushes the response buttons/hides them if they're not meant for currently viewed contact
    // Arguments taken from json file through StartMessagesCoroutine
    public void PopulateResps(List<string> Resps, List<int> NextChap){
        for (int i = 0; i < Resps.Count; i++) {
            string item = Resps[i];
            if (item.Contains("{")){
                Sprite img = Resources.Load("Images/Photos/" + item[1..^1], typeof(Sprite)) as Sprite;
                ImageButton(i, NextChap, TypeOfText.sentImage, img);
            } 
            else if (item.Contains("[")){
                Sprite img = Resources.Load("Images/Emojis/" + item[1..^1], typeof(Sprite)) as Sprite;
                ImageButton(i, NextChap, TypeOfText.sentEmoji, img);
            }
            else {
                TextButton(i, NextChap, item);
            }
        }
        if (Shared.contactPush != Shared.selectedIndex){
            gen.Hide(Shared.choices); 
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

    // handles deciphering and outputting the messages from the json subchapter then calling choice buttons
    public IEnumerator StartMessagesCoroutine(ChapImport.SubChap subChap) {
        string Contact = subChap.Contact;
        string TimeIndicator = subChap.TimeIndicator;
        List<string> TextList = subChap.TextList;
        List<float> RespTime = subChap.ResponseTime;
        ChapImport.Responses Responses = subChap.Responses;
        List<string> Resps = Responses.Resps;
        List<int> NextChap = Responses.NextChap;
        Shared.contactPush = Shared.ContactsList.IndexOf(Contact);
        int NumOfPerson = Shared.ContactsList.IndexOf(Contact);
        Sprite pfp = Resources.Load("Images/Headshots/" + NumOfPerson + Contact, typeof(Sprite)) as Sprite;            

        if (TimeIndicator.Length > 0){
            yield return StartCoroutine(AutoText(TypeOfText.indicateTime, 1.5f, textContent: TimeIndicator));
        }
        for (int i = 0; i < TextList.Count; i++) {
            string item = TextList[i];
            if (item.Contains("{")){
                string imgName = item[1..^1];
                Shared.seenImages.Add(imgName);
                Sprite img = Resources.Load("Images/Photos/" + imgName, typeof(Sprite)) as Sprite;
                yield return StartCoroutine(AutoText(TypeOfText.recImage, RespTime[i], pfp, img));
            } 
            else if (item.Contains("[")){
                Sprite img = Resources.Load("Images/Emojis/" + item[1..^1], typeof(Sprite)) as Sprite;
                yield return StartCoroutine(AutoText(TypeOfText.recEmoji, RespTime[i], pfp, img));
            }
            else {
                yield return StartCoroutine(AutoText(TypeOfText.recText, RespTime[i], pfp, textContent: item));
            }
        } 
        if (Resps.Count > 0){
            PopulateResps(Resps, NextChap);
        } else {
            yield return StartCoroutine(AutoText(TypeOfText.chapEnd, 1.0f, textContent: TextList[0]));
            CurrChapIndex += 1;
            if (CurrChapIndex <= Shared.ChapterList.Count -1) {
                ChapterSelect(Shared.ChapterList[CurrChapIndex]);
            }
        }
    }

    // Handles the building and pushing of text messages to the message list object.
    // textMessage: the prefab of the message (sent recieved)
    // messageContent: The text content of the message.
    public void TextPush(GameObject textMessage, string messageContent) {
        GameObject messageClone = Instantiate(textMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(Shared.contactPush));
        GameObject textContent = messageClone.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        textContent.GetComponent<TextMeshProUGUI>().text = messageContent;
    }

    // Handles the building and pushing of image messages to the message list object.
    // imageMessage: the prefab of which type of image text we're sending/recieving.
    // image: the actual image to be sent.
    public void ImagePush(TypeOfText type, GameObject imageMessage, Sprite image) {
        GameObject messageClone = Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, Shared.content.GetChild(Shared.contactPush));
        GameObject imageContent = messageClone.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        imageContent.GetComponent<Image>().sprite = image;
        if (type == TypeOfText.recImage || type == TypeOfText.sentImage) {
            Button button = messageClone.transform.GetChild(0).GetComponent<Button>();
            gen.ModalWindowOpen(button, image);
        }
    }

    // Handles message limits for all types of texts to the messageList
    // TypeOfText: an enum object that denotes the type of text we're sending
    // image: optional field in case we're sending an image
    // messageContent: default to "" in case you're sending an image.
    #nullable enable
    public void MessageListLimit(TypeOfText type, Sprite? image = null ,string messageContent = "") {
        if (Shared.content.GetChild(Shared.contactPush).childCount >= 25){
            Destroy(Shared.content.GetChild(Shared.contactPush).GetChild(0).gameObject);
        } 
        switch(type){
            case TypeOfText.sentText:
                TextPush(Prefabs.sentText, messageContent);
            break;
            case TypeOfText.recText:
                TextPush(Prefabs.recText, messageContent);
            break;
            case TypeOfText.sentEmoji:
                ImagePush(TypeOfText.sentEmoji, Prefabs.sentEmoji, image);
            break;
            case TypeOfText.recEmoji:
                ImagePush(TypeOfText.recEmoji, Prefabs.recEmoji, image);
            break;
            case TypeOfText.sentImage:
                ImagePush(TypeOfText.sentImage, Prefabs.sentImage, image);
            break;
            case TypeOfText.recImage:
                ImagePush(TypeOfText.recImage, Prefabs.recImage, image);
            break;
            case TypeOfText.chapEnd:
                TextPush(Prefabs.ChapComplete, messageContent);
            break;
            case TypeOfText.indicateTime:
                TextPush(Prefabs.TimeIndicator, messageContent);
            break;
        }
    }

    public void pushNotification(Sprite? pfp, string textContent) {
        bool viewingScreen = Shared.contactPush == Shared.selectedIndex;
        Destroy(Shared.notif);
        if (!viewingScreen) {
            gen.Show(Shared.cardsList.GetChild(Shared.contactPush).GetChild(2).transform);
            Shared.notif = Instantiate(Prefabs.Notification, new Vector3(0, 0, 0), Quaternion.identity, Shared.notificationArea);
            Shared.notif.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = textContent;
            Shared.notif.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = pfp;
        }
    }

    // Handles wait time for the messages recieved so they don't all display at once.
    // TypeOfText: an enum object that denotes the type of text we're sending
    // respTime: time to wait before sending the text.
    // image: optional. The image to send (emoji, photo)
    // messageContent: text of the message
    public IEnumerator AutoText(TypeOfText type, float respTime, Sprite? pfp = null, Sprite? image = null, string textContent = "Picture Message") {
        
        yield return new WaitForSeconds(respTime);

        switch (type) {
            case TypeOfText.recText:
                pushNotification(pfp, textContent);
                MessageListLimit(TypeOfText.recText, messageContent: textContent);
            break;
            case TypeOfText.recImage:
                pushNotification(pfp, textContent);
                MessageListLimit(TypeOfText.recImage, image);
            break;
            case TypeOfText.recEmoji:
                pushNotification(pfp, textContent);
                MessageListLimit(TypeOfText.recEmoji, image);
            break;
            case TypeOfText.chapEnd:
                MessageListLimit(TypeOfText.chapEnd, messageContent: textContent);
            break;
            case TypeOfText.indicateTime:
                MessageListLimit(TypeOfText.indicateTime, messageContent: textContent);
            break;
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
    public void ImageButton(int indx, List<int> NextChap, TypeOfText type, Sprite image) {
        GameObject ChoiceClone = Instantiate(Prefabs.choice, new Vector3(0, 0, 0), Quaternion.identity, Shared.choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<Image>().sprite = image;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MessageListLimit(type, image);
            Destroy(Shared.choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            responseHandle(NextChap[indx]);
        });
    }
}