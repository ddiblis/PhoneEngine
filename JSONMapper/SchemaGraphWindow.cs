using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System.Linq;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

namespace JSONMapper {

    public class SchemaGraphWindow : EditorWindow {
        
        private float delayTime = 320f;
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

            var saveChapterButton = new Button(() => SaveGraphToJson(ChapterType.Chapter)) {
                text = "Save a Chapter as JSON"
            };
            toolbar.Add(saveChapterButton);

            var saveMidrollButton = new Button(() => SaveGraphToJson(ChapterType.Midroll)) {
                text = "Save a Midroll as JSON"
            };
            toolbar.Add(saveMidrollButton);

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
                    break;
                }
            }

            string path = EditorUtility.SaveFilePanel("Save Graph to Asset", Application.dataPath + "/Chapters", "GraphData.asset", "asset");
            if (!string.IsNullOrEmpty(path)) {
                var asset = CreateInstance<GraphData>();
                asset.CopyFrom(graphData);
                AssetDatabase.CreateAsset(asset, FileUtil.GetProjectRelativePath(path));
                AssetDatabase.SaveAssets();
                Debug.Log("Graph saved to " + path);
            }
        }

        private void LoadGraphFromAsset() {
            string path = EditorUtility.OpenFilePanel("Load Graph from Asset", Application.dataPath + "/Chapters", "asset");
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

        public DBRoot LoadDB() {
            TextAsset DBFile = Resources.Load<TextAsset>("DB");
            return JsonUtility.FromJson<DBRoot>(DBFile.text);
        }

        private void SaveGraphToJson(ChapterType chapterType) {
            Chapter Chapter = new();
            DBRoot DB = LoadDB();

            string path;
            if (chapterType == ChapterType.Chapter) {
                path = EditorUtility.SaveFilePanel("Save Graph", Application.dataPath + "/Resources/Chapters", "Chapter.json", "json");
                var match = Regex.Match(path, @"\d+");

                if (match.Success) {
                    int parsedNumber = int.Parse(match.Value);
                    foreach (var node in graphView.nodes) {
                        if (node is ChapterNode chapterNode) {
                            Chapter = chapterNode.ToChapterData(DB, parsedNumber);
                            break;
                        }
                    }
                } else {
                    Debug.LogWarning("Chapters must be numbered. EG. Chapter1");
                }
            } else {
                path = EditorUtility.SaveFilePanel("Save Graph", Application.dataPath + "/Resources/Midrolls", "placeholder-0.json", "json");
                var match = Regex.Match(path, @"\d+");
                if (match.Success) {
                    int parsedNumber = int.Parse(match.Value);
                    foreach (var node in graphView.nodes) {
                        if (node is ChapterNode chapterNode) {
                            Chapter = chapterNode.ToChapterData(DB, parsedNumber);
                            break;
                        }
                    }
                    int indexOfMidrollNameStart = path.LastIndexOf("/") + 1;
                    DB.MidrollsList.Add( new DBMidRoll {
                        MidrollName = path[indexOfMidrollNameStart..^5],
                        Checkpoint = parsedNumber
                    });
                } else {
                    Debug.LogWarning("Midrolls must contain number of storypoint from checkpoint enum. EG. Goingout-1");
                }
            }


            string json = JsonUtility.ToJson(Chapter, true);
            if (!string.IsNullOrEmpty(path)) {
                File.WriteAllText(path, json);
                string DBFileName = Application.dataPath + "/Resources/DB.json";
                string DBJson = JsonUtility.ToJson(DB, true);
                File.WriteAllText(DBFileName, DBJson);
                Debug.Log("Graph saved to " + path);
            }
        }
    }
}
