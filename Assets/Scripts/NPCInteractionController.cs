using UnityEngine;

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


    private void HandleFishSelectionRequested()
    {
        waitingForFishSelection = true;

        var inventory = FishingInventory.Instance;
        bool hasAnyFish = false;
        foreach (var kv in inventory.GetCounts())
        {
            if (kv.Value > 0) { hasAnyFish = true; break; }
        }

        if (!hasAnyFish)
        {
            Debug.Log("Player has no fish to show.");
            CloseDialogue();
            return;
        }

        fishSelectionUI.Show();
    }

    private void HandleFishChosen(FishType fishType)
    {
        waitingForFishSelection = false;

        FishingInventory.Instance.RemoveFish(fishType);

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
        if (currentNPC != null && !currentNPC.hasMetPlayer)
            currentNPC.MarkMet();

        fishSelectionUI.Hide();
    }

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
