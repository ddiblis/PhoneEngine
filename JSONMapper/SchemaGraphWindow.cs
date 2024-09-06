using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System.Linq;
using Unity.VisualScripting;


namespace JSONMapper {
    public class SchemaGraphWindow : EditorWindow {
        private float delayTime = 60f;
        private float AutoSaveTimer;
        private SchemaGraphView graphView;

        [MenuItem("Window/JSONMapper")]
        public static void Open() {
            var window = GetWindow<SchemaGraphWindow>();
            window.titleContent = new GUIContent("JSONMapper");
        }

        private void OnEnable() {
            ConstructGraphView();
            GenerateToolbar();
            AddStyles();
            AutoSaveTimer = (float)EditorApplication.timeSinceStartup + delayTime;
            EditorApplication.update += OnEditorUpdate;
        }
        private void OnGUI() {
            delayTime = EditorGUILayout.FloatField("Delay Time (seconds)", delayTime);
        }

        private void OnDisable() {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate() {
            if (EditorApplication.timeSinceStartup >= AutoSaveTimer) {
                AutoSaveAsAsset();
                // Reset the next action time
                AutoSaveTimer = (float)EditorApplication.timeSinceStartup + delayTime;
            }
        }

        private void AutoSaveAsAsset() {
            var graphData = CreateInstance<GraphData>();
            foreach (var node in graphView.nodes) {
                if (node is ChapterNode chapterNode) {
                    graphData.Chapters.Add(chapterNode.ToChapterNodeData());
                }
            }

            string path = Application.dataPath + "/Chapters/AutoSave.asset";
            if (!string.IsNullOrEmpty(path)) {
                var asset = CreateInstance<GraphData>();
                asset.CopyFrom(graphData);
                AssetDatabase.CreateAsset(asset, FileUtil.GetProjectRelativePath(path));
                AssetDatabase.SaveAssets();
                Debug.Log("Graph AutoSaved to " + path);
            }
        }

        private void AddStyles() {
            rootVisualElement.AddStyleSheets(
                "JSONMapperStyles/JMVariables.uss"
            );
        }

        private void ConstructGraphView() {
            graphView = new SchemaGraphView
            {
                name = "JSONMapper"
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void GenerateToolbar() {
            var toolbar = new Toolbar();

            var saveButton = new Button(() => SaveGraphToJson()) {
                text = "Save as JSON"
            };
            toolbar.Add(saveButton);

            var saveAssetButton = new Button(() => SaveGraphToAsset()) {
                text = "Save to Asset"
            };
            toolbar.Add(saveAssetButton);

            var LoadAssetButton = new Button(() => LoadGraphFromAsset()) {
                text = "Load From Asset"
            };
            toolbar.Add(LoadAssetButton); 
            toolbar.AddStyleSheets("JSONMapperStyles/JMToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }
        
        private void SaveGraphToAsset() {
            var graphData = CreateInstance<GraphData>();
            foreach (var node in graphView.nodes) {
                if (node is ChapterNode chapterNode) {
                    graphData.Chapters.Add(chapterNode.ToChapterNodeData());
                }
            }

            string path = EditorUtility.SaveFilePanel("Save Graph to Asset", "", "GraphData.asset", "asset");
            if (!string.IsNullOrEmpty(path)) {
                var asset = CreateInstance<GraphData>();
                asset.CopyFrom(graphData);
                AssetDatabase.CreateAsset(asset, FileUtil.GetProjectRelativePath(path));
                AssetDatabase.SaveAssets();
                Debug.Log("Graph saved to " + path);
            }
        }

        private void LoadGraphFromAsset() {
            string path = EditorUtility.OpenFilePanel("Load Graph from Asset", "", "asset");
            if (!string.IsNullOrEmpty(path)) {
                var asset = AssetDatabase.LoadAssetAtPath<GraphData>(FileUtil.GetProjectRelativePath(path));
                if (asset != null) {
                    foreach (var node in graphView.nodes) {
                        graphView.RemoveElement(node);
                    }
                    foreach(var connection in graphView.edges) {
                        graphView.RemoveElement(connection);
                    }
                    asset.PopulateGraphView(graphView);
                    Debug.Log("Graph loaded from " + path);
                }
            }
        }

        private void SaveGraphToJson() {
            Chapter Chapter = new();

            foreach (var node in graphView.nodes) {
                if (node is ChapterNode chapterNode) {
                    Chapter = chapterNode.ToChapterData();
                    break;
                }
            }

            string json = JsonUtility.ToJson(Chapter, true);
            string path = EditorUtility.SaveFilePanel("Save Graph", "", "GraphData.json", "json");
            if (!string.IsNullOrEmpty(path)) {
                File.WriteAllText(path, json);
                Debug.Log("Graph saved to " + path);
            }
        }
    }
}
