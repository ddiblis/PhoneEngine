using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;

public class MessagingHandlers : MonoBehaviour {

    List<string> optionList = new List<string>
            { "Hey bf", "what are we eating for dinner today?", "what time are you home?", "I love you okay seriously bye"};


    public GameObject headshot;
    public Transform messageList;

    // Choices buttons box
    public Transform choices;
    public GameObject choice;


    public GameObject sentTextMessage;
    public GameObject sentEmoji;
    public GameObject sentImage;
    public GameObject recievedTextMessage;
    public GameObject recievedImageMessage;

    int choicePosition;
    bool choiceMade;



    void Awake(){

        // Emojis emojis = new Emojis();
        
        // headshot.GetComponent<Image>().sprite = emojis.blondeHeadshot;

        StartCoroutine(StartMessagesCoroutine());

    }

    void Update(){
        // if (choiceMade){
        //     Debug.Log(choicePosition);
        // }
        // Debug.Log(doneDisplayingTexts);

    }

    public IEnumerator StartMessagesCoroutine(){
        // Emojis emojis = new Emojis();

        string toptext = "testing top button";
        string bottomtext = "testing bottom button";

        for (int i = 0; i < optionList.Count; i++) {
            yield return StartCoroutine(AutoTextLimit(optionList[i]));
        } 
        TextButton(0, toptext);
        TextButton(1, bottomtext);
        // EmojiButton(0, emojis.blackHeart);
        // EmojiButton(1, emojis.redHeart);
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
    
    #nullable enable
    public void MessageListLimit(TypeOfText type, Sprite? image = null ,string messageContent = ""){
        if (messageList.childCount < 25){
            switch(type){
                case TypeOfText.sentText:
                    TextPush(sentTextMessage, messageContent);
                    break;
                case TypeOfText.recText:
                    TextPush(recievedTextMessage, messageContent);                    
                    break;
                case TypeOfText.sentEmoji:
                    ImagePush(sentEmoji, image);
                    break;
                case TypeOfText.sentImage:
                    ImagePush(sentImage, image);
                    break;
            }
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            switch(type){
                case TypeOfText.sentText:
                    TextPush(sentTextMessage, messageContent);
                    break;
                case TypeOfText.recText:
                    TextPush(recievedTextMessage, messageContent);
                    break;
                case TypeOfText.sentEmoji:
                    ImagePush(sentEmoji, image);
                    break;
                case TypeOfText.sentImage:
                    ImagePush(sentImage, image);
                    break;
            }
        }
    }

    // Handles wait time for the messages recieved so they don't all display at once.
    // messageContent: text of the message
    public IEnumerator AutoTextLimit(string textContent) {
        float lenOfText = textContent.Length;
        Debug.Log(lenOfText * 0.2f / 2);
        yield return new WaitForSeconds(lenOfText * 0.2f / 2);
        MessageListLimit(TypeOfText.recText, messageContent: textContent);

    }

    // handles the building and pushing of the text choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // textContent: shorthand of the text to be sent by using that button.
    public void TextButton(int indx, string textContent = "") {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = textContent;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MessageListLimit(TypeOfText.sentText, messageContent: textContent);
            Destroy(choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            choiceMade = true;
            choicePosition = indx;
        });
    }

    // handles the building and pushing of the image choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // image: sprite image to be sent (emoji)
    public void ImageButton(int indx, Sprite image) {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<Image>().sprite = image;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MessageListLimit(TypeOfText.sentImage, image);
            Destroy(choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            choiceMade = true;
            choicePosition = indx;
        });
    }

    public void EmojiButton(int indx, Sprite image) {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<Image>().sprite = image;
        Button button = ChoiceClone.GetComponent<Button>();
        button.onClick.AddListener(() => {
            MessageListLimit(TypeOfText.sentEmoji, image);
            Destroy(choices.transform.GetChild(indx == 1 ? 0 : 1).gameObject);
            Destroy(ChoiceClone);
            choiceMade = true;
            choicePosition = indx;
        });
    }
}