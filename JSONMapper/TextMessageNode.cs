using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class TextMessageNode : BaseNode {
        readonly List<string> TypeOptions = new() {
            "Type of Text", "Recieved Text 1", "Recieved Image 3", "Recieved Emoji 5", "Chapter end 6"
        };
        readonly List<string> DelayOptions = new() {
            "Delay Options", "Very Fast 0.5", "Fast 1.0", "Medium 2.0", "Slow 2.5", "Very slow 3.5", "Dramatic Pause 5.0"
        };

        public int Type;
        public string TextContent;
        public float TextDelay;

        private readonly TextField TextMessageField;
        private readonly DropdownField TypeDropDown;
        private readonly DropdownField DelayDropDown;
        public Port ParentSubChapPort;


        public TextMessageNode(GraphView graphView) : base(graphView) {

            title = "TextMessage";

            ParentSubChapPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(SubChapNode));
            ParentSubChapPort.portName = "Parent SubChap";
            inputContainer.Add(ParentSubChapPort);

            var CustomDataContainer = new VisualElement();
            CustomDataContainer.AddToClassList("jm-node__custom-data-container");

            var Foldout = new Foldout() { text = "Text Message Content" };

            TextMessageField = new TextField("Text message") { value = TextContent };
            TextMessageField.RegisterValueChangedCallback(evt => TextContent = evt.newValue);

            TypeDropDown = new DropdownField("Text Type", TypeOptions, 0);
            TypeDropDown.RegisterValueChangedCallback(evt => Type = int.Parse(Regex.Match(evt.newValue, @"\d+").Value));

            DelayDropDown = new DropdownField("Text Delay", DelayOptions, 0);
            DelayDropDown.RegisterValueChangedCallback(evt => TextDelay = float.Parse(Regex.Match(evt.newValue, @"\d+[.][^2]").Value));

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

            Foldout.Add(TextMessageField);
            Foldout.Add(TypeDropDown);
            Foldout.Add(DelayDropDown);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            int TypeIndex = TypeOptions.FindIndex(x => x.Contains("" + Type));
            int DelayIndex = DelayOptions.FindIndex(x => x.Contains("" + TextDelay));
            TextMessageField.value = TextContent;
            TypeDropDown.value = TypeOptions[TypeIndex >= 0 ? TypeIndex : 0];
            DelayDropDown.value = DelayOptions[DelayIndex >= 0 ? DelayIndex : 0];
        }

        public TextMessageData ToTextMessageNodeData() {
            Rect rect = this.GetPosition();
            return new TextMessageData {
                Type = this.Type,
                TextContent = this.TextContent,
                TextDelay = this.TextDelay,
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
                Type = this.Type,
                TextContent = this.TextContent,
                TextDelay = this.TextDelay
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new TextMessageNode(graphView);
        }
    }
}
