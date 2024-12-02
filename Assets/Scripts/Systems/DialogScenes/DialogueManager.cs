using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public Text characterNameText;
    public Text dialogueText;
    public GameObject dialoguePanel;
    public Animator characterAnimator;
    public AudioSource audioSource;
    public GameObject playerCharacter;
    public GameObject currentCharacter;

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private float audioStartTime;
    private bool isMoving = false;  // Track if the character is currently moving
    private bool isEventActive = false;  // Track if an event is currently active

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLineIndex < currentDialogue.lines.Count)
        {
            DialogueLine line = currentDialogue.lines[currentLineIndex];
            characterNameText.text = line.characterName;
            dialogueText.text = line.text;

            // Handle character movement and events sequentially
            HandleCharacterMovement(line);

            // Handle voiceover, if any
            if (line.voiceOverClip != null)
            {
                audioSource.clip = line.voiceOverClip;
                audioSource.Play();
                audioStartTime = Time.time;
                StartCoroutine(WaitForAudioToFinish(line.voiceOverClip.length, line.events));
            }

            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private void HandleCharacterMovement(DialogueLine line)
    {
        // Only process if there are events
        if (line.events != null && line.events.Count > 0)
        {
            foreach (var dialogueEvent in line.events)
            {
                // Check if the event is delayed based on the start timestamp
                if (Time.time >= audioStartTime + dialogueEvent.startTimestamp)
                {
                    if (dialogueEvent.mustMoveFirst)
                    {
                        // Prevent other events until movement is finished
                        StartCoroutine(MoveCharacterToPosition(dialogueEvent.moveToPosition, dialogueEvent.moveSpeed, dialogueEvent));
                    }
                    else
                    {
                        // Trigger the event immediately
                        TriggerEvent(dialogueEvent);
                    }
                }
            }
        }
    }

    private IEnumerator MoveCharacterToPosition(Vector3 targetPosition, float moveSpeed, DialogueEvent dialogueEvent)
    {
        isMoving = true;
        Vector3 startPosition = currentCharacter.transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        // Move character to the target position
        while (Vector3.Distance(currentCharacter.transform.position, targetPosition) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            currentCharacter.transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            yield return null;
        }

        currentCharacter.transform.position = targetPosition;

        // Once movement is finished, we trigger the event
        isMoving = false;
        TriggerEvent(dialogueEvent);
    }

    private void TriggerEvent(DialogueEvent dialogueEvent)
    {
        if (isMoving || isEventActive)
        {
            // If movement is still in progress or event is already active, don't trigger the action
            return;
        }

        isEventActive = true;  // Mark the event as active to avoid re-triggering during this time

        switch (dialogueEvent.actionName)
        {
            case DialogueAction.PickUpItem:
                GameObject targetObject = GameObject.Find(dialogueEvent.targetObjectName);
                if (targetObject != null)
                {
                    //targetObject.GetComponent<Item>().PickUp();  // Trigger the pickup action
                }
                break;

            case DialogueAction.PressButton:
                GameObject buttonObject = GameObject.Find(dialogueEvent.targetObjectName);
                if (buttonObject != null)
                {
                    //buttonObject.GetComponent<Button>().Press();  // Trigger the button press
                }
                break;

        }

        isEventActive = false;  // Reset the event status after execution
    }

    private IEnumerator WaitForAudioToFinish(float audioLength, System.Collections.Generic.List<DialogueEvent> events)
    {
        float audioEndTime = audioStartTime + audioLength;

        // Wait until the audio finishes, checking event timestamps
        foreach (var dialogueEvent in events)
        {
            while (Time.time < audioEndTime)
            {
                yield return null;
            }

            // Trigger the event after audio finishes
            TriggerEvent(dialogueEvent);
        }

        // Proceed to next dialogue line
        DisplayNextLine();
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
