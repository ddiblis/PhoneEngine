using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


namespace JSONMapper {
    public class SchemaGraphView : GraphView {
        public SchemaGraphView() {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddManipulators();
            AddContextualMenuOptions();
            AddStyles();
            this.graphViewChanged += OnGraphViewChanged;
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "JSONMapperStyles/JMGraphViewStyles.uss",
                "JSONMapperStyles/JMNodeStyles.uss"
            );
        }

        // it says 0 references but it is actually handling the connection, do not remove it
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            List<Port> compatiblePorts = new();
            ports.ForEach(port => {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;
                if (IsCompatible(startPort, port)) compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        private bool IsCompatible(Port startPort, Port targetPort) {
            var startNode = startPort.node as BaseNode;
            var targetNode = targetPort.node as BaseNode;

            // Ensure connections are between specific types
            if (
                startNode is ChapterNode
                && targetNode is SubChapNode
                && startPort.direction == Direction.Output
                && targetPort.direction == Direction.Input
            ) return true;

            if (
                startNode is SubChapNode
                && targetNode is ChapterNode
                && startPort.direction == Direction.Input
                && targetPort.direction == Direction.Output
            ) return true;

            if (
                startNode is SubChapNode
                && startPort.portName == "Text Messages"
                && targetNode is TextMessageNode
                && startPort.direction == Direction.Output
                && targetPort.direction == Direction.Input
            ) return true;

            if (
                startNode is SubChapNode
                && startPort.portName == "Responses"
                && targetNode is ResponseNode
                && startPort.direction == Direction.Output
                && targetPort.direction == Direction.Input
            ) return true;

            if (
                startNode is TextMessageNode
                && targetNode is SubChapNode
                && startPort.direction == Direction.Input
                && targetPort.direction == Direction.Output
                && targetPort.portName == "Text Messages"
            ) return true;

            if (
                startNode is ResponseNode
                && targetNode is SubChapNode
                && startPort.direction == Direction.Input
                && targetPort.direction == Direction.Output
                && targetPort.portName == "Responses"
            ) return true;

            if (
                startNode is ResponseNode
                && targetNode is SubChapNode
                && startPort.direction == Direction.Output
                && targetPort.direction == Direction.Input
                && targetPort.portName == "Parent Response"
            ) return true;

            if (
                startNode is SubChapNode
                && targetNode is ResponseNode
                && startPort.direction == Direction.Input
                && targetPort.direction == Direction.Output
                && targetPort.portName == "Next SubChap"
            ) return true;

            return false;
        }

        private void AddManipulators() {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void AddContextualMenuOptions() {
            this.AddManipulator(new ContextualMenuManipulator(evt => {
                evt.menu.AppendAction("Add Chapter Node", action => AddNode(new ChapterNode(this), GetLocalMousePosition(action.eventInfo.localMousePosition)));
                evt.menu.AppendAction("Add SubChap Node", action => AddNode(new SubChapNode(this), GetLocalMousePosition(action.eventInfo.localMousePosition)));
                evt.menu.AppendAction("Add TextMessage Node", action => AddNode(new TextMessageNode(this), GetLocalMousePosition(action.eventInfo.localMousePosition)));
                evt.menu.AppendAction("Add Response Node", action => AddNode(new ResponseNode(this), GetLocalMousePosition(action.eventInfo.localMousePosition)));
            }));
        }

        private void AddNode(BaseNode node, Vector2 position) {
            node.SetPosition(new Rect(position, new Vector2(200, 150)));
            AddElement(node);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            if (graphViewChange.edgesToCreate != null) {
                foreach (var edge in graphViewChange.edgesToCreate) {
                    var outputNode = edge.output.node as BaseNode;
                    var inputNode = edge.input.node as BaseNode;

                    outputNode?.OnConnected(edge.output, edge);
                    inputNode?.OnConnected(edge.input, edge);
                }
            }

            if (graphViewChange.elementsToRemove != null) {
                foreach (var element in graphViewChange.elementsToRemove) {
                    if (element is Edge edge) {
                        var outputNode = edge.output.node as BaseNode;
                        var inputNode = edge.input.node as BaseNode;

                        outputNode?.OnDisconnected(edge.output, edge);
                        inputNode?.OnDisconnected(edge.input, edge);
                    }
                }
            }

            return graphViewChange;
        }
        public Vector2 GetLocalMousePosition(Vector2 mousePosition) {
            Vector2 worldMousePosition = mousePosition;
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }
    }
}
