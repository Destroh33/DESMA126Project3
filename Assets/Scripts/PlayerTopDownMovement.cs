using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDownMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    [SerializeField] float speed = 5f;
    [SerializeField] Vector3 fishingReturnSpawnPoint = new Vector3(0, 0, 0); // Set this in Inspector for the spawn point after fishing
    [SerializeField] GameObject DialogueCanvas;
    [SerializeField] InkDialoguePlayer inkDialoguePlayer;
    [SerializeField] NPCInteractionController npcInteractionController;

    private bool dialogueOpen = false;
    private Vector2 lastMoveInput;
    private Vector2 lastDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastDirection = Vector2.down;
        DialogueCanvas.SetActive(false);

        // Check if returning from fishing
        if (PlayerFishing.ReturningFromFishing)
        {
            transform.position = fishingReturnSpawnPoint;
            PlayerFishing.ReturningFromFishing = false; // Reset flag
        }
    }

    void OnMove(InputValue v)
    {
        lastMoveInput = v.Get<Vector2>();

        if (lastMoveInput != Vector2.zero)
        {
            lastDirection = lastMoveInput.normalized;
        }

        if (dialogueOpen)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        rb.linearVelocity = lastMoveInput * speed;
        float absX = Mathf.Abs(lastDirection.x);
        float absY = Mathf.Abs(lastDirection.y);
        if (absX >= absY)
        {
            animator.SetFloat("Horizontal", Mathf.Sign(lastDirection.x));
            animator.SetFloat("Vertical", 0f);
            spriteRenderer.flipX = lastDirection.x < 0;
        }
        else
        {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", Mathf.Sign(lastDirection.y));
            spriteRenderer.flipX = false;
        }
        animator.SetFloat("Speed", lastMoveInput.magnitude);
        
        if (lastDirection.x > 0)
            spriteRenderer.flipX = true;
        else if (lastDirection.x < 0)
            spriteRenderer.flipX = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("NPC"))
            return;

        var npc = collision.gameObject.GetComponent<NPCScript>();
        if (npc == null)
        {
            Debug.LogError("NPC GameObject is missing an NPCScript component.");
            return;
        }

        dialogueOpen = true;
        rb.linearVelocity = Vector2.zero;

        npcInteractionController.StartInteraction(npc);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("NPC"))
            return;

        npcInteractionController.EndInteraction();
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;
        DialogueCanvas.SetActive(false);
        rb.linearVelocity = lastMoveInput * speed;
    }
}
