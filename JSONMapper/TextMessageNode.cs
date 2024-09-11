using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class TextMessageNode : BaseNode {

        // Add some kind of comprehension where if choice of type is emoji, it hides textbox and instead displays a drop down of emojis, then 
        // when an emoji is selected, the value is put into the textfield value, find the parsing method you can use for items inside of the 
        // resources foulder, iterate and create a list of the emojis avaliable
        // Side, maybe do the same thing to images 
        // Also implement the values system you devised, just remember what it was
        readonly List<string> TypeOptions = new() {
            "Type of Text", "Recieved Text", "Recieved Image", "Recieved Emoji", "Chapter end", "Recieved Contact", "Indicate Time"
        };
        readonly List<int> TypeValues = new() {
            0, 1, 3, 5, 6, 7, 8
        };
        readonly List<string> DelayOptions = new() {
            "Delay Options", "Almost Instant", "Very Fast", "Fast", "Medium", "Slow", "Very slow", "Dramatic Pause"
        };
        readonly List<float> DelayValues = new() {
            0, 0.16f, 0.57f, 1.1f, 2.1f, 2.5f, 3.5f, 5.1f 
        };
        readonly List<string> TendencyOptions = new() {
            "Tendency Options", "Neutral", "Submissive", "Dominant"
        };
        readonly List<int> TendencyValues = new() {
            0, 0, 1, 2
        };

        public string AltContact;        
        public int Type;
        public int Tendency;
        public string TextContent;
        public float TextDelay;

        public TextMessageNode NextTextNode;


        private readonly TextField TextMessageField;
        private readonly TextField AltContactField;
        private readonly DropdownField TypeDropDown;
        private readonly DropdownField DelayDropDown;
        private readonly DropdownField TendencyDropDown;
        public Port ParentSubChapPort;
        public Port PrevText;
        public Port NextText;


        public TextMessageNode(GraphView graphView) : base(graphView) {

            title = "TextMessage";

            ParentSubChapPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(SubChapNode));
            ParentSubChapPort.portName = "Parent SubChap";
            inputContainer.Add(ParentSubChapPort);

            PrevText = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(TextMessageNode));
            PrevText.portName = "Previous Text";
            inputContainer.Add(PrevText);

            NextText = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(TextMessageNode));
            NextText.portName = "Next Text";
            outputContainer.Add(NextText);

            var CustomDataContainer = new VisualElement();
            CustomDataContainer.AddToClassList("jm-node__custom-data-container");

            var Foldout = new Foldout() { text = "Text Message Content" };

            AltContactField = new TextField("Alt Contact") { value = TextContent };
            AltContactField.RegisterValueChangedCallback(evt => AltContact = evt.newValue);

            TextMessageField = new TextField("Text message") { value = TextContent };
            TextMessageField.RegisterValueChangedCallback(evt => TextContent = evt.newValue);

            TypeDropDown = new DropdownField("Text Type", TypeOptions, 0);
            TypeDropDown.RegisterValueChangedCallback(evt => Type = TypeValues[TypeOptions.FindIndex(x => x == evt.newValue)]);

            TendencyDropDown = new DropdownField("Tendency", TendencyOptions, 0);
            TendencyDropDown.RegisterValueChangedCallback(evt => Tendency = TendencyValues[TendencyOptions.FindIndex(x => x == evt.newValue)]);

            DelayDropDown = new DropdownField("Text Delay", DelayOptions, 0);
            TendencyDropDown.RegisterValueChangedCallback(evt => TextDelay = DelayValues[DelayOptions.FindIndex(x => x == evt.newValue)]);


            AltContactField.AddClasses(
                "jm-node__subchap-textfield",
                "jm-node__subchap-quote-textfield"
            );
            TextMessageField.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            TypeDropDown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            DelayDropDown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );

            TendencyDropDown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );

            Foldout.Add(AltContactField);
            Foldout.Add(TextMessageField);
            Foldout.Add(TypeDropDown);
            Foldout.Add(DelayDropDown);
            Foldout.Add(TendencyDropDown);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            int TypeIndex = TypeValues.FindIndex(x => x == Type);
            int DelayIndex = DelayValues.FindIndex(x => x == TextDelay);
            int TendencyIndex = TendencyValues.FindIndex(x => x == Tendency);
            TextMessageField.value = TextContent;
            AltContactField.value = AltContact;
            TendencyDropDown.value = TendencyOptions[TendencyIndex > 0 ? TendencyIndex : 1];
            TypeDropDown.value = TypeOptions[TypeIndex > 0 ? TypeIndex : 1];
            DelayDropDown.value = DelayOptions[DelayIndex > 0 ? DelayIndex : 1];
        }

        public TextMessageData ToTextMessageNodeData() {
            Rect rect = GetPosition();
            return new TextMessageData {
                Type = Type,
                AltContact = AltContact,
                TextContent = TextContent,
                TextDelay = TextDelay,
                Tendency = Tendency,
                location = new Location {
                    x = rect.x,
                    y = rect.y,
                    Width = rect.width,
                    Height = rect.height
                }
            };
        }

        public TextMessage ToTextMessageData() {
            return new TextMessage {
                Type = Type,
                AltContact = AltContact,
                TextContent = TextContent,
                TextDelay = TextDelay,
                Tendency = Tendency
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new TextMessageNode(graphView);
        }
    }
}
