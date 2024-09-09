using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public class TextMessageNode : BaseNode {
        readonly List<string> TypeOptions = new() {
            "Type of Text", "Recieved Text 1", "Recieved Image 3", "Recieved Emoji 5", "Chapter end 6", "Recieved Contact 7", "Indicate Time 8"
        };
        readonly List<string> DelayOptions = new() {
            "Delay Options", "Almost Instant 0.16", "Very Fast 0.57", "Fast 1.1", "Medium 2.1", "Slow 2.5", "Very slow 3.5", "Dramatic Pause 5.1"
        };

        readonly List<string> TendencyOptions = new() {
            "Tendency Options", "Neutral 0", "Submissive 1", "Dominant 2"
        };

        public int Type;
        public int Tendency;
        public string TextContent;
        public float TextDelay;

        private readonly TextField TextMessageField;
        private readonly DropdownField TypeDropDown;
        private readonly DropdownField DelayDropDown;
        private readonly DropdownField TendencyDropDown;
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

            TendencyDropDown = new DropdownField("Tendency", TendencyOptions, 0);
            TendencyDropDown.RegisterValueChangedCallback(evt => Tendency = int.Parse(Regex.Match(evt.newValue, @"\d+").Value));

            DelayDropDown = new DropdownField("Text Delay", DelayOptions, 0);
            DelayDropDown.RegisterValueChangedCallback(evt => TextDelay = float.Parse(Regex.Match(evt.newValue, @"\d\.\d{1,2}").Value));

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
            int TypeIndex = TypeOptions.FindIndex(x => x.Contains("" + Type));
            int DelayIndex = DelayOptions.FindIndex(x => x.Contains("" + TextDelay));
            int TendencyIndex = TendencyOptions.FindIndex(x => x.Contains("" + Tendency));
            TextMessageField.value = TextContent;
            TendencyDropDown.value = TendencyOptions[TendencyIndex >= 0 ? TendencyIndex : 0];
            TypeDropDown.value = TypeOptions[TypeIndex >= 0 ? TypeIndex : 0];
            DelayDropDown.value = DelayOptions[DelayIndex >= 0 ? DelayIndex : 0];
        }

        public TextMessageData ToTextMessageNodeData() {
            Rect rect = this.GetPosition();
            return new TextMessageData {
                Type = this.Type,
                TextContent = this.TextContent,
                TextDelay = this.TextDelay,
                Tendency = this.Tendency,
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
                TextDelay = this.TextDelay,
                Tendency = this.Tendency
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new TextMessageNode(graphView);
        }
    }
}
