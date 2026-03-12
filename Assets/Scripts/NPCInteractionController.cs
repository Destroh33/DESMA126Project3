using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteractionController : MonoBehaviour
{
    [SerializeField] private InkDialoguePlayer dialoguePlayer;
    [SerializeField] private FishSelectionUI fishSelectionUI;
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private PlayerTopDownMovement playerMovement;

    private NPCScript currentNPC;
    private bool waitingForFishSelection = false;
    private bool sceneTransitioning = false;

    // Pending post-fish-story transitions
    private bool pendingApprentice = false;  // load FishingScene after ask-apprentice story ends
    private bool pendingEndGame = false;     // load StartingScreen after ending story ends

    void OnEnable()
    {
        dialoguePlayer.OnFishSelectionRequested += HandleFishSelectionRequested;
        dialoguePlayer.OnApprenticeAccepted += HandleApprenticeAccepted;
        dialoguePlayer.onStoryEnd.AddListener(HandleStoryEnd);
        fishSelectionUI.OnFishChosen += HandleFishChosen;
        fishSelectionUI.OnCancelled += HandleFishSelectionCancelled;
    }

    void OnDisable()
    {
        dialoguePlayer.OnFishSelectionRequested -= HandleFishSelectionRequested;
        dialoguePlayer.OnApprenticeAccepted -= HandleApprenticeAccepted;
        dialoguePlayer.onStoryEnd.RemoveListener(HandleStoryEnd);
        fishSelectionUI.OnFishChosen -= HandleFishChosen;
        fishSelectionUI.OnCancelled -= HandleFishSelectionCancelled;
    }

    public void StartInteraction(NPCScript npc)
    {
        currentNPC = npc;
        waitingForFishSelection = false;
        pendingApprentice = false;
        pendingEndGame = false;

        TextAsset story = npc.GetCurrentStory();
        if (story == null)
        {
            Debug.LogWarning($"NPC '{npc.name}' has no story assigned for this interaction state.");
            return;
        }

        if (story == npc.endingStory)
            pendingEndGame = true;

        dialogueCanvas.SetActive(true);
        dialoguePlayer.LoadStory(story);
    }

    private void HandleApprenticeAccepted()
    {
        pendingApprentice = true;
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

        if (currentNPC != null && currentNPC.HasBeenGivenFish(fishType))
        {
            dialoguePlayer.ShowOneLineAndEnd(currentNPC.name, "Hmm.. I've seen that already.");
            return;
        }

        FishingInventory.Instance.RemoveFish(fishType);
        if (currentNPC != null) currentNPC.MarkFishGiven(fishType);

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
        // After the ending story finishes → load StartingScreen
        if (pendingEndGame)
        {
            pendingEndGame = false;
            sceneTransitioning = true;
            fishSelectionUI.Hide();
            SceneManager.LoadScene("StartingScreen");
            return;
        }

        // After the 3rd fish story → mark apprenticed and go to FishingScene
        if (pendingApprentice)
        {
            pendingApprentice = false;
            sceneTransitioning = true;
            if (currentNPC != null) currentNPC.MarkApprenticed();
            fishSelectionUI.Hide();
            playerMovement.CloseDialogue();
            SceneManager.LoadScene("FishingScene");
            return;
        }

        // Normal story end
        if (currentNPC != null && !currentNPC.hasMetPlayer)
            currentNPC.MarkMet();

        fishSelectionUI.Hide();
    }

    public void EndInteraction()
    {
        if (sceneTransitioning) return;
        waitingForFishSelection = false;
        fishSelectionUI.Hide();
        currentNPC = null;
        playerMovement.CloseDialogue();
    }

    private void CloseDialogue()
    {
        if (sceneTransitioning) return;
        fishSelectionUI.Hide();
        currentNPC = null;
        playerMovement.CloseDialogue();
    }
}
