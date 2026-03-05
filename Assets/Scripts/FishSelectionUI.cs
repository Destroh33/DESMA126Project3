using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Turns the existing fishing inventory UI into an interactive fish-picker
/// by overlaying a Button on each slot.
///
/// Setup in Inspector:
///   - slotButtons : one Button per inventory slot, parented/overlaid ON TOP of each
///                   existing inventory slot image. Order must match FishingInventory's
///                   uiSlots order (slot 0, 1, 2, 3, 4).
///   - cancelButton: a "Give Nothing" button placed above/near the inventory UI.
///
/// The inventory images and count texts update automatically via FishingInventory
/// when RemoveFish is called — no extra work needed here.
/// </summary>
public class FishSelectionUI : MonoBehaviour
{
    [SerializeField] private Button[] slotButtons;   // one per inventory slot
    [SerializeField] private Button cancelButton;

    public event Action<FishType> OnFishChosen;
    public event Action OnCancelled;

    void Awake()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int captured = i;
            slotButtons[i].onClick.AddListener(() => HandleSlotClicked(captured));
        }

        if (cancelButton != null)
            cancelButton.onClick.AddListener(HandleCancel);

        SetButtonsVisible(false);
    }

    /// <summary>Enable only the buttons whose slots actually contain fish.</summary>
    public void Show()
    {
        var inventory = FishingInventory.Instance;
        for (int i = 0; i < slotButtons.Length; i++)
        {
            bool hasFish = inventory.TryGetTypeAtSlot(i, out _);
            slotButtons[i].gameObject.SetActive(hasFish);
        }

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        SetButtonsVisible(false);
    }

    private void HandleSlotClicked(int slotIndex)
    {
        if (!FishingInventory.Instance.TryGetTypeAtSlot(slotIndex, out FishType type))
            return;

        Hide();
        OnFishChosen?.Invoke(type);
    }

    private void HandleCancel()
    {
        Hide();
        OnCancelled?.Invoke();
    }

    private void SetButtonsVisible(bool visible)
    {
        foreach (var btn in slotButtons)
            btn.gameObject.SetActive(visible);

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(visible);
    }
}
