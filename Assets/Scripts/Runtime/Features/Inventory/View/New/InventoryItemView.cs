using UnityEngine;
using PrimeTween;
using Runtime.Features.Inventory;

public class InventoryItemView : MonoBehaviour
{
	public InventoryItem Item { get; private set; }
    
	[Header("Settings")]
	[SerializeField] private float moveDuration = 0.2f;
	[SerializeField] private Ease moveEase = Ease.OutQuad;

	public void Setup(InventoryItem item)
	{
		Item = item;
		// Здесь можно обновить меш или текст, если нужно
	}

	public void UpdateVisualPosition(Vector3 targetLocalPos, bool immediate = false)
	{
		if (immediate)
		{
			transform.localPosition = targetLocalPos;
			return;
		}

		// PrimeTween сам остановит предыдущий твин на этом трансформе
		Tween.LocalPosition(transform, targetLocalPos, moveDuration, moveEase);
	}
}