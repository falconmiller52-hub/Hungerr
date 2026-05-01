#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Runtime.Common.InspectorFeatures.ButtonEditor
{
	[CustomEditor(typeof(IButtonPressedHandler))]
	public class ButtonEditor : Editor
	{
		private string _buttonName;

		protected void SetButtonName(string name) =>
			_buttonName = name;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var script = (IButtonPressedHandler)target;

			if (GUILayout.Button(_buttonName, GUILayout.Height(40)))
			{
				script.OnButtonPressed();
			}
		}
	}
}
#endif