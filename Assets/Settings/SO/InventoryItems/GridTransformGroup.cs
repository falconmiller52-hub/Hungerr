using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTransformGroup : MonoBehaviour
{
    [SerializeField] private Vector2 _cellSize;
    [SerializeField] private Vector2 _spacing;
    [SerializeField] private int _columns;
    
    [ContextMenu("Arrange Group")]
    public void ArrangeGroup()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                int row = i / _columns;
                int col = i % _columns;
                Vector3 newPos = new Vector3
                (
                    col * (_cellSize.x + _spacing.x),
                    -row * (_cellSize.y + _spacing.y),
                    0
                );

                child.localPosition = newPos;
            }
        }
    }
}
