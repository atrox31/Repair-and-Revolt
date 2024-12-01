using UnityEngine;

public enum DialogueAction
{
    PickUpItem,
    PressButton
}

[System.Serializable]
public class DialogueEvent
{
    public DialogueAction actionName;      // Action to perform (now using enum)
    public string targetObjectName;       // Target object to interact with (if applicable)
    public Vector3 moveToPosition;        // Position to move character to
    public float moveSpeed;               // Speed of movement
    public bool mustMoveFirst = false;    // If true, ensure move happens before any other event actions
    public float startTimestamp = 0f;     // Timestamp at which the event should start
}