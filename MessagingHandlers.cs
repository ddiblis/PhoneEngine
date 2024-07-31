using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Microsoft.Unity.VisualStudio.Editor;

public class MessagingHandlers : MonoBehaviour {



    // Handles the building and pushing of text messages to the message list object.
    // typeOfMessages: Prefab for the message type (sent or recieved)
    // messageContent: The text content of the message.
    public void TextPush(GameObject textMessage, string messageContent, Transform messageList){
        GameObject messageClone = Instantiate(textMessage, new Vector3(0, 0, 0), Quaternion.identity, messageList.transform);
        GameObject textContent = messageClone.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        textContent.GetComponent<TextMeshProUGUI>().text = messageContent;
    }

    // Handles the building and pushing of image messages to the message list object.
    // typeOfMessages: Prefab for the message type (sent or recieved)
    // messageContent: The image sprite of the message  
    public void ImagePush(GameObject imageMessage, Sprite image, Transform messageList) {
        GameObject messageClone = Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, messageList.transform);
        GameObject imageContent = messageClone.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        imageContent.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }

    // Handles wait time for the messages recieved so they don't all display at once.
    // sentMessage: Type of message (sent or recieved)
    // messageList: messageList object
    // messageContent: text of the message
    public IEnumerator handleAutoText(GameObject textMessage, Transform messageList, string messageContent) {
        yield return new WaitForSeconds(0.7f);
        if (messageList.childCount < 25){
            TextPush(textMessage, messageContent, messageList);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            TextPush(textMessage, messageContent, messageList);
        }
    }

    // Handles sent images within the limit allowed in the messageList object (default 25)
    // sentMessage: Type of message (sent or recieved)
    // messageList: messageList object
    // image: sprite of the image being sent in the text
    public void handleSentImage(GameObject sentMessage, Transform messageList, Sprite image){
        if (messageList.childCount < 25){
            ImagePush(sentMessage, image, messageList);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            ImagePush(sentMessage, image, messageList);
        }
    }

    // Handles sent text within the limit allowed in the messageList object (default 25)
    // sentMessage: Type of message (sent or recieved)
    // messageList: messageList object
    // messageContent: text of the message
    public void handleSentText(GameObject sentMessage, Transform messageList, string messageContent){
        if (messageList.childCount < 25){
            TextPush(sentMessage, messageContent, messageList);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            TextPush(sentMessage, messageContent, messageList);
        }
    }
    

    // handles the building and pushing of the text choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // textContent: shorthand of the text to be sent by using that button.
    public void choiceTextButton(GameObject choice, Transform choices, string textContent = "") {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(1).gameObject);
        GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = textContent;
    }

    // handles the building and pushing of the image choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // image: sprite image to be sent (emoji)
    public void choiceImageButton(GameObject choice, Transform choices, Sprite image) {
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(0).gameObject);
        GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
        imageObject.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }


    // Handles the output of player choice from button press to a sent text into the chat list
    // buttonPosition: 1 or 0, 0 for top button, 1 for bottom button
    public void handleTextChoice(int buttonPosition, GameObject messageType, Transform messageList, Transform choices, string textContent = "") {
        GameObject ButtonChoice = choices.transform.GetChild(buttonPosition).gameObject;
        Button button = ButtonChoice.GetComponent<Button>();
        button.onClick.AddListener(() => {
            handleSentText(messageType, messageList, textContent);
            Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
            Destroy(ButtonChoice);
        });
    }

    // Handles the output of player choice from button press to a sent text into the chat list
    // buttonPosition: 1 or 0, 0 for top button, 1 for bottom button
    public void handleImageChoice(int buttonPosition, GameObject messageType, Transform messageList, Transform choices, Sprite image) {
        GameObject ButtonChoice = choices.transform.GetChild(buttonPosition).gameObject;
        Button button = ButtonChoice.GetComponent<Button>();
        button.onClick.AddListener(() => {
            handleSentImage(messageType, messageList, image);
            Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
            Destroy(ButtonChoice);
        });
    }

    
}