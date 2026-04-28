using TMPro;
using UnityEngine;

namespace Runtime.Features.Inventory.WorldItem
{
	public class TooltipController : MonoBehaviour
	{
		public static TooltipController Instance;
    
		[SerializeField] private GameObject _tooltipPanel;
		[SerializeField] private TMP_Text _tooltipNameText;
		[SerializeField] private TMP_Text _tooltipDescriptionText;
		[SerializeField] private TMP_Text _tooltipAmountText;
    
		private WorldItem _currentHoveredItem;
    
		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
        
			_tooltipPanel.SetActive(false);
		}
    
		public void ShowTooltip(WorldItem item)
		{
			_currentHoveredItem = item;
        
			_tooltipNameText.text = item.GetDisplayName();
			_tooltipDescriptionText.text = item.GetDescription();
        
			if (item.GetItem().Amount > 1)
			{
				_tooltipAmountText.text = $"x{item.GetItem().Amount}";
				_tooltipAmountText.enabled = true;
			}
			else
			{
				_tooltipAmountText.enabled = false;
			}
        
			_tooltipPanel.SetActive(true);
		}
    
		public void HideTooltip()
		{
			_tooltipPanel.SetActive(false);
			_currentHoveredItem = null;
		}
    
		private void Update()
		{
			if (_tooltipPanel.activeSelf && _currentHoveredItem != null)
			{
				_tooltipPanel.transform.position = Input.mousePosition + new Vector3(15, 15);
			}
		}
	}
}