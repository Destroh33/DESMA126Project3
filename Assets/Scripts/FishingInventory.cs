using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct Slot
{
    public Image displayImage;
    public TextMeshProUGUI countText;
}

public class FishingInventory : MonoBehaviour
{
    public static FishingInventory Instance { get; private set; }

    public const int MaxSlots = 5;

    [SerializeField] private Slot[] uiSlots = new Slot[MaxSlots];

    private readonly Dictionary<FishType, int> counts = new();
    private readonly Dictionary<FishType, int> typeToSlotIndex = new();
    private int nextFreeSlot = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddFish(Fish fish)
    {
        FishType type = fish.fishType;
        bool isNew = !counts.ContainsKey(type);

        if (!isNew)
        {
            counts[type]++;
        }
        else if (nextFreeSlot < MaxSlots)
        {
            counts[type] = 1;
            typeToSlotIndex[type] = nextFreeSlot++;
        }
        else
        {
            Debug.Log("Inventory full.");
            return;
        }

        UpdateSlotUI(type, isNew ? fish.invImage : null);
        Debug.Log($"Caught {type}! Count: {counts[type]}");
    }

    void UpdateSlotUI(FishType type, Sprite newSprite)
    {
        if (!typeToSlotIndex.TryGetValue(type, out int index)) return;
        Slot slot = uiSlots[index];

        if (newSprite != null && slot.displayImage != null)
        {
            slot.displayImage.sprite = newSprite;
            slot.displayImage.color = Color.white;
        }

        if (slot.countText != null)
            slot.countText.text = counts[type].ToString();
    }

    public bool TryGetCount(FishType type, out int count) => counts.TryGetValue(type, out count);

    public IReadOnlyDictionary<FishType, int> GetCounts() => counts;
}
