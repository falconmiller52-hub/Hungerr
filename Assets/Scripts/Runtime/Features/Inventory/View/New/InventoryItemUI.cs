using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text name;
	private RectTransform rectTransform;

	public void Setup(Sprite sprite, int width, int height, float cellSize)
	{
		rectTransform = GetComponent<RectTransform>();
		icon.sprite = sprite;

		name.text = "pp";
		
		// Устанавливаем размер UI элемента исходя из размера сетки
		rectTransform.sizeDelta = new Vector2(width * cellSize, height * cellSize);
	}

	public void SetPosition(Vector2Int gridPos, float cellSize)
	{
		rectTransform = GetComponent<RectTransform>();
		rectTransform.pivot = new Vector2(0, 1);
		rectTransform.anchoredPosition = new Vector2(gridPos.x * cellSize, -gridPos.y * cellSize);
		
		name.text = $"x{gridPos.x} \n y{gridPos.y}";
	}
}