

    // oneChoice: true is single choice, false multiple choice
    public class ResponseChoices {
        bool oneChoice { get; set; }
        string topShortHand { get; set; }
        string topText { get; set; }
        string bottomShortHand { get; set; }
        string bottomText { get; set; }
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

