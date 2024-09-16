using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System.Text;
using System;
using UnityEngine.UI;
using System.IO;

namespace JSONMapper {
    public class SubChapNode : BaseNode {  
        public int SubChapIndex;
        public string Contact;
        public string UnlockIPAccount;
        public List<int> UnlockPosts;
        public TextMessageNode FirstText;
        public List<ResponseNode> Responses = new();

        private readonly TextField UnlockListTextField;
        private readonly DropdownField ContactDropDown;
        private readonly DropdownField UnlockIPAccountDropDown;
        public Port ParentChapterPort;
        public Port FirstTextPort;
        public Port ResponsesPort;
        public Port ParentResponsePort;


        public SubChapNode(GraphView graphView) : base(graphView) {
        
            CustomLists Lists = new();

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

            UnlockListTextField = new TextField("Unlock Posts List");
            UnlockListTextField.RegisterValueChangedCallback(evt => {
                var UnlockList = (from Match m in Regex.Matches(evt.newValue, @"\d+") select m.Value).ToList();
                UnlockPosts = UnlockList.ConvertAll(int.Parse);
            });

            ContactDropDown = new DropdownField("Contacts", Lists.Contacts, 0);
            ContactDropDown.RegisterValueChangedCallback(evt => {
                Contact = Lists.Contacts[Lists.Contacts.FindIndex(x => x == evt.newValue)];
            });

            UnlockIPAccountDropDown = new DropdownField("InstaPosts Account", Lists.Contacts, 0);
            UnlockIPAccountDropDown.RegisterValueChangedCallback(evt => {
                UnlockIPAccount = Lists.Contacts[Lists.Contacts.FindIndex(x => x == evt.newValue)];
            });

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
            Foldout.Add(ContactDropDown);
            Foldout.Add(UnlockIPAccountDropDown);
            Foldout.Add(UnlockListTextField);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);
            CustomDataContainer.Add(ResponsePortContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            CustomLists Lists = new();
            int ContactIndex = Lists.Contacts.FindIndex(x => x == Contact);
            ContactDropDown.value = Lists.Contacts[ContactIndex > 0 ? ContactIndex : 0];

            int IPAccountIndex = Lists.Contacts.FindIndex(x => x == UnlockIPAccount);
            UnlockIPAccountDropDown.value = Lists.Contacts[IPAccountIndex > 0 ? IPAccountIndex : 0];
            if (UnlockPosts != null && UnlockPosts.Count > 0) {
                UnlockListTextField.value = string.Join( ",", UnlockPosts.ToArray());
            } else {  
                UnlockListTextField.value = "";
            }
        }

        public SubChapData ToSubChapNodeData() {
            Rect rect = GetPosition();
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
            CustomLists Lists = new();
            int IndexOfContact = Lists.Contacts.FindIndex(x => x == Contact);
            int IPAccountIndex = Lists.Contacts.FindIndex(x => x == UnlockIPAccount);
            return new SubChapData {
                Contact = IndexOfContact > 0 ? Contact : "",
                UnlockInstaPostsAccount = IPAccountIndex > 0 ? UnlockIPAccount : "",
                UnlockPosts = UnlockPosts,
                TextList = TextList.ConvertAll(textNode => textNode.ToTextMessageNodeData()),
                Responses = Responses.ConvertAll(responseNode => responseNode.ToResponseNodeData()),
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
                Contact = Contact != "Contacts" ? Contact : "",
                UnlockInstaPostsAccount = UnlockIPAccount != "Contacts" ? UnlockIPAccount : "",
                UnlockPosts = UnlockPosts,
                TextList = TextList.ConvertAll(textNode => textNode.ToTextMessageData()),
                Responses = Responses.ConvertAll(responseNode => responseNode.ToResponseData())
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new SubChapNode(graphView);
        }
    }
}
