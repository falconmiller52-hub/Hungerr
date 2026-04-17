// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;
    
	[SerializeField] private GameObject tooltipPanel;
	[SerializeField] private Text tooltipNameText;
	[SerializeField] private Text tooltipDescriptionText;
	[SerializeField] private Text tooltipAmountText;
	[SerializeField] private GameObject interactPrompt;
	[SerializeField] private Text interactText;
    
	private WorldItem currentHoveredItem;
    
	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
        
		tooltipPanel.SetActive(false);
		interactPrompt.SetActive(false);
	}
    
	public void ShowTooltip(WorldItem item)
	{
		currentHoveredItem = item;
        
		tooltipNameText.text = item.GetDisplayName();
		tooltipDescriptionText.text = item.GetDescription();
        
		if (item.Instance.Amount > 1)
		{
			tooltipAmountText.text = $"x{item.Instance.Amount}";
			tooltipAmountText.enabled = true;
		}
		else
		{
			tooltipAmountText.enabled = false;
		}
        
		tooltipPanel.SetActive(true);
	}
    
	public void HideTooltip()
	{
		tooltipPanel.SetActive(false);
		currentHoveredItem = null;
	}
    
	public void ShowInteractPrompt(string text)
	{
		interactText.text = text;
		interactPrompt.SetActive(true);
	}
    
	public void HideInteractPrompt()
	{
		interactPrompt.SetActive(false);
	}
    
	private void Update()
	{
		if (tooltipPanel.activeSelf && currentHoveredItem != null)
		{
			tooltipPanel.transform.position = Input.mousePosition + new Vector3(15, 15);
		}
	}
}