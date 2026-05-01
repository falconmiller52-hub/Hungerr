#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Runtime.Common.InspectorFeatures.ButtonEditor
{
	[CustomEditor(typeof(IButtonPressedHandler))]
	public class ButtonEditor : Editor
	{
		private string[] _buttonNames;

		protected void SetButtonName(string[] names) =>
			_buttonNames = names;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var script = (IButtonPressedHandler)target;

			foreach (var button in _buttonNames)
			{
				if (GUILayout.Button(button, GUILayout.Height(40)))
				{
					script.OnButtonPressed(button);
				}
			}
			
		}
	}
}
#endif