using PrimeTween;
using TMPro;
using UnityEngine;

namespace Runtime.Features.Inventory.View.New
{
	public class InventoryItemView : MonoBehaviour
	{
		public InventoryItem Item { get; private set; }
    
		[Header("Settings")]
		[SerializeField] private float _moveDuration = 0.2f;
		[SerializeField] private Ease _moveEase = Ease.OutQuad;
		[SerializeField] private TMP_Text _amount;

		public void Setup(InventoryItem item)
		{
			Item = item;
			_amount.text = $"x:{item.Amount}"; 
		
			transform.localScale = new Vector3(item.Data.Width, 1, item.Data.Height);
		}

		public void UpdateVisualPosition(Vector3 targetLocalPos, bool immediate = false)
		{
			if (immediate)
			{
				transform.localPosition = targetLocalPos;
				return;
			}

			if (transform.localPosition == targetLocalPos)
				return;
		
			// PrimeTween сам остановит предыдущий твин на этом трансформе
			Tween.LocalPosition(transform, targetLocalPos, _moveDuration, _moveEase);
		}
	}
}