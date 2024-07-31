

    // top: true for top button clicked, false for bottom button
    // singleChoice: true is single choice, false multiple choice
    /* topText/bottomText: int of 1 or 0:
       1 denotes it is a text so that the handler removes the image, 
       0 removes the text so that only image remains */
    public class ResponseChoices {
        bool top { get; set;}
        bool singleChoice { get; set; }
        int topText { get; set; }
        string topChoiceShortHand { get; set; }
        string topChoiceText { get; set; }
        int bottomText { get; set; }
        string bottomChoiceShortHand { get; set; }
        string bottomChoiceText { get; set; }
    }

    // sent: true text is sent, false text is recieved
    // textContent: text content of the text
    // choice: true there are choices, false there are no choices
    // choices: ResponseChoices Object
    public class TextMessage {
        bool sent { get; set; }
        string textContent { get; set; }
        bool choice { get; set; }
        ResponseChoices choices { get; set; }
    }

