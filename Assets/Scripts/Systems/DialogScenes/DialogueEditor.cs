using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueData))]
public class DialogueEditor : Editor
{
    private DialogueData dialogueData;
    private bool showLines = true;

    // Track the current dialogue line and event being edited for position
    private DialogueLine editingLine = null;
    private DialogueEvent editingEvent = null;

    public override void OnInspectorGUI()
    {
        dialogueData = (DialogueData)target;

        EditorGUILayout.LabelField("Dialogue Data Editor", EditorStyles.boldLabel);

        dialogueData.dialogueTitle = EditorGUILayout.TextField("Title", dialogueData.dialogueTitle);
        dialogueData.dialogueDescription = EditorGUILayout.TextField("Description", dialogueData.dialogueDescription);

        if (GUILayout.Button("Add Dialogue Line"))
        {
            dialogueData.dialogueLines.Add(new DialogueLine());
        }

        showLines = EditorGUILayout.Foldout(showLines, "Dialogue Lines");

        if (showLines)
        {
            for (int i = 0; i < dialogueData.dialogueLines.Count; i++)
            {
                DialogueLine line = dialogueData.dialogueLines[i];
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField($"Line {i + 1}", EditorStyles.boldLabel);

                line.characterName = EditorGUILayout.TextField("Character Name", line.characterName);
                line.receiver = EditorGUILayout.TextField("Receiver", line.receiver);
                line.dialogueText = EditorGUILayout.TextField("Dialogue Text", line.dialogueText);
                line.voiceOver = (AudioClip)EditorGUILayout.ObjectField("Voice Over", line.voiceOver, typeof(AudioClip), false);
                line.timestamp = EditorGUILayout.FloatField("Timestamp", line.timestamp);

                if (GUILayout.Button("Add Event"))
                {
                    line.events.Add(new DialogueEvent());
                }

                for (int j = 0; j < line.events.Count; j++)
                {
                    DialogueEvent dialogueEvent = line.events[j];
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField($"Event {j + 1}", EditorStyles.boldLabel);

                    dialogueEvent.actionType = (DialogueEvent.ActionType)EditorGUILayout.EnumPopup("Action Type", dialogueEvent.actionType);
                    dialogueEvent.target = EditorGUILayout.TextField("Target", dialogueEvent.target);
                    dialogueEvent.animationName = EditorGUILayout.TextField("Animation Name", dialogueEvent.animationName);

                    if (dialogueEvent.targetPosition != Vector3.zero)
                    {
                        dialogueEvent.targetPosition = EditorGUILayout.Vector3Field("Target Position", dialogueEvent.targetPosition);
                    }

                    if (GUILayout.Button("Select Position From Scene"))
                    {
                        editingLine = line;
                        editingEvent = dialogueEvent;
                        DialoguePositionTool.StartPositionSelection(OnPositionSelected);
                    }

                    if (GUILayout.Button("Clear Position"))
                    {
                        dialogueEvent.targetPosition = Vector3.zero;
                    }

                    if (GUILayout.Button("Remove Event"))
                    {
                        line.events.RemoveAt(j);
                    }

                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("Remove Line"))
                {
                    dialogueData.dialogueLines.RemoveAt(i);
                }

                EditorGUILayout.EndVertical();
            }
        }

        if (GUILayout.Button("Save Dialogue"))
        {
            EditorUtility.SetDirty(dialogueData);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.SetDirty(dialogueData);
    }

    private void OnPositionSelected(Vector3 position)
    {
        if (editingEvent != null)
        {
            editingEvent.targetPosition = position;
            Debug.Log($"Position selected: {position}");
        }

        editingLine = null;
        editingEvent = null;
    }
}
