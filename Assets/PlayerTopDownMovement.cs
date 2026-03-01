using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDownMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject DialogueCanvas;
    [SerializeField] InkDialoguePlayer inkDialoguePlayer;

    private bool dialogueOpen = false;
    private Vector2 lastMoveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DialogueCanvas.SetActive(false);
    }

    void OnMove(InputValue v)
    {
        lastMoveInput = v.Get<Vector2>();

        if (dialogueOpen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = lastMoveInput * speed;
    }

    void Update()
    {
        if (!dialogueOpen || inkDialoguePlayer.currentStory == null)
            return;

        // Detect story end
        if (!inkDialoguePlayer.currentStory.canContinue &&
            inkDialoguePlayer.currentStory.currentChoices.Count == 0)
        {
            CloseDialogue();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("NPC"))
            return;

        var npc = collision.gameObject.GetComponent<NPCScript>();
        if (npc == null || npc.storyJSON == null)
        {
            Debug.LogError("NPC missing storyJSON.");
            return;
        }

        dialogueOpen = true;
        rb.linearVelocity = Vector2.zero;

        DialogueCanvas.SetActive(true);
        inkDialoguePlayer.LoadStory(npc.storyJSON);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("NPC"))
            return;

        CloseDialogue();
    }

    private void CloseDialogue()
    {
        dialogueOpen = false;
        DialogueCanvas.SetActive(false);
        rb.linearVelocity = lastMoveInput * speed;
    }
}