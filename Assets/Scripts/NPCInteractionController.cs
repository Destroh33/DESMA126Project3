using UnityEngine;

/// <summary>
/// Manages the full NPC interaction flow:
///   1. Load the correct opening story (first-meet vs. returning visit).
///   2. When the Ink story calls ~ TriggerFishSelection(), show the fish-selection UI.
///   3. After the player picks a fish, consume it from inventory and play the fish-specific story.
///   4. Mark the NPC as "met" after their first story completes.
///
/// Attach this to the same GameObject as PlayerTopDownMovement.
///
/// Inspector wiring required:
///   - dialoguePlayer   → the InkDialoguePlayer in the scene
///   - fishSelectionUI  → the FishSelectionUI component
///   - dialogueCanvas   → the dialogue Canvas GameObject
/// </summary>
public class NPCInteractionController : MonoBehaviour
{
    [SerializeField] private InkDialoguePlayer dialoguePlayer;
    [SerializeField] private FishSelectionUI fishSelectionUI;
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private PlayerTopDownMovement playerMovement;

    private NPCScript currentNPC;
    private bool waitingForFishSelection = false;

    void OnEnable()
    {
        dialoguePlayer.OnFishSelectionRequested += HandleFishSelectionRequested;
        dialoguePlayer.onStoryEnd.AddListener(HandleStoryEnd);
        fishSelectionUI.OnFishChosen += HandleFishChosen;
        fishSelectionUI.OnCancelled += HandleFishSelectionCancelled;
    }

    void OnDisable()
    {
        dialoguePlayer.OnFishSelectionRequested -= HandleFishSelectionRequested;
        dialoguePlayer.onStoryEnd.RemoveListener(HandleStoryEnd);
        fishSelectionUI.OnFishChosen -= HandleFishChosen;
        fishSelectionUI.OnCancelled -= HandleFishSelectionCancelled;
    }

    /// <summary>Called by PlayerTopDownMovement when the player touches an NPC.</summary>
    public void StartInteraction(NPCScript npc)
    {
        currentNPC = npc;
        waitingForFishSelection = false;

        TextAsset story = npc.GetCurrentStory();
        if (story == null)
        {
            Debug.LogWarning($"NPC '{npc.name}' has no story assigned for this interaction state.");
            return;
        }

        dialogueCanvas.SetActive(true);
        dialoguePlayer.LoadStory(story);
    }

    // --- Event handlers ---

    private void HandleFishSelectionRequested()
    {
        // The pre-apprentice story has ended after calling TriggerFishSelection.
        // Show the fish picker — dialogue canvas stays open.
        waitingForFishSelection = true;

        var inventory = FishingInventory.Instance;
        bool hasAnyFish = false;
        foreach (var kv in inventory.GetCounts())
        {
            if (kv.Value > 0) { hasAnyFish = true; break; }
        }

        if (!hasAnyFish)
        {
            // No fish to give — just close.
            Debug.Log("Player has no fish to show.");
            CloseDialogue();
            return;
        }

        fishSelectionUI.Show();
    }

    private void HandleFishChosen(FishType fishType)
    {
        waitingForFishSelection = false;

        // Consume one fish from inventory
        FishingInventory.Instance.RemoveFish(fishType);

        // Play the fish-specific story for this NPC
        if (currentNPC != null && currentNPC.TryGetFishStory(fishType, out TextAsset fishStory))
        {
            dialoguePlayer.LoadStory(fishStory);
        }
        else
        {
            Debug.LogWarning($"No story mapped for {fishType} on NPC '{currentNPC?.name}'.");
            CloseDialogue();
        }
    }

    private void HandleFishSelectionCancelled()
    {
        waitingForFishSelection = false;
        CloseDialogue();
    }

    private void HandleStoryEnd()
    {
        // Mark NPC as met if this was the first interaction
        if (currentNPC != null && !currentNPC.hasMetPlayer)
            currentNPC.MarkMet();

        // The dialogue canvas is closed by PlayerTopDownMovement.CloseDialogue(),
        // which is wired to onStoryEnd in the Inspector. No need to do it here.
        // But we do need to ensure the fish UI is hidden.
        fishSelectionUI.Hide();
    }

    /// <summary>Called by PlayerTopDownMovement when the player leaves the NPC's collision area.</summary>
    public void EndInteraction()
    {
        waitingForFishSelection = false;
        fishSelectionUI.Hide();
        currentNPC = null;
        playerMovement.CloseDialogue();
    }

    private void CloseDialogue()
    {
        fishSelectionUI.Hide();
        currentNPC = null;
        playerMovement.CloseDialogue();
    }
}
