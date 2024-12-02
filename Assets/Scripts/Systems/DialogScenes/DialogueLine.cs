using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string characterName; // Name of the speaking character
    public string receiver; // Target character to interact with (optional)
    public string dialogueText; // The line of dialogue
    public AudioClip voiceOver; // Voice-over clip (optional)
    public float timestamp; // Time to trigger the line (0 = sequential)
    public List<DialogueEvent> events = new List<DialogueEvent>(); // Events linked to this line
}