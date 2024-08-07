using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine.TextCore.Text;

public class MessagingHandlers : MonoBehaviour {
    public GameObject headshot;
    public Transform messageList;

    // Choices buttons box
    public Transform choices;
    public GameObject choice;


    public GameObject sentText;
    public GameObject recText;
    public GameObject sentImage;
    public GameObject recImage;
    public GameObject sentEmoji;
    public GameObject recEmoji;

    
    public ChapImport chap;


    void Awake(){
        ChapImport.Chapter chapOne = chap.getChapter();
        StartCoroutine(StartMessagesCoroutine(chapOne.SubChaps[0]));

    }

    void Update(){
    }

    public void responseHandle(int subChapNum){
        ChapImport.Chapter chapOne = chap.getChapter();
        StartCoroutine(StartMessagesCoroutine(chapOne.SubChaps[subChapNum]));
    }

    public void PopulateResps(List<string> Resps, List<int> subChaps){
        for (int i = 0; i < Resps.Count; i++) {
            string item = Resps[i];
            if (item.Contains("{")){
                Sprite img = Resources.Load(item[1..^1], typeof(Sprite)) as Sprite;
                ImageButton(i, subChaps, TypeOfText.sentImage, img);
            } 
            else if (item.Contains("[")){
                Sprite img = Resources.Load(item[1..^1], typeof(Sprite)) as Sprite;
                ImageButton(i, subChaps, TypeOfText.sentEmoji, img);
            }
            else {
                TextButton(i, subChaps, item);
            }
        }
    }

    public IEnumerator StartMessagesCoroutine(ChapImport.SubChap subChap){
        List<string> TextList = subChap.TextList;
        List<float> RespTime = subChap.ResponseTime;
        ChapImport.Responses Responses = subChap.Responses;
        List<string> Resps = Responses.Resps;
        List<int> subChaps = Responses.SubChaps;

        for (int i = 0; i < TextList.Count; i++) {
            string item = TextList[i];
            if (item.Contains("{")){
                Sprite img = Resources.Load(item[1..^1], typeof(Sprite)) as Sprite;
                yield return StartCoroutine(AutoText(TypeOfText.recImage, RespTime[i], img));
            } 
            else if (item.Contains("[")){
                Sprite img = Resources.Load(item[1..^1], typeof(Sprite)) as Sprite;
                yield return StartCoroutine(AutoText(TypeOfText.recEmoji, RespTime[i], img));
            }
            else {
                yield return StartCoroutine(AutoText(TypeOfText.recText, RespTime[i], textContent: item));
            }
        } 
        PopulateResps(Resps, subChaps);
    }

    // Handles the building and pushing of text messages to the message list object.
    // messageContent: The text content of the message.
    public void TextPush(GameObject textMessage, string messageContent){
        GameObject messageClone = Instantiate(textMessage, new Vector3(0, 0, 0), Quaternion.identity, messageList.transform);
        GameObject textContent = messageClone.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        textContent.GetComponent<TextMeshProUGUI>().text = messageContent;
    }

    // Handles the building and pushing of image messages to the message list object.
    // messageContent: The image sprite of the message  
    public void ImagePush(GameObject imageMessage, Sprite image) {
        GameObject messageClone = Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, messageList.transform);
        GameObject imageContent = messageClone.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        imageContent.GetComponent<Image>().sprite = image;
    }

    public enum TypeOfText {
        sentText = 0,
        recText = 1,
        sentImage = 2,
        recImage = 3,
        sentEmoji = 4,
        recEmoji = 5
    }
    


    // Handles message limits for all types of texts to the messageList
    // TypeOfText: an enum object that denotes the type of text we're sending
    // image: optional field in case we're sending an image
    // messageContent: default to "" it's the text of the message being sent
    #nullable enable
    public void MessageListLimit(TypeOfText type, Sprite? image = null ,string messageContent = ""){
        if (messageList.childCount >= 25){
            Destroy(messageList.transform.GetChild(0).gameObject);
        } 
        switch(type){
            case TypeOfText.sentText:
                TextPush(sentText, messageContent);
            break;
            case TypeOfText.recText:
                TextPush(recText, messageContent);
            break;
            case TypeOfText.sentEmoji:
                ImagePush(sentEmoji, image);
            break;
            case TypeOfText.recEmoji:
            ImagePush(recEmoji, image);
            break;
            case TypeOfText.sentImage:
                ImagePush(sentImage, image);
            break;
            case TypeOfText.recImage:
                ImagePush(recImage, image);
            break;
        }
    }

    // Handles wait time for the messages recieved so they don't all display at once.
    // messageContent: text of the message
    public IEnumerator AutoText(TypeOfText type, float respTime, Sprite? image = null, string textContent = "") {

        yield return new WaitForSeconds(respTime);
        switch (type){
            case TypeOfText.recText:
                MessageListLimit(TypeOfText.recText, messageContent: textContent);
            break;
            case TypeOfText.recImage:
                MessageListLimit(TypeOfText.recImage, image);
            break;
            case TypeOfText.recEmoji:
                MessageListLimit(TypeOfText.recEmoji, image);
            break;
        }
    }

    // handles the building and pushing of the text choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // textContent: shorthand of the text to be sent by using that button.
    public void TextButton(int indx, List<int> subChaps,string textContent = "") {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = textContent;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MessageListLimit(TypeOfText.sentText, messageContent: textContent);
            Destroy(choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            responseHandle(subChaps[indx]);
        });
    }

    // handles the building and pushing of the image/emoji choice buttons into the choices list
    // indx: Index of the button, preset, can be any number you want but designed to be 0 or 1 and results in different responses
    // type: type of image (emoji/photo)
    // image: sprite image to be sent (emoji/photo)
    public void ImageButton(int indx, List<int> subChaps, TypeOfText type, Sprite image) {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<Image>().sprite = image;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MessageListLimit(type, image);
            Destroy(choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            responseHandle(subChaps[indx]);
        });
    }
}