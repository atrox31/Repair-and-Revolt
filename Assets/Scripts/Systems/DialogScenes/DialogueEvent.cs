using UnityEngine;

[System.Serializable]
public class DialogueEvent
{
    public enum ActionType { Move, PickUp, Use, React, Animate } // Event actions
    public ActionType actionType; // Enum-based event action
    public string target; // Target object or character (if applicable)
    public Vector3 targetPosition; // Target position for move-related events
    public string animationName; // Animation to trigger (if any)
}