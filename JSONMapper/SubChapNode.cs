using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System.Text;
using System;
using UnityEngine.UI;

namespace JSONMapper {
    public class SubChapNode : BaseNode {  
        public string Contact;
        public string UnlockInstaPostsAccount;
        public List<int> UnlockPosts;
        public TextMessageNode FirstText;
        public List<ResponseNode> Responses = new();


        private readonly TextField ContactTextField;
        private readonly TextField UnlockInstaPostsAccountTextField;
        private readonly TextField UnlockListTextField;
        public Port ParentChapterPort;
        public Port FirstTextPort;
        public Port ResponsesPort;
        public Port ParentResponsePort;


        public SubChapNode(GraphView graphView) : base(graphView) {
            title = "SubChapter";

            ParentChapterPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(ChapterNode));
            ParentChapterPort.portName = "Parent Chapter";
            inputContainer.Add(ParentChapterPort);

            FirstTextPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(TextMessageNode));
            FirstTextPort.portName = "Start of Texts";
            outputContainer.Add(FirstTextPort);

            var ParentResponsePortContainer = new VisualElement();

            ParentResponsePort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(ChapterNode));
            ParentResponsePort.portName = "Parent Response";
            ParentResponsePortContainer.Add(ParentResponsePort);

            var ResponsePortContainer = new VisualElement();

            ResponsesPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(ResponseNode));
            ResponsesPort.portName = "Responses";
            ResponsePortContainer.Add(ResponsesPort);

            var CustomDataContainer = new VisualElement();
            CustomDataContainer.AddToClassList("jm-node__custom-data-container");

            var Foldout = new Foldout() { text = "Sub Chapter Content" };

            ContactTextField = new TextField("Contact") { value = Contact };
            ContactTextField.RegisterValueChangedCallback(evt => Contact = evt.newValue);

            UnlockInstaPostsAccountTextField = new TextField("Unlock Insta Account") { value = UnlockInstaPostsAccount };
            UnlockInstaPostsAccountTextField.RegisterValueChangedCallback(evt => UnlockInstaPostsAccount = evt.newValue);

            UnlockListTextField = new TextField("Unlock Posts List");
            UnlockListTextField.RegisterValueChangedCallback(evt => {
                var UnlockList = (from Match m in Regex.Matches(evt.newValue, @"\d+") select m.Value).ToList();
                UnlockPosts = UnlockList.ConvertAll(int.Parse);
            });

            ContactTextField.AddClasses(
                "jm-node__subchap-textfield",
                "jm-node__subchap-quote-textfield"
            );
            UnlockInstaPostsAccountTextField.AddClasses(
                "jm-node__subchap-textfield",
                "jm-node__subchap-quote-textfield"
            );
            UnlockListTextField.AddClasses(
                "jm-node__subchap-textfield",
                "jm-node__subchap-quote-textfield"
            );
            ParentResponsePortContainer.AddClasses(
                "jm-node__custom-data-container"
            );
            ParentResponsePort.AddClasses(
                "jm-node__ParentResponsePort"
            );
            ResponsePortContainer.AddClasses(
                "jm-node__NextSubChapterNodePort"
            );


            Insert(0, ParentResponsePortContainer);
            Foldout.Add(ContactTextField);
            Foldout.Add(UnlockInstaPostsAccountTextField);
            Foldout.Add(UnlockListTextField);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);
            CustomDataContainer.Add(ResponsePortContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            ContactTextField.value = Contact;
            UnlockInstaPostsAccountTextField.value = UnlockInstaPostsAccount;
            if (UnlockPosts.Count > 0) {
                UnlockListTextField.value = string.Join( ",", UnlockPosts.ToArray());
            } else {
                UnlockListTextField.value = "";
            }
        }

        public SubChapData ToSubChapNodeData() {
            Rect rect = this.GetPosition();
            List<TextMessageNode> TextList = new();
            if (FirstText != null) {
                TextList.Add(FirstText);
                TextMessageNode CurrNode = FirstText;
                bool nextTextAvaliable = true;
                while (nextTextAvaliable) {
                    if (CurrNode.NextTextNode != null) {
                        CurrNode = CurrNode.NextTextNode;
                        TextList.Add(CurrNode);
                    } else {
                        nextTextAvaliable = false;
                    }
                }
            }
            return new SubChapData {
                Contact = this.Contact,
                UnlockInstaPostsAccount = this.UnlockInstaPostsAccount,
                UnlockPosts = this.UnlockPosts,
                TextList = TextList.ConvertAll(textNode => textNode.ToTextMessageNodeData()),
                Responses = this.Responses.ConvertAll(responseNode => responseNode.ToResponseNodeData()),
                location = new Location {
                    x = rect.x,
                    y = rect.y,
                    Width = rect.width,
                    Height = rect.height
                }
            };
        }

        public SubChap ToSubChapData() {
            List<TextMessageNode> TextList = new();
            if (FirstText != null) {
                TextList.Add(FirstText);
                TextMessageNode CurrNode = FirstText;
                bool nextTextAvaliable = true;
                while (nextTextAvaliable) {
                    if (CurrNode.NextTextNode != null) {
                        CurrNode = CurrNode.NextTextNode;
                        TextList.Add(CurrNode);
                    } else {
                        nextTextAvaliable = false;
                    }
                }
            }
            return new SubChap {
                Contact = this.Contact,
                UnlockInstaPostsAccount = this.UnlockInstaPostsAccount,
                UnlockPosts = this.UnlockPosts,
                TextList = TextList.ConvertAll(textNode => textNode.ToTextMessageData()),
                Responses = this.Responses.ConvertAll(responseNode => responseNode.ToResponseData())
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new SubChapNode(graphView);
        }
    }
}
