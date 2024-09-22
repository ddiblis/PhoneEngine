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
        public bool isChapter = true;

        public SubChapNode FirstSubChap;

        private readonly Toggle allowMidrollsToggle;

        private readonly DropdownField CheckpointDropdown;
        // determines if what you're saving is a chapter or midroll
        private readonly Toggle ChapterToggle;

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

            ChapterToggle = new Toggle("Is Chapter") { value = isChapter };
            ChapterToggle.RegisterValueChangedCallback(evt => isChapter = evt.newValue);
        

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
            Foldout.Add(ChapterToggle);
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
            ChapterToggle.value = isChapter;
        }

        private void LevelOrder(SubChapNode rootNode, int chapNum = -1, DBRoot DB = null) {
            if (rootNode == null) return;

            Queue<SubChapNode> queue = new();
            queue.Enqueue(rootNode);

            // Process nodes in level order
            while (queue.Count > 0) {
                SubChapNode node = queue.Dequeue();

                if (!SubChaps.Contains(node)) {
                    SubChaps.Add(node);

                    // Add unlock posts if chapter number is valid
                    if (chapNum > -1 && node.UnlockPosts.Count > 0) {
                        DB?.ChapterInstaPosts[chapNum].InstaPostsList.AddRange(node.UnlockPosts);
                    }
                }

                // Enqueue non-null responses
                for (int i = 0; i < node.Responses.Count; i++) {
                    SubChapNode nextSubChap = node.Responses[i].NextSubChap;
                    if (nextSubChap != null && !queue.Contains(nextSubChap)) {
                        queue.Enqueue(nextSubChap);
                    }
                }
            }

            // Update response indices
            foreach (SubChapNode subChap in SubChaps) {
                for (int i = 0; i < subChap.Responses.Count; i++) {
                    SubChapNode nextSubChap = subChap.Responses[i].NextSubChap;
                    if (nextSubChap != null) {
                        subChap.Responses[i].SubChapNum = SubChaps.IndexOf(nextSubChap);
                    }
                }
            }
        }
        
        public ChapterData ToChapterNodeData() {
            Rect rect = GetPosition();
            LevelOrder(FirstSubChap);

            return new ChapterData {
                isChapter = isChapter,
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
        
        public Chapter ToChapterData(DBRoot DB, string ChapName = "") {
            int indexOfChap = DB.ChapterList.FindIndex(x => x == ChapName);
            if (isChapter) {
                if(indexOfChap == -1) {
                    indexOfChap = DB.ChapterList.Count;
                    DB.ChapterList.Add(ChapName);
                    DB.ChapterInstaPosts.Add(new ChapterInstaPostsList());
                    DB.ChapterImages.Add(new ChapterImagesList());
                } else {
                    DB.ChapterList[indexOfChap] = ChapName;
                    DB.ChapterInstaPosts[indexOfChap] = new();
                    DB.ChapterImages[indexOfChap] = new();
                }
                LevelOrder(FirstSubChap, indexOfChap, DB);
            } else {
                LevelOrder(FirstSubChap);
            }

            return new Chapter {
                AllowMidrolls = allowMidrolls,
                StoryCheckpoint = Checkpoint,
                SubChaps = SubChaps.ConvertAll(subChapNode => subChapNode.ToSubChapData(DB, isChapter, indexOfChap))
            };
        }

        public override BaseNode InstantiateNodeCopy() {
            return new ChapterNode(graphView);
        }
    }
}