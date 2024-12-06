using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueData dialogueData;
    private Vector2 scrollPos;

    private int selectedLineIndex = -1;

    [MenuItem("Tools/Dialogue Editor")]
    public static void OpenDialogueEditor()
    {
        // Make sure the window is opened with correct anti-aliasing settings
        GetWindow<DialogueEditorWindow>("Dialogue Editor");
    }

    private void OnGUI()
    {
        // Clear any invalid anti-aliasing settings, this shouldn't be required in the editor window
        EditorGUIUtility.labelWidth = 120; // Set label width for better alignment
        GUI.backgroundColor = Color.white;

        EditorGUILayout.LabelField("Dialogue Editor", EditorStyles.boldLabel);

        if (dialogueData == null)
            dialogueData = ScriptableObject.CreateInstance<DialogueData>();

        dialogueData.dialogueTitle = EditorGUILayout.TextField("Title", dialogueData.dialogueTitle);
        dialogueData.dialogueDescription = EditorGUILayout.TextField("Description", dialogueData.dialogueDescription);

        EditorGUILayout.Space();

        // Scrollable view for lines
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        if (dialogueData.dialogueLines == null)
            dialogueData.dialogueLines = new List<DialogueLine>();

        // Dialogue lines list
        for (int i = 0; i < dialogueData.dialogueLines.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            // Draw each dialogue line
            DrawDialogueLineEditor(i);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Buttons for adding new line or clearing all lines
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Line"))
        {
            dialogueData.dialogueLines.Add(new DialogueLine());
        }
        if (GUILayout.Button("Clear Dialogue"))
        {
            if (EditorUtility.DisplayDialog("Clear Dialogue", "Are you sure you want to clear the dialogue?", "Yes", "No"))
            {
                dialogueData = new DialogueData();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Import/Export options
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import JSON"))
        {
            ImportDialogue();
        }
        if (GUILayout.Button("Export JSON"))
        {
            ExportDialogue();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDialogueLineEditor(int index)
    {
        var line = dialogueData.dialogueLines[index];

        // Input fields for each line
        EditorGUILayout.LabelField($"Line {index + 1}", EditorStyles.boldLabel);
        line.characterName = EditorGUILayout.TextField("Character Name", line.characterName);
        line.receiver = EditorGUILayout.TextField("Receiver", line.receiver);
        line.dialogueText = EditorGUILayout.TextField("Dialogue Text", line.dialogueText);
        line.voiceOver = (AudioClip)EditorGUILayout.ObjectField("Voice Over", line.voiceOver, typeof(AudioClip), false);
        line.timestamp = EditorGUILayout.FloatField("Timestamp", line.timestamp);

        // Adding events to the dialogue line
        if (line.events == null)
            line.events = new List<DialogueEvent>();

        EditorGUILayout.LabelField("Events:");
        for (int j = 0; j < line.events.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();

            // Action Type selector with proper layout
            line.events[j].actionType = (DialogueEvent.ActionType)EditorGUILayout.EnumPopup("Action Type", line.events[j].actionType);

            // Target Field
            line.events[j].target = EditorGUILayout.TextField("Target", line.events[j].target);

            // Target Position field
            line.events[j].targetPosition = EditorGUILayout.Vector3Field("Target Position", line.events[j].targetPosition);

            // Animation fields
            line.events[j].animationName = EditorGUILayout.TextField("Animation Name", line.events[j].animationName);

            // Remove event button
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                line.events.RemoveAt(j);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        // Add Event button
        if (GUILayout.Button("Add Event"))
        {
            line.events.Add(new DialogueEvent());
        }

        // Remove dialogue line button
        if (GUILayout.Button("Remove Line"))
        {
            dialogueData.dialogueLines.RemoveAt(index);
            return;
        }
    }

    private void OnPositionSelected(Vector3 position)
    {
        if (selectedLineIndex >= 0 && selectedLineIndex < dialogueData.dialogueLines.Count)
        {
            var selectedLine = dialogueData.dialogueLines[selectedLineIndex];

            // Only update the position for the selected event
            foreach (var dialogueEvent in selectedLine.events)
            {
                if (dialogueEvent.targetPosition == position) return; // Don't update if already set
                dialogueEvent.targetPosition = position;
            }

            selectedLineIndex = -1;
            Repaint();
        }
    }

    private void ImportDialogue()
    {
        string path = EditorUtility.OpenFilePanel("Import Dialogue JSON", Application.dataPath, "json");
        if (string.IsNullOrEmpty(path))
            return;

        string json = File.ReadAllText(path);
        dialogueData = JsonUtility.FromJson<DialogueData>(json);

        EditorUtility.DisplayDialog("Success", "Dialogue imported successfully!", "OK");
    }

    private void ExportDialogue()
    {
        string path = EditorUtility.SaveFilePanel("Export Dialogue JSON", Application.dataPath, dialogueData.dialogueTitle, "json");
        if (string.IsNullOrEmpty(path))
            return;

        string json = JsonUtility.ToJson(dialogueData, true);
        File.WriteAllText(path, json);

        EditorUtility.DisplayDialog("Success", "Dialogue exported successfully!", "OK");
    }
}
