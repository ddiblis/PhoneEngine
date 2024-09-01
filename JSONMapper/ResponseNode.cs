using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class ResponseNode : BaseNode {
        readonly List<string> TypeOptions = new() { "Type of Text", "Sent Text 0", "Sent Image 2", "Sent Emoji 4" };

        public bool RespTree = false;
        public string TextContent;
        public int SubChapNum;
        public int Type;

        private readonly TextField TextMessageField;
        private readonly Toggle ResponseTreeToggle;
        private readonly IntegerField NextSubChapField;
        private readonly DropdownField TypeDropDown;
        public Port ParentSubChapPort;
        public Port NextSubChapterNodePort;

        public ResponseNode(GraphView graphView) : base(graphView) {
            title = "Response";

            var ParentSubChapPortContainer = new VisualElement();

            ParentSubChapPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(SubChapNode));
            ParentSubChapPort.portName = "Parent SubChap";
            ParentSubChapPortContainer.Add(ParentSubChapPort);

            var NextSubChapPortContainer = new VisualElement();

            NextSubChapterNodePort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(SubChapNode));
            NextSubChapterNodePort.portName = "Next SubChap";
            NextSubChapPortContainer.Add(NextSubChapterNodePort);

            var CustomDataContainer = new VisualElement();
            CustomDataContainer.AddToClassList("jm-node__custom-data-container");

            var Foldout = new Foldout() { text = "Response Content" };

            TextMessageField = new TextField("Response Text", 256, false, false, 'a') { value = TextContent };
            TextMessageField.RegisterValueChangedCallback(evt => TextContent = evt.newValue);
            
            TypeDropDown = new DropdownField("Text Type", TypeOptions, 0);
            TypeDropDown.RegisterValueChangedCallback(evt => Type = int.Parse(Regex.Match(evt.newValue, @"\d+").Value));

            NextSubChapField = new IntegerField("Next Sub Chapter") { value = SubChapNum };
            NextSubChapField.RegisterValueChangedCallback(evt => SubChapNum = evt.newValue);

            ResponseTreeToggle = new Toggle("Response Tree") { value = RespTree };
            ResponseTreeToggle.RegisterValueChangedCallback(evt => RespTree = evt.newValue);

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
            NextSubChapField.AddClasses(
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
            Foldout.Add(ResponseTreeToggle);
            Foldout.Add(TextMessageField);
            Foldout.Add(TypeDropDown);
            Foldout.Add(NextSubChapField);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);
            CustomDataContainer.Add(NextSubChapPortContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            int TypeIndex = TypeOptions.FindIndex(x => x.Contains("" + Type));
            TextMessageField.value = TextContent;
            ResponseTreeToggle.value = RespTree;
            NextSubChapField.value = SubChapNum;
            TypeDropDown.value = TypeOptions[TypeIndex >= 0 ? TypeIndex : 0];
        }

        public ResponseData ToResponseNodeData() {
            Rect rect = this.GetPosition();
            return new ResponseData {
                RespTree = this.RespTree,
                TextContent = this.TextContent,
                SubChapNum = this.SubChapNum,
                Type = this.Type,
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
                RespTree = this.RespTree,
                TextContent = this.TextContent,
                SubChapNum = this.SubChapNum,
                Type = this.Type
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new ResponseNode(graphView);
        }
    }
}
