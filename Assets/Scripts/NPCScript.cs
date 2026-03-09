using UnityEngine;
using System.Collections.Generic;
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
    [Tooltip("Unique identifier used for persistence. Leave empty to use the GameObject name.")]
    public string npcID;

    private static HashSet<string> metNPCs = new HashSet<string>();

    private static Dictionary<string, HashSet<FishType>> fishGivenByNPC = new Dictionary<string, HashSet<FishType>>();

    private string ID => string.IsNullOrEmpty(npcID) ? gameObject.name : npcID;

    public bool hasMetPlayer
    {
        get => metNPCs.Contains(ID);
    }

    public TextAsset GetCurrentStory()
    {
        if (!hasMetPlayer || preApprenticeStory == null)
            return firstInteractionStory;

        return preApprenticeStory;
    }

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
        metNPCs.Add(ID);
    }

    public void MarkFishGiven(FishType fish)
    {
        if (!fishGivenByNPC.TryGetValue(ID, out var set))
        {
            set = new HashSet<FishType>();
            fishGivenByNPC[ID] = set;
        }
        set.Add(fish);
    }

    public bool HasBeenGivenFish(FishType fish)
    {
        return fishGivenByNPC.TryGetValue(ID, out var set) && set.Contains(fish);
    }
}
