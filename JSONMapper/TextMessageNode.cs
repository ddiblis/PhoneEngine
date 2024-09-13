using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class TextMessageNode : BaseNode {

        readonly List<string> Emojis = new() {
            "Emojis"
        };
        readonly List<string> Photos = new() {
            "Photos"
        };
        readonly List<string> Contacts = new() {
            "Contacts"
        };
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
        Sprite image;
        readonly Image ImageContainer;
        readonly Sprite emptyImage;

        public TextMessageNode NextTextNode;


        private readonly TextField TextMessageField;
        private readonly TextField AltContactField;
        private readonly DropdownField TypeDropDown;
        private readonly DropdownField DelayDropDown;
        private readonly DropdownField TendencyDropDown;
        private readonly DropdownField EmojiDropDown;
        private readonly DropdownField PhotoDropDown;
        private readonly DropdownField ContactDropDown;
        public Port ParentSubChapPort;
        public Port PrevText;
        public Port NextText;


        public TextMessageNode(GraphView graphView, int GivenType) : base(graphView) {
            Type = GivenType;

            string[] EmojiList = Directory.GetFiles("Assets/Resources/Images/Emojis","*.png");
            foreach (string Emoji in EmojiList) {
                Emojis.Add(Emoji[31..^4]);
            }
            string[] PhotoList = Directory.GetFiles("Assets/Resources/Images/Photos","*.png");
            foreach (string Photo in PhotoList) {
                Photos.Add(Photo[31..^4]);
            }
            string[] ContactList = Directory.GetFiles("Assets/Resources/Images/Headshots","*.png");
            foreach (string Contact in ContactList) {
                Contacts.Add(Contact[34..^4]);
            }

            title = "Text Message";

            ImageContainer = new Image();

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

            TendencyDropDown = new DropdownField("Tendency", TendencyOptions, 0);
            TendencyDropDown.RegisterValueChangedCallback(evt => Tendency = TendencyValues[TendencyOptions.FindIndex(x => x == evt.newValue)]);

            DelayDropDown = new DropdownField("Text Delay", DelayOptions, 0);
            DelayDropDown.RegisterValueChangedCallback(evt => TextDelay = DelayValues[DelayOptions.FindIndex(x => x == evt.newValue)]);


            EmojiDropDown = new DropdownField("Emojis", Emojis, 0);
            EmojiDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Emojis[Emojis.FindIndex(x => x == evt.newValue)];
                image = Resources.Load("Images/Emojis/" + evt.newValue, typeof(Sprite)) as Sprite;
                ImageContainer.sprite = image;
            });

            PhotoDropDown = new DropdownField("Photos", Photos, 0);
            PhotoDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Photos[Photos.FindIndex(x => x == evt.newValue)];
                image = Resources.Load("Images/Photos/" + evt.newValue, typeof(Sprite)) as Sprite;
                ImageContainer.sprite = image;
            });

            ContactDropDown = new DropdownField("Contacts", Contacts, 0);
            ContactDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Contacts[Contacts.FindIndex(x => x == evt.newValue)];
                image = Resources.Load("Images/Headshots/" + evt.newValue, typeof(Sprite)) as Sprite;
                ImageContainer.sprite = image;
            });

            TypeDropDown = new DropdownField("Text Type", TypeOptions, 0);
            TypeDropDown.RegisterValueChangedCallback(evt => {
                Type = TypeValues[TypeOptions.FindIndex(x => x == evt.newValue)];
                switch(Type) {
                    case (int)TypeOfText.recImage:
                        for (int i = 1; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.Add(PhotoDropDown);
                        Foldout.Add(TypeDropDown);
                        Foldout.Add(TendencyDropDown);
                        Foldout.Add(DelayDropDown);
                        EmojiDropDown.index = 0;
                        ContactDropDown.index = 0;
                        ImageContainer.sprite = emptyImage;
                        ImageContainer.style.width = 100;
                        ImageContainer.style.height = 150;
                        Foldout.Add(ImageContainer);                    
                    break;
                    case (int)TypeOfText.recEmoji:
                        for (int i = 1; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.Add(EmojiDropDown);
                        Foldout.Add(TypeDropDown);
                        Foldout.Add(TendencyDropDown);
                        Foldout.Add(DelayDropDown);
                        PhotoDropDown.index = 0;
                        ContactDropDown.index = 0;
                        ImageContainer.sprite = emptyImage;
                        ImageContainer.style.width = 50;
                        ImageContainer.style.height = 50;
                        Foldout.Add(ImageContainer);               
                    break;
                    case (int)TypeOfText.recContact:
                        for (int i = 1; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.Add(ContactDropDown);
                        Foldout.Add(TypeDropDown);
                        Foldout.Add(DelayDropDown);
                        PhotoDropDown.index = 0;
                        EmojiDropDown.index = 0;
                        ImageContainer.sprite = emptyImage;
                        ImageContainer.style.width = 50;
                        ImageContainer.style.height = 50;
                        Foldout.Add(ImageContainer);               
                    break;
                    default:
                        for (int i = 1; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.Add(TextMessageField);
                        Foldout.Add(TypeDropDown);
                        Foldout.Add(TendencyDropDown);
                        Foldout.Add(DelayDropDown);
                    break;
                }
            });

            AltContactField.AddClasses(
                "jm-node__subchap-altContact-textfield",
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
            EmojiDropDown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            PhotoDropDown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            ContactDropDown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );

            
            switch(Type) {
                case (int)TypeOfText.recImage:
                    Foldout.Add(AltContactField);
                    Foldout.Add(PhotoDropDown);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(TendencyDropDown);
                    Foldout.Add(DelayDropDown);
                    ImageContainer.style.width = 100;
                    ImageContainer.style.height = 150;
                    Foldout.Add(ImageContainer);
                break;
                case (int)TypeOfText.recEmoji:
                    Foldout.Add(AltContactField);
                    Foldout.Add(EmojiDropDown);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(TendencyDropDown);
                    Foldout.Add(DelayDropDown);
                    ImageContainer.style.width = 50;
                    ImageContainer.style.height = 50;
                    Foldout.Add(ImageContainer);
                break;
                case (int)TypeOfText.recContact:
                    Foldout.Add(AltContactField);
                    Foldout.Add(ContactDropDown);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(DelayDropDown);
                    ImageContainer.style.width = 50;
                    ImageContainer.style.height = 50;
                    Foldout.Add(ImageContainer);
                break;
                default:
                    Foldout.Add(AltContactField);
                    Foldout.Add(TextMessageField);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(TendencyDropDown);
                    Foldout.Add(DelayDropDown);
                break;
            }
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            int TypeIndex = TypeValues.FindIndex(x => x == Type);
            switch (Type) {
                case (int)TypeOfText.recImage:
                    int PhotoIndex = Photos.FindIndex(x => x == TextContent);
                    PhotoDropDown.value = Photos[PhotoIndex > 0 ? PhotoIndex : 0];
                    image = Resources.Load("Images/Photos/" + TextContent, typeof(Sprite)) as Sprite;
                    ImageContainer.sprite = image;
                break;
                case (int)TypeOfText.recEmoji:
                    int EmojiIndex = Emojis.FindIndex(x => x == TextContent);
                    EmojiDropDown.value = Emojis[EmojiIndex > 0 ? EmojiIndex : 0];
                    image = Resources.Load("Images/Emojis/" + TextContent, typeof(Sprite)) as Sprite;
                    ImageContainer.sprite = image;
                break;
                case (int)TypeOfText.recContact:
                    int ContactIndex = Contacts.FindIndex(x => x == TextContent);
                    ContactDropDown.value = Contacts[ContactIndex > 0 ? ContactIndex : 0];
                    image = Resources.Load("Images/Headshots/" + TextContent, typeof(Sprite)) as Sprite;
                    ImageContainer.sprite = image;
                break;
            }
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
            return new TextMessageNode(graphView, Type);
        }
    }
}
