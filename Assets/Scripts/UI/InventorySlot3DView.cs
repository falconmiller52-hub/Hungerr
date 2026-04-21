using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot3DView : MonoBehaviour
{
    public Button SlotButton;
    public Image Icon;
    public Text CountText;
    private IInventory inventory;
    private int slotIndex;
    public event Action<IInventory, int> OnSlotClicked;

    public void BindSlot(IInventory inv, int index)
    {
        inventory = inv;
        slotIndex = index;
        if (SlotButton != null)
        {
            SlotButton.onClick.RemoveAllListeners();
            SlotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(inv, index));
        }
        Refresh();
    }

    public void Refresh()
    {
        var stack = inventory?.GetSlot(slotIndex);
        if (Icon != null)
        {
            Icon.enabled = stack != null && stack.Count > 0;
            if (Icon.enabled && stack?.Item != null)
                Icon.sprite = stack.Item.Icon;
        }
        if (CountText != null)
        {
            CountText.text = stack != null ? stack.Count.ToString() : "";
        }
    }

    public void SetSelected(bool value)
    {
        if (Icon != null) Icon.color = value ? Color.yellow : Color.white;
    }
}
