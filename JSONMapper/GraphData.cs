using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.IO;
using UnityEditor.UIElements;
using System.Linq;

namespace JSONMapper {
    [System.Serializable]
    public class GraphData : ScriptableObject {
        public List<ChapterData> Chapters = new();
        

        public void CopyFrom(GraphData saved) {
            Chapters = new List<ChapterData>(saved.Chapters);
        }

        public void PopulateGraphView(GraphView graphView) {
            ChapterData Chapter = Chapters[0];
            var ChapterNode = new ChapterNode(graphView) {
                allowMidrolls = Chapter.AllowMidrolls,
                Checkpoint = Chapter.StoryCheckpoint
            };
            ChapterNode.UpdateFields();
            ChapterNode.SetPosition(new Rect{
                x = Chapter.location.x,
                y = Chapter.location.y,
                width = Chapter.location.Width,
                height = Chapter.location.Height,
            });
            graphView.AddElement(ChapterNode);
            LoadSubChapNodes(Chapter, graphView, ChapterNode);
        }

        private void ConnectSubChapToParentResponses(GraphView graphView) {
            List<ResponseNode> ResponseParentNodes = new();
            SubChapNode subChap = null;

            foreach(var node in graphView.nodes) {
                if (node is ResponseNode responseNode) {
                    foreach(var node1 in graphView.nodes) {
                        if (node1 is SubChapNode subChapNode && subChapNode.SubChapIndex == responseNode.SubChapNum) {
                            subChap = subChapNode;
                            ResponseParentNodes.Add(responseNode);
                            responseNode.NextSubChap = subChapNode;
                        }
                    }
                    foreach (var respNode in ResponseParentNodes) {
                        ConnectNodes(subChap.ParentResponsePort, respNode.NextSubChapterNodePort, graphView);
                    }
                }
                ResponseParentNodes = new();
                subChap = null;
            }
        }


        private void ConnectNodes(Port InputPort, Port OutputPort, GraphView graphView) {
            Edge Connection = new() {
                input = InputPort,
                output = OutputPort
            };
            graphView.AddElement(Connection);
            OutputPort.ConnectTo(InputPort);
        }

        private void LoadSubChapNodes(ChapterData chapter, GraphView graphView, ChapterNode ChapterNode) {
            int index = 0;
            foreach(SubChapData subChap in chapter.SubChaps) {
                var SubChapNode = new SubChapNode(graphView) {
                Contact = subChap.Contact,
                UnlockIPAccount = subChap.UnlockInstaPostsAccount,
                UnlockPosts = subChap.UnlockPosts,
                SubChapIndex = index
                };
                SubChapNode.UpdateFields();
                SubChapNode.SetPosition(new Rect {
                    x = subChap.location.x,
                    y = subChap.location.y,
                    width = subChap.location.Width,
                    height = subChap.location.Height
                });
                graphView.AddElement(SubChapNode);
                // Creates the connections and adds them to the list inside the parent node
                if (index == 0) {
                    ChapterNode.FirstSubChap = SubChapNode;
                    ConnectNodes(SubChapNode.ParentChapterPort, ChapterNode.SubChaptersPort, graphView);
                } 
                LoadTextMessageNodes(subChap.TextList, graphView, SubChapNode);
                LoadResponseNodes(subChap, graphView, SubChapNode);
                index += 1;
            }
            ConnectSubChapToParentResponses(graphView);
        }

        private void LoadTextMessageNodes(List<TextMessageData> textList, GraphView graphView, SubChapNode SubChapNode) {
            TextMessageNode PrevTextNode = null;
            for (int i = 0; i < textList.Count; i ++) {
                TextMessageData text = textList[i];
                var TextMessageNode = new TextMessageNode(graphView, text.Type) {
                    AltContact = text.AltContact,
                    TextContent = text.TextContent,
                    TextDelay = text.TextDelay,
                    Type = text.Type,
                    Tendency = text.Tendency
                };
                TextMessageNode.UpdateFields();
                TextMessageNode.SetPosition(new Rect {
                    x = text.location.x,
                    y = text.location.y,
                    width = text.location.Width,
                    height = text.location.Height,
                });
                graphView.AddElement(TextMessageNode);
                if (i == 0) {
                    ConnectNodes(TextMessageNode.ParentSubChapPort, SubChapNode.FirstTextPort, graphView);
                    SubChapNode.FirstText = TextMessageNode;
                    PrevTextNode = TextMessageNode;
                } else {
                    ConnectNodes(TextMessageNode.PrevText, PrevTextNode.NextText, graphView);
                    PrevTextNode.NextTextNode = TextMessageNode;
                    PrevTextNode = TextMessageNode;
                }
            }
        }

        private void LoadResponseNodes(SubChapData subChap, GraphView graphView, SubChapNode SubChapNode) {
            foreach (ResponseData resp in subChap.Responses) {
                var ResponseNode = new ResponseNode(graphView, resp.Type) {
                    RespTree = resp.RespTree,
                    TextContent = resp.TextContent,
                    SubChapNum = resp.SubChapNum,
                    Type = resp.Type
                };
                ResponseNode.UpdateFields();
                ResponseNode.SetPosition(new Rect {
                    x = resp.location.x,
                    y = resp.location.y,
                    width = resp.location.Width,
                    height = resp.location.Height,
                });
                graphView.AddElement(ResponseNode);
                ConnectNodes(ResponseNode.ParentSubChapPort, SubChapNode.ResponsesPort, graphView);
                SubChapNode.Responses.Add(ResponseNode);

            }
        }

    }
}
