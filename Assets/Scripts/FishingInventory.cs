using System.Collections.Generic;
using UnityEngine;

public class FishingInventory : MonoBehaviour
{
    public static FishingInventory Instance { get; private set; }

    // Each slot: a unique FishType and how many have been caught
    private readonly Dictionary<FishType, int> slots = new Dictionary<FishType, int>();

    // Max distinct fish types that can fill slots (one slot per type)
    public const int MaxSlots = 5;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddFish(FishType type)
    {
        if (slots.ContainsKey(type))
        {
            slots[type]++;
        }
        else if (slots.Count < MaxSlots)
        {
            slots[type] = 1;
        }
        else
        {
            Debug.Log("Inventory full — no empty slot for a new fish type.");
            return;
        }

        Debug.Log($"Caught {type}! Count: {slots[type]}");
    }

    public bool TryGetCount(FishType type, out int count) => slots.TryGetValue(type, out count);

    public IReadOnlyDictionary<FishType, int> GetSlots() => slots;
}
