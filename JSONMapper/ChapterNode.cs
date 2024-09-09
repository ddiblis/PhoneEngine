using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace JSONMapper {
    public class ChapterNode : BaseNode {
        public bool allowMidrolls;
        public int Checkpoint;


        private readonly Toggle allowMidrollsToggle;
        private readonly DropdownField CheckpointDropdown;

        readonly List<string> CheckpointOptions = new() {
            "Checkpoint Options", "During trip 0"
        };
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

            CheckpointDropdown = new DropdownField("Storypoint", CheckpointOptions, 0);
            CheckpointDropdown.RegisterValueChangedCallback(evt => Checkpoint = int.Parse(Regex.Match(evt.newValue, @"\d+").Value));
            

            allowMidrollsToggle = new Toggle("Allow Midrolls") { value = allowMidrolls };
            allowMidrollsToggle.RegisterValueChangedCallback(evt => allowMidrolls = evt.newValue);

            allowMidrollsToggle.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );
            CheckpointDropdown.AddClasses(
                "jm-node__textfield",
                "jm-node__quote-textfield"
            );

            Foldout.Add(CheckpointDropdown);
            Foldout.Add(allowMidrollsToggle);
            CustomDataContainer.Add(Foldout);
            extensionContainer.Add(CustomDataContainer);

            RefreshExpandedState();
            RefreshPorts();
        }

        public void UpdateFields() {
            int CheckpointIndex = CheckpointOptions.FindIndex(x => x.Contains("" + Checkpoint));
            CheckpointDropdown.value = CheckpointOptions[CheckpointIndex >= 0 ? CheckpointIndex : 0];
            allowMidrollsToggle.value = allowMidrolls;
        }

        public ChapterData ToChapterNodeData() {
            Rect rect = this.GetPosition();
            return new ChapterData {
                AllowMidrolls = this.allowMidrolls,
                StoryCheckpoint = this.Checkpoint,
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
                StoryCheckpoint = this.Checkpoint,
                SubChaps = this.SubChaps.ConvertAll(subChapNode => subChapNode.ToSubChapData())
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new ChapterNode(graphView);
        }
    }
}

