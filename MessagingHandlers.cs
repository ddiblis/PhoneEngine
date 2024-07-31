using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Microsoft.Unity.VisualStudio.Editor;

public class MessagingHandlers : MonoBehaviour {


    TextMeshProUGUI messageText;
    


    // Handles the building and pushing of text messages to the message list object.
    // typeOfMessages: Prefab for the message type (sent or recieved)
    // messageContent: The text content of the message.
    // To do: add handler for image messages.
    public void handleTextPush(GameObject textMessage, string messageContent, Transform messageList){
        GameObject messageClone = Instantiate(textMessage, new Vector3(0, 0, 0), Quaternion.identity, messageList.transform);
        GameObject textContent = messageClone.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        messageText = textContent.GetComponent<TextMeshProUGUI>();
        messageText.text = messageContent;
    }

    public void handleImagePush(GameObject imageMessage, Sprite image, Transform messageList) {
        GameObject messageClone = Instantiate(imageMessage, new Vector3(0, 0, 0), Quaternion.identity, messageList.transform);
        GameObject imageContent = messageClone.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        imageContent.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }

    
    public IEnumerator handleRecievedMessages(GameObject textMessage, string messageContent, Transform messageList) {
        yield return new WaitForSeconds(0.7f);
        if (messageList.childCount < 25){
            handleTextPush(textMessage, messageContent, messageList);
        } else {
            Destroy(messageList.transform.GetChild(0).gameObject);
            handleTextPush(textMessage, messageContent, messageList);
        }
    }

    public void handleSentMessages(GameObject sentMessage, string messageContent, Transform messageList, Sprite image, bool result){
        if (result) {
            if (messageList.childCount < 25){
                handleTextPush(sentMessage, messageContent, messageList);
            } else {
                Destroy(messageList.transform.GetChild(0).gameObject);
                handleTextPush(sentMessage, messageContent, messageList);
            }
        } else {
            if (messageList.childCount < 25){
                handleImagePush(sentMessage, image, messageList);
            } else {
                Destroy(messageList.transform.GetChild(0).gameObject);
                handleImagePush(sentMessage, image, messageList);
            }
        }
    }
    

    // handles the building and pushing of the choice buttons into the choices list
    // choice: prefab of the button (top or bottom) ps. destinction might not be necessary, we'll see.
    // textContent: shorthand of the text to be sent by using that button.
    // contentType: if 1 it removes the image, if 0 it removes the text
    public void choiceButtonConstructor(GameObject choice, Transform choices, int contentType, Sprite image, string textContent = "") {
        bool result = contentType != 0; 
        GameObject ChoiceClone = Instantiate(choice, new Vector3(0, 0, 0), Quaternion.identity, choices.transform);
        Destroy(ChoiceClone.transform.GetChild(contentType).gameObject);
        if (result) {
            GameObject textObject = ChoiceClone.transform.GetChild(0).gameObject;
            textObject.GetComponent<TextMeshProUGUI>().text = textContent;
        } else {
            GameObject imageObject = ChoiceClone.transform.GetChild(1).gameObject;
            imageObject.GetComponent<UnityEngine.UI.Image>().sprite = image;
        }
    }

    // Handles the output of player choice from button press to a sent text into the chat list
    // buttonPosition: 1 or 0, 0 for top button, 1 for bottom button
    // content: 1 or 0, 0 for image, 1 for text
    public void handlePlayerChoice(int buttonPosition, int content, GameObject messageType, Transform messageList, Sprite image, Transform choices, string textContent = "") {
        bool result = content != 0; 
        GameObject ButtonChoice = choices.transform.GetChild(buttonPosition).gameObject;
        Button button = ButtonChoice.GetComponent<Button>();
        if (result) {
            button.onClick.AddListener(() => {
                handleSentMessages(messageType, textContent, messageList, image, result);
                Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
                Destroy(ButtonChoice);
            });
        } else {
            button.onClick.AddListener(() => {
                handleSentMessages(messageType, textContent, messageList, image, result);
                Destroy(choices.transform.GetChild(buttonPosition == 1 ? 0 : 1).gameObject);
                Destroy(ButtonChoice);
            });
        }

        
    }
}