using UnityEngine;
using System.Collections;

public enum NPCState
{
    Idle,
    Walking,
    Working,
    Suspicious,
    Investigating,
    Chasing,
    Dialogue // New state for handling dialogue
}

public class NPCbase : MonoBehaviour
{
    // Common NPC properties
    [SerializeField]
    private string npcName;
    [SerializeField]
    private float moveSpeed = 3f;
    private Vector3 targetPosition;
    private bool playerInSight;
    private NPCState currentState = NPCState.Idle;
    [SerializeField]
    private float sightRange = 5f;
    private LayerMask playerLayer;

    // Animation handling
    private Animator animator;

    // Dialogue handling
    public bool isInDialogue = false;

    // Working indicator
    public bool isBusy = false;

    // Reference to the player
    private Transform playerTransform;

    // NPC's pathfinding components (optional for complex movement)
    private UnityEngine.AI.NavMeshAgent navAgent;

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

    // Update is called once per frame
    protected virtual void Update()
    {
        // Perform NPC's state behavior
        HandleNPCState();

        // Check if player is within detection range
        CheckForPlayerDetection();
    }

    // Method for detecting the player
    protected virtual void CheckForPlayerDetection()
    {
        // Simple detection: If the player is within sight range, the NPC notices them
        if (isBusy || isInDialogue) return;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= sightRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerTransform.position - transform.position, out hit, sightRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerInSight = true;
                    currentState = NPCState.Suspicious;
                }
            }
        }
        else
        {
            playerInSight = false;
        }
    }

    // Handle the state of the NPC
    private void HandleNPCState()
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
            case NPCState.Suspicious:
                // NPC enters a suspicious state and starts investigating
                SuspiciousBehavior();
                break;
            case NPCState.Investigating:
                // NPC investigates a possible sighting
                InvestigatingBehavior();
                break;
            case NPCState.Chasing:
                // NPC starts chasing the player
                ChasingBehavior();
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
        isBusy = true;
        PlayAnimation("Working");
        // Example: NPC can be doing an animation or task in the scene
    }

    // Suspicious behavior (NPC noticed something unusual)
    protected virtual void SuspiciousBehavior()
    {
        PlayAnimation("Suspicious");
        // NPC may wander around or wait for more information
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

    // Chasing behavior (NPC chases the player)
    protected virtual void ChasingBehavior()
    {
        PlayAnimation("Chasing");
        // NPC chases the player if detected
        if (navAgent != null)
        {
            navAgent.SetDestination(playerTransform.position);
        }
    }

    // Dialogue behavior (NPC is in dialogue with the player)
    protected virtual void DialogueBehavior()
    {
        
    }

    // Reset state to Idle
    private void ResetState()
    {
        isInDialogue = false;
        isBusy = false;
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
