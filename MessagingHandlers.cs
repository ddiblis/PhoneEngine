using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

public class MessagingHandlers : MonoBehaviour {

    List<string> optionList = new List<string>
            { "Hey bf", "what are we eating for dinner today?", "what time are you home?", "I love you okay seriously bye"};


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
        StartCoroutine(StartMessagesCoroutine());

        Emojis emojis = new Emojis();
        // string toptext = "testing top button";
        // string bottomtext = "testing bottom button";

        // choiceTextButton(toptext);
        // choiceTextButton(bottomtext);
        // handleTextChoice(0, toptext);
        // handleTextChoice(1, bottomtext);
        choiceImageButton(emojis.gfStanding);
        choiceImageButton(emojis.redHeart);
        // handleImageChoice(0, emojis.gfStanding);
        handleEmojiChoice(0, emojis.blackHeart);
        handleEmojiChoice(1, emojis.redHeart);
    }

    void Update(){
        if (choiceMade){
            Debug.Log(choicePosition);
        }
    }

    public IEnumerator StartMessagesCoroutine(){
        for (int i = 0; i < optionList.Count; i++) {
            yield return StartCoroutine(handleAutoText(optionList[i]));
        } 
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
        imageContent.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }

    // Handles wait time for the messages recieved so they don't all display at once.
    // messageContent: text of the message
    public IEnumerator handleAutoText(string messageContent) {
        yield return new WaitForSeconds(0.7f);
        if (messageList.childCount < 25){
            TextPush(recievedTextMessage, messageContent);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            TextPush(recievedTextMessage, messageContent);
        }
    }

    // Handles sent emojis within the limit allowed in the messageList object (default 25)
    // emoji: sprite of the emoji being sent in the text
    public void handleSentEmoji(Sprite emoji){
        if (messageList.childCount < 25){
            ImagePush(sentEmoji, emoji);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            ImagePush(sentEmoji, emoji);
        }
    }

    // Handles sent images within the limit allowed in the messageList object (default 25)
    // image: sprite of the image being sent in the text
    public void handleSentImage(Sprite image){
        if (messageList.childCount < 25){
            ImagePush(sentImage, image);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            ImagePush(sentImage, image);
        }
    }

    // Handles sent text within the limit allowed in the messageList object (default 25)
    // messageContent: text of the message
    public void handleSentText(string messageContent){
        if (messageList.childCount < 25){
            TextPush(sentTextMessage, messageContent);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            TextPush(sentTextMessage, messageContent);
        }
    }
    

    // handles the building and pushing of the text choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // textContent: shorthand of the text to be sent by using that button.
    public void choiceTextButton(string textContent = "") {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = textContent;
    }

    // handles the building and pushing of the image choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // image: sprite image to be sent (emoji)
    public void choiceImageButton(Sprite image) {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }


    // Handles the output of player choice from button press to a sent text into the chat list
    // buttonPosition: 1 or 0, 0 for top button, 1 for bottom button
    public void handleTextChoice(int buttonPosition, string textContent = "") {
        GameObject ButtonChoice = choices.transform.GetChild(buttonPosition).gameObject;
        Button button = ButtonChoice.GetComponent<Button>();
        button.onClick.AddListener(() => {
            handleSentText(textContent);
            Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
            Destroy(ButtonChoice);
            choiceMade = true;
            choicePosition = buttonPosition;
        });

    }

    // Handles the output of player choice from button press to a sent text into the chat list
    // buttonPosition: 1 or 0, 0 for top button, 1 for bottom button
    public void handleEmojiChoice(int buttonPosition, Sprite image) {
        GameObject ButtonChoice = choices.transform.GetChild(buttonPosition).gameObject;
        Button button = ButtonChoice.GetComponent<Button>();
        button.onClick.AddListener(() => {
            handleSentEmoji(image);
            Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
            Destroy(ButtonChoice);
            choiceMade = true;
            choicePosition = buttonPosition;
        });
    }

    // Handles the output of player choice from button press to a sent text into the chat list
    // buttonPosition: 1 or 0, 0 for top button, 1 for bottom button
    public void handleImageChoice(int buttonPosition, Sprite image) {
        GameObject ButtonChoice = choices.transform.GetChild(buttonPosition).gameObject;
        Button button = ButtonChoice.GetComponent<Button>();
        button.onClick.AddListener(() => {
            handleSentImage(image);
            Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
            Destroy(ButtonChoice);
            choiceMade = true;
            choicePosition = buttonPosition;
        });
    }
}