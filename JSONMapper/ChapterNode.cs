using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

namespace JSONMapper {
    public class ChapterNode : BaseNode {
        public bool allowMidrolls;
        public int Checkpoint;

        public SubChapNode FirstSubChap;

        private readonly Toggle allowMidrollsToggle;
        private readonly DropdownField CheckpointDropdown;

        List<SubChapNode> SubChaps = new();


        readonly List<string> CheckpointOptions = new() {
            "Checkpoint Options", "During trip 0"
        };
        public Port SubChaptersPort;

        public ChapterNode(GraphView graphView) : base(graphView) {
            title = "Chapter";

            SubChaptersPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(SubChapNode));
            SubChaptersPort.portName = "First SubChapter";
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

        private void LevelOrder(SubChapNode rootNode) {
            if (rootNode == null) return;
            Queue<SubChapNode> queue = new();
            queue.Enqueue(rootNode);
            while (queue.Count > 0) {
                SubChapNode node = queue.Dequeue();
                int indexOfSubChap = SubChaps.FindIndex(x => x == node);
                if (indexOfSubChap < 0) {
                    SubChaps.Add(node);
                } 

                if (
                    0 < node.Responses.Count 
                    && node.Responses[0].NextSubChap != null
                    && !queue.Contains(node.Responses[0].NextSubChap)
                    ) queue.Enqueue(node.Responses[0].NextSubChap);
                
                if (
                    1 < node.Responses.Count 
                    && node.Responses[1].NextSubChap != null
                    && !queue.Contains(node.Responses[1].NextSubChap)
                    ) queue.Enqueue(node.Responses[1].NextSubChap);
                
            }
            foreach(SubChapNode subChap in SubChaps) {
                if (0 < subChap.Responses.Count) {
                    int indexOfSubChap = SubChaps.FindIndex(x => x == subChap.Responses[0].NextSubChap);
                    subChap.Responses[0].SubChapNum = indexOfSubChap;
                }
                if (1 < subChap.Responses.Count) {
                    int indexOfSubChap = SubChaps.FindIndex(x => x == subChap.Responses[1].NextSubChap);
                    subChap.Responses[1].SubChapNum = indexOfSubChap;
                }
            }
        }

        public ChapterData ToChapterNodeData() {
            Rect rect = GetPosition();
            LevelOrder(FirstSubChap);

            return new ChapterData {
                AllowMidrolls = allowMidrolls,
                StoryCheckpoint = Checkpoint,
                SubChaps = SubChaps.ConvertAll(subChapNode => subChapNode.ToSubChapNodeData()),
                location = new Location {
                    x = rect.x,
                    y = rect.y,
                    Width = rect.width,
                    Height = rect.height
                }
            };
        }
        
        public Chapter ToChapterData() {
            LevelOrder(FirstSubChap);

            return new Chapter {
                AllowMidrolls = allowMidrolls,
                StoryCheckpoint = Checkpoint,
                SubChaps = SubChaps.ConvertAll(subChapNode => subChapNode.ToSubChapData())
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new ChapterNode(graphView);
        }
    }
}

