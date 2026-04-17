// WorldItem.cs
using UnityEngine;
using Runtime.Features.Interactable;
using Runtime.Features.Inventory; // если используешь UniTask, иначе замени на_coroutine

[RequireComponent(typeof(MeshRenderer))]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private InventoryItemData itemData;
    [SerializeField] private int amount = 1;
    
    private InventoryItem instance;
    private MeshRenderer meshRenderer;
    private Outline outline;
    private bool isHovered = false;
    
    public InventoryItem Instance => instance;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
        
        instance = new InventoryItem(itemData, amount);
        HideOutline();
    }
    
    public void Initialize(InventoryItemData data, int count = 1)
    {
        itemData = data;
        amount = count;
        instance = new InventoryItem(data, count);
        
        if (data.WorldPrefab != null)
        {
            var prefab = Instantiate(data.WorldPrefab, transform);
            prefab.transform.localPosition = Vector3.zero;
        }
    }
    
    public void OnHoverEnter()
    {
        isHovered = true;
        ShowOutline();
        UIManager.Instance?.ShowTooltip(this);
        UIManager.Instance?.ShowInteractPrompt("F ВЗЯТЬ");
    }
    
    public void OnHoverExit()
    {
        isHovered = false;
        HideOutline();
        UIManager.Instance?.HideTooltip();
        UIManager.Instance?.HideInteractPrompt();
    }
    
    private void ShowOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }
    
    private void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
    
    public void Interact(GameObject player)
    {
        var inventoryComponent = player.GetComponent<PlayerInventory>();
        if (inventoryComponent != null)
        {
            if (inventoryComponent.AddItem(instance))
            {
                Destroy(gameObject);
            }
        }
    }
    
    public InventoryItemData GetItemData() => itemData;
    public string GetDisplayName() => itemData.ItemName;
    public string GetDescription() => itemData.Description;
}