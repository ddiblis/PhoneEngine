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

            string path = $"{Application.dataPath}/Chapters/AutoSave.asset";
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

            string path = EditorUtility.SaveFilePanel("Save Graph to Asset", $"{Application.dataPath}/Chapters", "GraphData.asset", "asset");
            if (!string.IsNullOrEmpty(path)) {
                var asset = CreateInstance<GraphData>();
                asset.CopyFrom(graphData);
                AssetDatabase.CreateAsset(asset, FileUtil.GetProjectRelativePath(path));
                AssetDatabase.SaveAssets();
                Debug.Log("Graph saved to " + path);
            }
        }

        private void LoadGraphFromAsset() {
            string path = EditorUtility.OpenFilePanel("Load Graph from Asset", $"{Application.dataPath}/Chapters", "asset");
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
            try {
                TextAsset DBFile = Resources.Load<TextAsset>("DB");
                return JsonUtility.FromJson<DBRoot>(DBFile.text);
            }
            catch {
                Debug.LogWarning("DB File not found, creating new one");
                return new DBRoot();
            }
        }

        void SaveChapterJson(Chapter Chapter, string path, DBRoot DB) {
            string json = JsonUtility.ToJson(Chapter, true);
            if (!string.IsNullOrEmpty(path)) {
                File.WriteAllText(path, json);
                string DBFileName = $"{Application.dataPath}/Resources/DB.json";
                string DBJson = JsonUtility.ToJson(DB, true);
                File.WriteAllText(DBFileName, DBJson);
                Debug.Log("Graph saved to " + path);
            }
        }

        private void SaveGraphToJson(ChapterType chapterType) {
            Chapter Chapter = new();
            DBRoot DB = LoadDB();
            string DirectoryPath, DefaultFileName, ErrorMessage;

            if (chapterType == ChapterType.Chapter) {
                DirectoryPath = $"{Application.dataPath}/Resources/Chapters";
                DefaultFileName = "Chapter.json";
                ErrorMessage = "Chapters must be numbered. EG. Chapter1, Chapter didn't save";
            } else {
                DirectoryPath = $"{Application.dataPath}/Resources/Midrolls";
                DefaultFileName = "placeholder-0.json";
                ErrorMessage = "Midrolls must contain number of storypoint from checkpoint enum. EG. Goingout-1, Midroll didn't save";
            }

            if (!Directory.Exists(DirectoryPath)) {
                Directory.CreateDirectory(DirectoryPath);
                Debug.LogWarning($"{DirectoryPath} Directory not found, created one");
            }

            string path = EditorUtility.SaveFilePanel("Save Graph", DirectoryPath, DefaultFileName, "json");
            int indexOfLastSlash = path.LastIndexOf("/") + 1;
            string fileName = path[indexOfLastSlash..^5];

            // Check for number in file name
            var match = Regex.Match(fileName, @"\d+");
            if (match.Success) {
                // Save the chapter node
                foreach (var node in graphView.nodes) {
                    if (node is ChapterNode chapterNode) {
                        Chapter = chapterNode.ToChapterData(DB, fileName);
                        break;
                    }
                }

                // Add to Midroll list if applicable
                if (chapterType == ChapterType.Midroll) {
                    int parsedNumber = int.Parse(match.Value);
                    DB.MidrollsList.Add(new DBMidRoll {
                        MidrollName = fileName,
                        Checkpoint = parsedNumber
                    });
                }

                SaveChapterJson(Chapter, path, DB);
            } else {
                Debug.LogError(ErrorMessage);
            }
        }
    }
}
