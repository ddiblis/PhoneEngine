using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class ResponseNode : BaseNode {
        // readonly List<string> Emojis = new() {
        //     "Emojis"
        // };
        // readonly List<string> Photos = new() {
        //     "Photos"
        // };
        readonly List<string> TypeOptions = new() { "Type of Text", "Sent Text", "Sent Image", "Sent Emoji" };
        readonly List<int> TypeValues = new() {
            0, 0, 2, 4
        };

        public bool RespTree = false;
        public string TextContent;
        public int SubChapNum;
        public int Type;

        Sprite image;
        readonly Image ImageContainer;
        readonly Sprite emptyImage;

        public SubChapNode NextSubChap;

        private readonly TextField TextMessageField;
        private readonly Toggle ResponseTreeToggle;
        private readonly DropdownField TypeDropDown;
        private readonly DropdownField EmojiDropDown;
        private readonly DropdownField PhotoDropDown;
        public Port ParentSubChapPort;
        public Port NextSubChapterNodePort;

        public ResponseNode(GraphView graphView, int GivenType) : base(graphView) {
            Type = GivenType;

            // string[] EmojiList = Directory.GetFiles("Assets/Resources/Images/Emojis","*.png");
            // foreach (string Emoji in EmojiList) {
            //     Emojis.Add(Emoji[31..^4]);
            // }
            // string[] PhotoList = Directory.GetFiles("Assets/Resources/Images/Photos","*.png");
            // foreach (string Photo in PhotoList) {
            //     Photos.Add(Photo[31..^4]);
            // }
            CustomLists Lists = new();

            title = "Response";

            ImageContainer = new Image();

            var ParentSubChapPortContainer = new VisualElement();

            ParentSubChapPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(SubChapNode));
            ParentSubChapPort.portName = "Parent SubChap";
            ParentSubChapPortContainer.Add(ParentSubChapPort);

            var NextSubChapPortContainer = new VisualElement();

            NextSubChapterNodePort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(SubChapNode));
            NextSubChapterNodePort.portName = "Next SubChap";
            NextSubChapPortContainer.Add(NextSubChapterNodePort);

            var CustomDataContainer = new VisualElement();
            CustomDataContainer.AddToClassList("jm-node__custom-data-container");

            var Foldout = new Foldout() { text = "Response Content" };

            ResponseTreeToggle = new Toggle("Response Tree") { value = RespTree };
            ResponseTreeToggle.RegisterValueChangedCallback(evt => RespTree = evt.newValue);

            TextMessageField = new TextField("Response Text", 256, false, false, 'a') { value = TextContent };
            TextMessageField.RegisterValueChangedCallback(evt => TextContent = evt.newValue);

            EmojiDropDown = new DropdownField("Emojis", Lists.Emojis, 0);
            EmojiDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Lists.Emojis[Lists.Emojis.FindIndex(x => x == evt.newValue)];
                image = Resources.Load("Images/Emojis/" + evt.newValue, typeof(Sprite)) as Sprite;
                ImageContainer.sprite = image;
            });

            PhotoDropDown = new DropdownField("Photos", Lists.Photos, 0);
            PhotoDropDown.RegisterValueChangedCallback(evt => {
                TextContent = Lists.Photos[Lists.Photos.FindIndex(x => x == evt.newValue)];
                image = Resources.Load("Images/Photos/" + evt.newValue, typeof(Sprite)) as Sprite;
                ImageContainer.sprite = image;
            });
            
            TypeDropDown = new DropdownField("Text Type", TypeOptions, 0);
            TypeDropDown.RegisterValueChangedCallback(evt => {
                Type = TypeValues[TypeOptions.FindIndex(x => x == evt.newValue)];
                switch(Type) {
                    case (int)TypeOfText.sentImage:
                        for (int i = 0; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.RemoveAt(0);
                        Foldout.Add(PhotoDropDown);
                        Foldout.Add(TypeDropDown);
                        EmojiDropDown.index = 0;
                        ImageContainer.sprite = emptyImage;
                        ImageContainer.style.width = 100;
                        ImageContainer.style.height = 150;
                        Foldout.Add(ImageContainer);  
                    break;
                    case (int)TypeOfText.sentEmoji:
                        for (int i = 0; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.RemoveAt(0);
                        Foldout.Add(EmojiDropDown);
                        Foldout.Add(TypeDropDown);
                        PhotoDropDown.index = 0;
                        ImageContainer.sprite = emptyImage;
                        ImageContainer.style.width = 50;
                        ImageContainer.style.height = 50;
                        Foldout.Add(ImageContainer);
                    break;
                    default:
                        for (int i = 0; i < Foldout.childCount; i++) {
                            Foldout.RemoveAt(i);
                        }
                        Foldout.RemoveAt(0);
                        Foldout.Add(ResponseTreeToggle);
                        Foldout.Add(TextMessageField);
                        Foldout.Add(TypeDropDown);
                    break;
                }
            });



            TextMessageField.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            ResponseTreeToggle.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            TypeDropDown.AddClasses(
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
            ParentSubChapPortContainer.AddClasses(
                "jm-node__custom-data-container"
            );
            NextSubChapPortContainer.AddClasses(
                "jm-node__custom-data-container"
            );
            ParentSubChapPort.AddClasses(
                "jm-node__ParentResponsePort"
            );
            NextSubChapterNodePort.AddClasses(
                "jm-node__NextSubChapterNodePort"
            );

            Insert(0, ParentSubChapPortContainer);
            switch(Type) {
                case (int)TypeOfText.sentImage:
                    Foldout.Add(PhotoDropDown);
                    Foldout.Add(TypeDropDown);
                    ImageContainer.style.width = 100;
                    ImageContainer.style.height = 150;
                    Foldout.Add(ImageContainer);
                break;
                case (int)TypeOfText.sentEmoji:
                    Foldout.Add(EmojiDropDown);
                    Foldout.Add(TypeDropDown);
                    ImageContainer.style.width = 50;
                    ImageContainer.style.height = 50;
                    Foldout.Add(ImageContainer);
                break;
                default:
                    Foldout.Add(ResponseTreeToggle);
                    Foldout.Add(TextMessageField);
                    Foldout.Add(TypeDropDown);
                break;
            }
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);
            CustomDataContainer.Add(NextSubChapPortContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            int TypeIndex = TypeValues.FindIndex(x => x == Type);
            CustomLists Lists = new();
            switch (Type) {
                case (int)TypeOfText.sentImage:
                    int PhotoIndex = Lists.Photos.FindIndex(x => x == TextContent);
                    PhotoDropDown.value = Lists.Photos[PhotoIndex > 0 ? PhotoIndex : 0];
                    image = Resources.Load("Images/Photos/" + TextContent, typeof(Sprite)) as Sprite;
                    ImageContainer.sprite = image;
                break;
                case (int)TypeOfText.sentEmoji:
                    int EmojiIndex = Lists.Emojis.FindIndex(x => x == TextContent);
                    EmojiDropDown.value = Lists.Emojis[EmojiIndex > 0 ? EmojiIndex : 0];
                    image = Resources.Load("Images/Emojis/" + TextContent, typeof(Sprite)) as Sprite;
                    ImageContainer.sprite = image;
                break;
                default:
                    ResponseTreeToggle.value = RespTree;
                break;
            }
            TextMessageField.value = TextContent;
            TypeDropDown.value = TypeOptions[TypeIndex > 0 ? TypeIndex : 1];
        }

        public ResponseData ToResponseNodeData() {
            Rect rect = GetPosition();
            return new ResponseData {
                RespTree = RespTree,
                TextContent = TextContent,
                SubChapNum = SubChapNum,
                Type = Type,
                location = new Location {
                    x = rect.x,
                    y = rect.y,
                    Width = rect.width,
                    Height = rect.height
                }
            };
        }

        public Response ToResponseData() {
            return new Response {
                RespTree = RespTree,
                TextContent = TextContent,
                SubChapNum = SubChapNum,
                Type = Type
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new ResponseNode(graphView, Type);
        }
    }
}
