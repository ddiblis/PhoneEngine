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

        private void ConnectSubChapToParentResponses(GraphView graphView, Port InputPort, int index) {
            List<ResponseNode> ResponseParentNodes = new();

            foreach (var node in graphView.nodes) {
                if (node is ResponseNode responseNode && responseNode.SubChapNum == index) {
                    ResponseParentNodes.Add(responseNode);
                }
            }
            foreach (var respNode in ResponseParentNodes) {
                ConnectNodes(InputPort, respNode.NextSubChapterNodePort, graphView);
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
                UnlockInstaPostsAccount = subChap.UnlockInstaPostsAccount,
                UnlockPosts = subChap.UnlockPosts
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
                ConnectNodes(SubChapNode.ParentChapterPort, ChapterNode.SubChaptersPort, graphView);
                ChapterNode.SubChaps.Add(SubChapNode);
                ConnectSubChapToParentResponses(graphView, SubChapNode.ParentResponsePort, index);
                LoadTextMessageNodes(subChap, graphView, SubChapNode);
                LoadResponseNodes(subChap, graphView, SubChapNode);
                index += 1;
            }
        }

        private void LoadTextMessageNodes(SubChapData subChap, GraphView graphView, SubChapNode SubChapNode) {
            foreach (TextMessageData text in subChap.TextList) {
                var TextMessageNode = new TextMessageNode(graphView) {
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
                ConnectNodes(TextMessageNode.ParentSubChapPort, SubChapNode.TextMessagesPort, graphView);
                SubChapNode.TextList.Add(TextMessageNode);
            }
        }

        private void LoadResponseNodes(SubChapData subChap, GraphView graphView, SubChapNode SubChapNode) {
            foreach (ResponseData resp in subChap.Responses) {
                var ResponseNode = new ResponseNode(graphView) {
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
