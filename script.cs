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
            { "Hey bf", "Hey gf", "what are we eating for dinner today?", "idk sushi?", "nah I'm not in the mood for sushi, pizza?", "what kind of pizza?", "pepperoni?", "Sure what time are you home?", "I should be home by 1800", "Sounds good I'll order it and you pick it up when you're home?", "Yes please, see you in 2 hours", "sure", "ok", "i love you", "i love you too", "no you", "no you", "no me", "no way", "yes way", "shut up", "no you", "you shut up", "I love you okay seriously bye", "okay seriously bye"};


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
                yield return StartCoroutine(messagehandlers.handleRecievedMessages(sentTextMessage, optionList[i], messageList));
            } else {
                yield return StartCoroutine(messagehandlers.handleRecievedMessages(recievedTextMessage, optionList[i], messageList));
            }
        } 
    }

    // Start is called before the first frame update
    void Start() {
        Emojis emojis = new Emojis();
        // string toptext = "testing top button";
        // string bottomtext = "testing bottom button";

        // StartCoroutine(StartMessagesCoroutine());

        messagehandlers.choiceButtonConstructor(topChoice, choices, 0, emojis.gfStanding);
        messagehandlers.choiceButtonConstructor(bottomChoice, choices, 0, emojis.redHeart);
        // messagehandlers.handlePlayerChoice(0, 1, toptext, sentTextMessage, messageList, choices);
        // messagehandlers.handlePlayerChoice(1, 1, bottomtext, sentTextMessage, messageList, choices);
        messagehandlers.handlePlayerChoice(0, 0, sentImageMessage, messageList, emojis.gfStanding, choices);
        messagehandlers.handlePlayerChoice(1, 0, sentImageMessage, messageList, emojis.redHeart, choices);

    }

    // Update is called once per frame
    void Update() {
    }
}