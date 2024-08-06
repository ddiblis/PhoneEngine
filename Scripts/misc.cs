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

public class ResponseChoices {
        List<string> texts;
        bool img;
        Sprite Image;
        // TypeOfText type;
    }

    // sent: true text is sent, false text is recieved
    // textContent: text content of the text
    // choice: true there are choices, false there are no choices
    // choices: ResponseChoices Object
    public class TextMessage {
        bool sent { get; set; }
        string textContent { get; set; }
        string imageContent { get; set;}
        bool choice { get; set; }
        ResponseChoices choices { get; set; }
    }

/*Chapter file {
    bool ChapComplete
    SubChapter sub: {
        0: {
            List<string> textList
            List<string> imageList
            List<string> subList
            List<string> DomList
            List<float> responseTime
            Response<schema>{
                List<string> Shorthands
                List<string> topResps
                List<string> bottomResps
                float subchapterForTop
                float subchapterForBottom
            }
        }
    }
}
*/
