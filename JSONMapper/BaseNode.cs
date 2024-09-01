using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JSONMapper {
    public abstract class BaseNode : Node {
        protected GraphView graphView;

        public BaseNode(GraphView graphView) {
            this.graphView = graphView;

            this.AddManipulator(new ContextualMenuManipulator(evt => evt.menu.AppendAction("Duplicate Node", action => {
                var duplicate = this.InstantiateNodeCopy();
                duplicate.SetPosition(new Rect(this.GetPosition().position + new Vector2(20, 20), this.GetPosition().size));
                this.graphView.AddElement(duplicate);
            })));

            RefreshExpandedState();
            RefreshPorts();
        }

        public virtual void OnConnected(Port port, Edge edge) {
            if (port.direction == Direction.Output) {
                if (this is SubChapNode subChapNode) {
                    if (port.portName == "Text Messages" && edge.input.node is TextMessageNode textMessageNode) {
                        subChapNode.TextList.Add(textMessageNode);
                    }
                    else if (port.portName == "Responses" && edge.input.node is ResponseNode responseNode) {
                        subChapNode.Responses.Add(responseNode);
                    }
                }

                else if (this is ChapterNode chapterNode && port.portName == "SubChapters" && edge.input.node is SubChapNode subChap) {
                    chapterNode.SubChaps.Add(subChap);
                }
                else if (this is ResponseNode responseNode && port.portName == "Next SubChap" && edge.input.node is SubChapNode subChap1) {
                    ChapterNode chapNode = null;
                    foreach (var node in graphView.nodes) {
                        if (node is ChapterNode chapterNode1) {
                            chapNode = chapterNode1;
                        }
                    }
                    responseNode.SubChapNum = chapNode.SubChaps.FindIndex(x => x == subChap1);
                    responseNode.UpdateFields();
                }
            }
        }

        public virtual void OnDisconnected(Port port, Edge edge) {
            if (port.direction == Direction.Output) {
                if (this is SubChapNode subChapNode) {
                    if (port.portName == "Text Messages" && edge.input.node is TextMessageNode textMessageNode) {
                        subChapNode.TextList.Remove(textMessageNode);
                    }
                    else if (port.portName == "Responses" && edge.input.node is ResponseNode responseNode) {
                        subChapNode.Responses.Remove(responseNode);
                    }
                }
                else if (this is ChapterNode chapterNode && port.portName == "SubChapters" && edge.input.node is SubChapNode subChap) {
                    chapterNode.SubChaps.Remove(subChap);
                }
            }
        }

        public abstract BaseNode InstantiateNodeCopy();
    }
}
