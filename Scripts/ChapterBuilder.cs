using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class JsonEditorWindow : EditorWindow
{
    private Chapter chapter = new();

    [MenuItem("Window/JSON Editor")]
    public static void ShowWindow()
    {
        GetWindow<JsonEditorWindow>("JSON Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Chapter", EditorStyles.boldLabel);
        chapter.AllowMidrolls = EditorGUILayout.Toggle("Allow Midrolls", chapter.AllowMidrolls);

        if (GUILayout.Button("Add SubChap"))
        {
            chapter.SubChaps.Add(new SubChap());
        }

        for (int i = 0; i < chapter.SubChaps.Count; i++)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label($"SubChap {i + 1}", EditorStyles.boldLabel);

            SubChap subChap = chapter.SubChaps[i];
            subChap.Contact = EditorGUILayout.TextField("Contact", subChap.Contact);
            subChap.TimeIndicator = EditorGUILayout.TextField("Time Indicator", subChap.TimeIndicator);
            subChap.UnlockInstaPostsAccount = EditorGUILayout.TextField("Unlock Insta Posts Account", subChap.UnlockInstaPostsAccount);

            if (GUILayout.Button("Add Text Message"))
            {
                subChap.TextList.Add(new TextMessage());
            }

            for (int j = 0; j < subChap.TextList.Count; j++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Text Message {j + 1}", EditorStyles.boldLabel);

                TextMessage textMessage = subChap.TextList[j];
                textMessage.Type = EditorGUILayout.IntField("Type", textMessage.Type);
                textMessage.TextContent = EditorGUILayout.TextField("Text Content", textMessage.TextContent);
                textMessage.TextDelay = EditorGUILayout.FloatField("Text Delay", textMessage.TextDelay);

                GUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Response"))
            {
                subChap.Responses.Add(new Response());
            }

            for (int k = 0; k < subChap.Responses.Count; k++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Response {k + 1}", EditorStyles.boldLabel);

                Response response = subChap.Responses[k];
                response.RespTree = EditorGUILayout.Toggle("Resp Tree", response.RespTree);
                response.TextContent = EditorGUILayout.TextField("Text Content", response.TextContent);
                response.SubChapNum = EditorGUILayout.IntField("SubChap Num", response.SubChapNum);
                response.Type = EditorGUILayout.IntField("Type", response.Type);

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Save to JSON"))
        {
            SaveToJson();
        }

        if (GUILayout.Button("Load from JSON"))
        {
            LoadFromJson();
        }
    }

    private void SaveToJson()
    {
        string json = JsonUtility.ToJson(chapter, true);
        string path = EditorUtility.SaveFilePanel("Save JSON", "", "chapter.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            EditorUtility.DisplayDialog("JSON Saved", "The JSON file has been saved successfully.", "OK");
        }
    }

    private void LoadFromJson()
    {
        string path = EditorUtility.OpenFilePanel("Load JSON", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = File.ReadAllText(path);
            chapter = JsonUtility.FromJson<Chapter>(json);

            // Ensure all lists are initialized
            foreach (var subChap in chapter.SubChaps)
            {
                subChap.TextList ??= new List<TextMessage>();

                subChap.UnlockPosts ??= new List<int>();

                subChap.Responses ??= new List<Response>();
            }

            EditorUtility.DisplayDialog("JSON Loaded", "The JSON file has been loaded successfully.", "OK");
        }
    }
}

