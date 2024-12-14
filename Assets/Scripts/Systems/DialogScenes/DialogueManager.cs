using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogueData;
    private int currentLineIndex = 0;

    public delegate void EventAction(DialogueEvent dialogueEvent);
    public static event EventAction OnDialogueEventTriggered;

    public Dictionary<string, GameObject> characters = new Dictionary<string, GameObject>(); // Link names to character GameObjects

    private void Start()
    {
        StartCoroutine(PlayDialogue());
    }

    private IEnumerator PlayDialogue()
    {
        while (currentLineIndex < dialogueData.dialogueLines.Count)
        {
            DialogueLine line = dialogueData.dialogueLines[currentLineIndex];
            float delay = line.timestamp > 0 ? line.timestamp - Time.time : 0;

            if (delay > 0)
                yield return new WaitForSeconds(delay);

            StartCoroutine(PlayLine(line));
            currentLineIndex++;
        }
    }

    private IEnumerator PlayLine(DialogueLine line)
    {
        if (characters.TryGetValue(line.characterName, out GameObject speaker))
        {
            // Make the speaking character look at the receiver
            if (!string.IsNullOrEmpty(line.receiver) && characters.TryGetValue(line.receiver, out GameObject receiver))
            {
                Vector3 lookDirection = receiver.transform.position - speaker.transform.position;
                speaker.transform.rotation = Quaternion.LookRotation(lookDirection);
            }

            // Trigger dialogue and voice-over
            Debug.Log($"[{line.characterName}]: {line.dialogueText}");
            AudioSource audioSource = speaker.GetComponent<AudioSource>();
            float audioLength = 0;

            if (line.voiceOver != null && audioSource)
            {
                audioSource.clip = line.voiceOver;
                audioSource.Play();
                audioLength = line.voiceOver.length;
            }

            // Process events tied to this line
            foreach (var dialogueEvent in line.events)
            {
                HandleEvent(dialogueEvent, speaker);
            }

            // Wait for the line's audio to complete before continuing
            yield return new WaitForSeconds(audioLength > 0 ? audioLength : 1f);
        }
    }

    private void HandleEvent(DialogueEvent dialogueEvent, GameObject speaker)
    {
        switch (dialogueEvent.actionType)
        {
            case DialogueEvent.ActionType.Move:
                StartCoroutine(HandleMove(dialogueEvent, speaker));
                break;

            case DialogueEvent.ActionType.PickUp:
                // Ensure the character reaches the target before executing the pickup
                if (dialogueEvent.targetPosition != Vector3.zero)
                    StartCoroutine(HandleMove(dialogueEvent, speaker, () => Debug.Log($"{speaker.name} picks up {dialogueEvent.target}")));
                break;

            case DialogueEvent.ActionType.React:
                HandleReaction(dialogueEvent, speaker);
                break;

            case DialogueEvent.ActionType.Animate:
                PlayAnimation(speaker, dialogueEvent.animationName);
                break;

            case DialogueEvent.ActionType.Use:
                Debug.Log($"{speaker.name} uses {dialogueEvent.target}");
                break;
        }
    }

    private IEnumerator HandleMove(DialogueEvent dialogueEvent, GameObject speaker, System.Action onComplete = null)
    {
        if (dialogueEvent.targetPosition == Vector3.zero)
            yield break;

        Animator animator = speaker.GetComponent<Animator>();
        Vector3 startPosition = speaker.transform.position;
        Vector3 endPosition = dialogueEvent.targetPosition;
        float moveSpeed = 2.0f; // Adjustable speed
        float distance = Vector3.Distance(startPosition, endPosition);

        if (animator)
            animator.SetBool("isWalking", true);

        while (distance > 0.1f)
        {
            speaker.transform.position = Vector3.MoveTowards(speaker.transform.position, endPosition, moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(speaker.transform.position, endPosition);
            yield return null;
        }

        if (animator)
            animator.SetBool("isWalking", false);

        onComplete?.Invoke();
    }

    private void HandleReaction(DialogueEvent dialogueEvent, GameObject speaker)
    {
        PlayAnimation(speaker, dialogueEvent.animationName);
    }

    private void PlayAnimation(GameObject character, string animationName)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator && !string.IsNullOrEmpty(animationName))
        {
            animator.Play(animationName);
        }
    }
}
