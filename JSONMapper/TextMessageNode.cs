using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class TextMessageNode : BaseNode {
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
            "Tendency Options", "Neutral"
        };
        readonly List<int> TendencyValues = new() {
            0, 0
        };

        public string AltContact;
        public int Type;
        public int Tendency;
        public string TextContent;
        public float TextDelay;
        public TextStats Stats = new();
        Sprite image;
        readonly Image ImageContainer;
        readonly Sprite emptyImage;

        public TextMessageNode NextTextNode;


        private readonly TextField TextMessageField;
        private readonly DropdownField AltContactDropDown;
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

            CustomLists Lists = new();

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

            TextMessageField = new TextField("Text message") { value = TextContent };
            TextMessageField.RegisterValueChangedCallback(evt => TextContent = evt.newValue);

            TendencyDropDown = new DropdownField("Tendency", TendencyOptions, 0);
            TendencyDropDown.RegisterValueChangedCallback(evt => Tendency = TendencyValues[TendencyOptions.FindIndex(x => x == evt.newValue)]);

            DelayDropDown = new DropdownField("Text Delay", DelayOptions, 0);
            DelayDropDown.RegisterValueChangedCallback(evt => TextDelay = DelayValues[DelayOptions.FindIndex(x => x == evt.newValue)]);


            EmojiDropDown = new DropdownField("Emojis", Lists.Emojis, 0);
            EmojiDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Lists.Emojis[Lists.Emojis.FindIndex(x => x == evt.newValue)];
                image = Resources.Load<Sprite>($"Images/Emojis/{evt.newValue}");
                ImageContainer.sprite = image;
            });

            PhotoDropDown = new DropdownField("Photos", Lists.Photos, 0);
            PhotoDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Lists.Photos[Lists.Photos.FindIndex(x => x == evt.newValue)];
                image = Resources.Load<Sprite>($"Images/Photos/{evt.newValue}");
            });

            ContactDropDown = new DropdownField("Contacts", Lists.Contacts, 0);
            ContactDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Lists.Contacts[Lists.Contacts.FindIndex(x => x == evt.newValue)];
                image = Resources.Load<Sprite>($"Images/Headshots/{evt.newValue}");
                ImageContainer.sprite = image;
            });

            AltContactDropDown = new DropdownField("Alt Contact", Lists.Contacts, 0);
            AltContactDropDown.RegisterValueChangedCallback(evt => AltContact = Lists.Contacts[Lists.Contacts.FindIndex(x => x == evt.newValue)]);

            // Stats toggles

            var StatsFoldout = new Foldout() { text = "Stats" };

            // Add Stat Toggle here, if stat is active it'll check in game if the player chose to have that stat active to display that specific text
            // E.G.
            // SadToggle = new Toggle("Sad") { value = Stats.Sad };
            // Sad.RegisterValueChangedCallback(evt => Stats.Sad = evt.newValue);

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

            // StatsFoldout.Add(HumilationToggle);
            switch(Type) {
                case (int)TypeOfText.recImage:
                    Foldout.Add(AltContactDropDown);
                    Foldout.Add(PhotoDropDown);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(TendencyDropDown);
                    Foldout.Add(DelayDropDown);
                    ImageContainer.style.width = 100;
                    ImageContainer.style.height = 150;
                    Foldout.Add(ImageContainer);
                break;
                case (int)TypeOfText.recEmoji:
                    Foldout.Add(AltContactDropDown);
                    Foldout.Add(EmojiDropDown);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(TendencyDropDown);
                    Foldout.Add(DelayDropDown);
                    ImageContainer.style.width = 50;
                    ImageContainer.style.height = 50;
                    Foldout.Add(ImageContainer);
                break;
                case (int)TypeOfText.recContact:
                    Foldout.Add(AltContactDropDown);
                    Foldout.Add(ContactDropDown);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(DelayDropDown);
                    ImageContainer.style.width = 50;
                    ImageContainer.style.height = 50;
                    Foldout.Add(ImageContainer);
                break;
                default:
                    Foldout.Add(AltContactDropDown);
                    Foldout.Add(TextMessageField);
                    Foldout.Add(TypeDropDown);
                    Foldout.Add(TendencyDropDown);
                    Foldout.Add(DelayDropDown);
                break;
            }
            CustomDataContainer.Add(StatsFoldout);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            CustomLists Lists = new();
            int TypeIndex = TypeValues.FindIndex(x => x == Type);

            // Map TypeOfText to respective lists and folder paths
            Dictionary<TypeOfText, (List<string> list, string path)> typeMap = new() {
                { TypeOfText.recImage, (Lists.Photos, "Images/Photos/") },
                { TypeOfText.recEmoji, (Lists.Emojis, "Images/Emojis/") },
                { TypeOfText.recContact, (Lists.Contacts, "Images/Headshots/") }
            };

            // Update dropdowns and image if a matching type exists
            if (typeMap.TryGetValue((TypeOfText)Type, out var entry)) {
                int contentIndex = entry.list.FindIndex(x => x == TextContent);
                var dropDown = Type switch {
                    (int)TypeOfText.recImage => PhotoDropDown,
                    (int)TypeOfText.recEmoji => EmojiDropDown,
                    (int)TypeOfText.recContact => ContactDropDown,
                    _ => null
                };

                if (dropDown != null) {
                    dropDown.value = entry.list[contentIndex > 0 ? contentIndex : 0];
                }

                image = Resources.Load<Sprite>(entry.path + TextContent);
                ImageContainer.sprite = image;
            }

            // Update other dropdowns and fields
            int DelayIndex = DelayValues.FindIndex(x => x == TextDelay);
            int TendencyIndex = TendencyValues.FindIndex(x => x == Tendency);
            int AltContactIndex = Lists.Contacts.FindIndex(x => x == AltContact);

            AltContactDropDown.value = Lists.Contacts[AltContactIndex > 0 ? AltContactIndex : 0];
            TextMessageField.value = TextContent;
            TendencyDropDown.value = TendencyOptions[TendencyIndex > 0 ? TendencyIndex : 1];
            TypeDropDown.value = TypeOptions[TypeIndex > 0 ? TypeIndex : 1];
            DelayDropDown.value = DelayOptions[DelayIndex > 0 ? DelayIndex : 1];

            // Stat toggles
            // SadToggle.value = Stats.Sad;
        }
        public TextMessageData ToTextMessageNodeData() {
            Rect rect = GetPosition();
            return new TextMessageData {
                Type = Type,
                Stats = Stats,
                AltContact = AltContact != "Contacts" ? AltContact : "",
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
            // when saving create a list in the db if it's not already there and add to it the images seen within this chapter
            return new TextMessage {
                Type = Type,
                Stats = Stats,
                AltContact = AltContact != "Contacts" ? AltContact : "",
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
