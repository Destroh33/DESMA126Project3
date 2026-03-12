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

    [Tooltip("Shown on all subsequent visits before apprenticed — should call ~ TriggerFishSelection() in Ink.")]
    public TextAsset preApprenticeStory;

    [Tooltip("Shown when the NPC has 3+ fish but hasn't been apprenticed yet. No fish selection — ends by triggering apprenticeship.")]
    public TextAsset askApprenticeStory;

    [Tooltip("Shown after the NPC is apprenticed — should call ~ TriggerFishSelection() in Ink.")]
    public TextAsset postApprenticeStory;

    [Tooltip("Played after the NPC receives all 5 fish. After it ends, StartingScreen loads.")]
    public TextAsset endingStory;

    [Header("Fish Gift Stories")]
    [Tooltip("Map each FishType to the Ink story that plays when that fish is given.")]
    public FishStoryMapping[] fishStoryMappings;

    [Header("Apprentice Spawn")]
    [Tooltip("World position the NPC teleports to every time the scene loads while they are apprenticed.")]
    public Vector3 postApprenticePosition;

    [Header("Runtime State")]
    [Tooltip("Unique identifier used for persistence. Leave empty to use the GameObject name.")]
    public string npcID;

    // --- Static persistent state ---
    private static HashSet<string> metNPCs = new HashSet<string>();
    private static HashSet<string> apprenticedNPCs = new HashSet<string>();
    private static Dictionary<string, HashSet<FishType>> fishGivenByNPC = new Dictionary<string, HashSet<FishType>>();

    private string ID => string.IsNullOrEmpty(npcID) ? gameObject.name : npcID;

    public bool hasMetPlayer => metNPCs.Contains(ID);
    public bool isApprenticed => apprenticedNPCs.Contains(ID);

    // True if any NPC across the game is currently apprenticed
    public static bool HasAnyApprenticed => apprenticedNPCs.Count > 0;

    // Returns the ID of the apprenticed NPC, or null if none
    public static string GetApprenticedID()
    {
        foreach (var id in apprenticedNPCs)
            return id;
        return null;
    }

    void Start()
    {
        if (isApprenticed)
            transform.position = postApprenticePosition;
    }

    public TextAsset GetCurrentStory()
    {
        if (isApprenticed)
        {
            if (GetFishGivenCount() >= 5)
                return endingStory;
            return postApprenticeStory;
        }

        if (!hasMetPlayer)
            return firstInteractionStory;

        if (GetFishGivenCount() >= 3)
            return askApprenticeStory;

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

    public void MarkApprenticed()
    {
        apprenticedNPCs.Add(ID);
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

    public int GetFishGivenCount()
    {
        if (fishGivenByNPC.TryGetValue(ID, out var set))
            return set.Count;
        return 0;
    }
}
