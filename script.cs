using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Animations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

public class Test : MonoBehaviour {
    List<string> optionList = new List<string>
            { "Hey bf", "what are we eating for dinner today?",  "nah I'm not in the mood for sushi, pizza?", "what kind of pizza?", "pepperoni?", "Sure what time are you home?", "I should be home by 1800", "Sounds good I'll order it and you pick it up when you're home?", "Yes please, see you in 2 hours", "sure", "ok", "i love you", "i love you too", "no you", "no you", "no me", "no way", "yes way", "shut up", "no you", "you shut up", "I love you okay seriously bye", "okay seriously bye"};


    readonly MessagingHandlers messagehandlers = new MessagingHandlers();

    public Transform messageList;
    public GameObject sentTextMessage;
    public GameObject sentImageMessage;
    public GameObject recievedTextMessage;
    public GameObject recievedImageMessage;

    // Choices buttons box
    public Transform choices;
    public GameObject topChoice;
    public GameObject bottomChoice;

    public IEnumerator StartMessagesCoroutine(){
        for (int i = 0; i < optionList.Count; i++) {
            if (i%2 != 0) {
                yield return StartCoroutine(messagehandlers.handleAutoText(sentTextMessage, messageList, optionList[i]));
            } else {
                yield return StartCoroutine(messagehandlers.handleAutoText(recievedTextMessage, messageList, optionList[i]));
            }
        } 
    }

    // Start is called before the first frame update
    void Start() {
        Emojis emojis = new Emojis();
        string toptext = "testing top button";
        string bottomtext = "testing bottom button";

        StartCoroutine(StartMessagesCoroutine());

        messagehandlers.choiceTextButton(topChoice, choices, toptext);
        messagehandlers.choiceTextButton(bottomChoice, choices, bottomtext);
        messagehandlers.handleTextChoice(0, sentTextMessage, messageList, choices, toptext);
        messagehandlers.handleTextChoice(1, sentTextMessage, messageList, choices, bottomtext);
        // messagehandlers.choiceImageButton(topChoice, choices, emojis.gfStanding);
        // messagehandlers.choiceImageButton(bottomChoice, choices, emojis.redHeart);
        // messagehandlers.handleImageChoice(0, sentImageMessage, messageList, choices, emojis.gfStanding);
        // messagehandlers.handleImageChoice(1, sentImageMessage, messageList, choices, emojis.redHeart);

    }

    // Update is called once per frame
    void Update() {
    }
}