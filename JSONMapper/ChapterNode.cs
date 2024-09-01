using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;

namespace JSONMapper {
    public class ChapterNode : BaseNode {
        public bool allowMidrolls;
        private readonly Toggle allowMidrollsToggle;
        public Port SubChaptersPort;
        public List<SubChapNode> SubChaps = new();

        public ChapterNode(GraphView graphView) : base(graphView) {
            title = "Chapter";

            SubChaptersPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(SubChapNode));
            SubChaptersPort.portName = "SubChapters";
            inputContainer.Add(SubChaptersPort);

            var CustomDataContainer = new VisualElement();
            CustomDataContainer.AddToClassList("jm-node__custom-data-container");

            var Foldout = new Foldout() { text = "Chapter Content" };

            allowMidrollsToggle = new Toggle("Allow Midrolls") { value = allowMidrolls };
            allowMidrollsToggle.RegisterValueChangedCallback(evt => allowMidrolls = evt.newValue);

            allowMidrollsToggle.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );

            Foldout.Add(allowMidrollsToggle);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            allowMidrollsToggle.value = allowMidrolls;
        }

        public ChapterData ToChapterNodeData() {
            Rect rect = this.GetPosition();
            return new ChapterData {
                AllowMidrolls = this.allowMidrolls,
                SubChaps = this.SubChaps.ConvertAll(subChapNode => subChapNode.ToSubChapNodeData()),
                location = new Location {
                    x = rect.x,
                    y = rect.y,
                    Width = rect.width,
                    Height = rect.height
                }
            };
        }
        
        public Chapter ToChapterData() {
            return new Chapter {
                AllowMidrolls = this.allowMidrolls,
                SubChaps = this.SubChaps.ConvertAll(subChapNode => subChapNode.ToSubChapData())
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new ChapterNode(graphView);
        }
    }
}

