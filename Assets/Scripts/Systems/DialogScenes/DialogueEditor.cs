using UnityEditor;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    private DialogueData currentDialogue;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Dialogue Editor")]
    public static void ShowWindow()
    {
        GetWindow<DialogueEditor>("Dialogue Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialogue Editor", EditorStyles.boldLabel);

        // Load or create dialogue
        currentDialogue = (DialogueData)EditorGUILayout.ObjectField("Dialogue File", currentDialogue, typeof(DialogueData), false);
        if (currentDialogue == null)
        {
            if (GUILayout.Button("Create New Dialogue"))
            {
                currentDialogue = CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(currentDialogue, "Assets/NewDialogue.asset");
                AssetDatabase.SaveAssets();
            }
            return;
        }

        if (GUILayout.Button("Save Dialogue"))
        {
            EditorUtility.SetDirty(currentDialogue);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < currentDialogue.lines.Count; i++)
        {
            var line = currentDialogue.lines[i];
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label($"Line {i + 1}", EditorStyles.boldLabel);

            line.characterName = EditorGUILayout.TextField("Speaker Name", line.characterName);
            line.text = EditorGUILayout.TextField("Dialogue Text", line.text);
            line.voiceOverClip = (AudioClip)EditorGUILayout.ObjectField("Voice Over", line.voiceOverClip, typeof(AudioClip), false);

            EditorGUILayout.LabelField("Actions:");
            if (line.events == null)
                line.events = new System.Collections.Generic.List<DialogueEvent>();

            for (int j = 0; j < line.events.Count; j++)
            {
                var action = line.events[j];
                EditorGUILayout.BeginHorizontal();
                action.actionName = (DialogueAction)EditorGUILayout.EnumPopup("Action Type", action.actionName);
                action.targetObjectName = EditorGUILayout.TextField("Target Name", action.targetObjectName);
                action.moveToPosition = EditorGUILayout.Vector3Field("Move To Position", action.moveToPosition);
                action.mustMoveFirst = EditorGUILayout.Toggle("Move To Position", action.mustMoveFirst);
                action.startTimestamp = EditorGUILayout.FloatField("Start Time", action.startTimestamp);

                if (GUILayout.Button("Remove Action"))
                {
                    line.events.RemoveAt(j);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Action"))
            {
                line.events.Add(new DialogueEvent());
            }

            if (GUILayout.Button("Remove Line"))
            {
                currentDialogue.lines.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        if (GUILayout.Button("Add New Line"))
        {
            currentDialogue.lines.Add(new DialogueLine());
        }

        EditorGUILayout.EndScrollView();
    }
}
