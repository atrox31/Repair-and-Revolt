using UnityEngine;

namespace Assets.Scripts.Systems.DialogScenes
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;       // The name of the character speaking
        public string text;                // The text of the dialogue
        public AudioClip voiceOverClip;    // Optional voice over audio clip
        public System.Collections.Generic.List<DialogueEvent> events;     // Any events that should be triggered during this line

        public DialogueLine()
        {
        }

        public DialogueLine(string speaker, string dialogueText, AudioClip clip, System.Collections.Generic.List<DialogueEvent> dialogueEvents = null)
        {
            characterName = speaker;
            text = dialogueText;
            voiceOverClip = clip;
            events = dialogueEvents;
        }
    }
}