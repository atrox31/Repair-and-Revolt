using UnityEngine;
using System.Collections;

public class NPCbase : MonoBehaviour
{
    public enum NPCState
    {
        // every possigle state for each npc in game
        Idle,
        Walking,
        Resting,
        Sitting,
        Guard,
        Patroling,
        Working,
        Suspicious,
        Chasing,
        Catching,
        Dialogue
    }

    // Common NPC properties
    [SerializeField]
    private string npcName;
    [SerializeField]
    private float moveSpeed = 3f;
    [SerializeField]
    private float sightRange = 5f;

    // inner variables
    private Vector3 targetPosition;
    private bool playerInSight;
    private NPCState currentState = NPCState.Idle;
    private LayerMask playerLayer;
    private Transform playerTransform;

    //components
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent navAgent;

    // state flags
    private bool StateIsInDialogue = false;
    private bool StateIsBusy = false;
    private float StateSuspiciousMeter = 0.0f;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
        }
    }

    public virtual bool HandleState() { return false; }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Check if player is within detection range
        if (StateIsBusy == false)
        {
            CheckForPlayerDetection();
            if (GameManager.GlobalStatus_PlayerIsSuspicious && playerInSight)
            {
                StateSuspiciousMeter += Time.deltaTime * GameManager.StateSuspiciousMeterGrow;
            }
            if(GameManager.GlobalStatus_PlayerIsSuspicious == false)
            {
                StateSuspiciousMeter = 0.0f;
            }
        }
        // handle custom handler, if not nessesary run standard behaviour
        if ((StateIsInDialogue == false) && (HandleState() == false))
        {
            // Perform NPC's state behavior
            HandleNPCStandardState();
        }
    }

    // Method for detecting the player
    protected virtual void CheckForPlayerDetection()
    {
        // Simple detection: If the player is within sight range, the NPC notices them
        if (StateIsBusy || StateIsInDialogue) return;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > sightRange)
        {
            playerInSight = false;
            return;
        }
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerTransform.position - transform.position, out hit, sightRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerInSight = true;
                    return;
                }
            }
        
    }

    // Handle the state of the NPC
    private void HandleNPCStandardState()
    {
        switch (currentState)
        {
            case NPCState.Idle:
                // NPC does nothing, waits for an action
                IdleBehavior();
                break;
            case NPCState.Walking:
                // NPC moves towards a target position
                WalkBehavior();
                break;
            case NPCState.Working:
                // NPC performs an action or task
                WorkingBehavior();
                break;
            case NPCState.Dialogue:
                // NPC starts or continues a dialogue
                DialogueBehavior();
                break;
        }
    }

    // Idle behavior (NPC is doing nothing)
    protected virtual void IdleBehavior()
    {
        PlayAnimation("Idle");
    }

    // Walking behavior (NPC is moving to a target)
    protected virtual void WalkBehavior()
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(targetPosition);
        }
        PlayAnimation("Walking");
    }

    // Working behavior (NPC is completing a task)
    protected virtual void WorkingBehavior()
    {
        PlayAnimation("Working");
        // Example: NPC can be doing an animation or task in the scene
    }


    // Investigating behavior (NPC is actively investigating)
    protected virtual void InvestigatingBehavior()
    {
        PlayAnimation("Investigating");
        // NPC moves to investigate a specific area or spot
        if (navAgent != null)
        {
            navAgent.SetDestination(targetPosition);
        }
    }

    // Dialogue behavior (NPC is in dialogue with the player)
    protected virtual void DialogueBehavior()
    {
        StateIsInDialogue = true;
        PlayAnimation("Dialogue");
    }

    // Reset state to Idle
    private void ResetState()
    {
        StateIsBusy = false;
        StateIsInDialogue = false;
        currentState = NPCState.Idle; // Or set back to a different state
    }

    // Play the specified animation for the NPC
    private void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }

    // Additional helper functions like setting new target positions, changing states, etc.
    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    public void ChangeState(NPCState newState)
    {
        currentState = newState;
    }

    // For debugging purposes, display current state
    private void OnGUI()
    {
        GUILayout.Label("NPC State: " + currentState.ToString());
    }
}
