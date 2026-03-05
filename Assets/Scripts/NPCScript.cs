using UnityEngine;

[System.Serializable]
public struct FishStoryMapping
{
    public FishType fishType;
    public TextAsset story;
}

public class NPCScript : MonoBehaviour
{
    [Header("Interaction Stories")]
    [Tooltip("Shown the very first time the player meets this NPC.")]
    public TextAsset firstInteractionStory;

    [Tooltip("Shown on all subsequent visits — should call ~ TriggerFishSelection() in Ink.")]
    public TextAsset preApprenticeStory;

    [Header("Fish Gift Stories")]
    [Tooltip("Map each FishType to the Ink story that plays when that fish is given.")]
    public FishStoryMapping[] fishStoryMappings;

    [Header("Runtime State")]
    [Tooltip("Automatically set to true after the first interaction completes.")]
    public bool hasMetPlayer = false;

    /// <summary>Returns the correct opening story based on whether the player has met this NPC.</summary>
    public TextAsset GetCurrentStory()
    {
        if (!hasMetPlayer || preApprenticeStory == null)
            return firstInteractionStory;

        return preApprenticeStory;
    }

    /// <summary>Returns the Ink story to play when the player gives this NPC the specified fish type.
    /// Returns false if no mapping exists for that type.</summary>
    public bool TryGetFishStory(FishType fishType, out TextAsset story)
    {
        story = null;
        foreach (var mapping in fishStoryMappings)
        {
            if (mapping.fishType == fishType)
            {
                story = mapping.story;
                return story != null;
            }
        }
        return false;
    }

    public void MarkMet()
    {
        hasMetPlayer = true;
    }
}
