#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false; // Выключаем возможность редактирования
		EditorGUI.PropertyField(position, property, label);
		GUI.enabled = true;  // Включаем обратно для остальных элементов
	}
}
#endif