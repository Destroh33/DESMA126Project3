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

    // Static: survives scene loads automatically
    private static readonly Dictionary<FishType, int> counts = new();
    private static readonly Dictionary<FishType, int> typeToSlotIndex = new();
    private static readonly Dictionary<FishType, Sprite> sprites = new();
    private static int nextFreeSlot = 0;

    void Awake()
    {
        Instance = this;
        RefreshUI();
    }

    // Redraws all persisted fish data into this scene's UI slots
    void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (uiSlots[i].displayImage != null)
                uiSlots[i].displayImage.color = new Color(1f, 1f, 1f, 0f);
            if (uiSlots[i].countText != null)
                uiSlots[i].countText.text = "";
        }

        foreach (var kv in typeToSlotIndex)
        {
            FishType type = kv.Key;
            int index = kv.Value;
            if (index >= uiSlots.Length) continue;

            Slot slot = uiSlots[index];
            if (sprites.TryGetValue(type, out Sprite sp) && slot.displayImage != null)
            {
                slot.displayImage.sprite = sp;
                slot.displayImage.color = Color.white;
            }
            if (slot.countText != null && counts.TryGetValue(type, out int c))
                slot.countText.text = c.ToString();
        }
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
            if (fish.invImage != null)
                sprites[type] = fish.invImage;
        }
        else
        {
            Debug.Log("Inventory full.");
            return;
        }

        UpdateSlotUI(type, fish.invImage);
        Debug.Log($"Caught {type}! Count: {counts[type]}");
    }

    public bool RemoveFish(FishType type)
    {
        if (!counts.TryGetValue(type, out int count) || count <= 0)
            return false;

        if (count == 1)
        {
            counts.Remove(type);
            if (typeToSlotIndex.TryGetValue(type, out int idx))
            {
                Slot slot = uiSlots[idx];
                if (slot.displayImage != null)
                    slot.displayImage.color = new Color(1f, 1f, 1f, 0f);
                if (slot.countText != null)
                    slot.countText.text = "";
                typeToSlotIndex.Remove(type);
            }
            sprites.Remove(type);
        }
        else
        {
            counts[type] = count - 1;
            UpdateSlotUI(type, null);
        }
        return true;
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
            slot.countText.text = counts.TryGetValue(type, out int c) ? c.ToString() : "";
    }

    public bool TryGetTypeAtSlot(int slotIndex, out FishType type)
    {
        foreach (var kv in typeToSlotIndex)
        {
            if (kv.Value == slotIndex)
            {
                type = kv.Key;
                return true;
            }
        }
        type = default;
        return false;
    }

    public bool TryGetCount(FishType type, out int count) => counts.TryGetValue(type, out count);

    public bool TryGetSprite(FishType type, out Sprite sprite) => sprites.TryGetValue(type, out sprite);

    public IReadOnlyDictionary<FishType, int> GetCounts() => counts;
}
