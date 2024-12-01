using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string characterName;       // The name of the character speaking
    public string text;                // The text of the dialogue
    public AudioClip voiceOverClip;    // Optional voice over audio clip
    public DialogueEvent[] events;     // Any events that should be triggered during this line
}