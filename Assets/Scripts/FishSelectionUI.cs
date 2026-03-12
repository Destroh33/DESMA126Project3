using System;
using UnityEngine;
using UnityEngine.UI;


public class FishSelectionUI : MonoBehaviour
{
    [SerializeField] private Button[] slotButtons;  
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
            if (btn != null) btn.gameObject.SetActive(visible);

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(visible);
    }
}
