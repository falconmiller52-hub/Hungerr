#if UNITY_EDITOR
using Runtime.Common.InspectorFeatures.ButtonEditor;
using UnityEditor;

namespace Runtime.Common.Services.ItemsIdentifier
{
	[CustomEditor(typeof(ItemsIdentifierSO))]
	public class ItemsIdentifierEditor : ButtonEditor
	{
		private void OnEnable()
		{
			SetButtonName("Reload & Identify");
		}
	}
}
#endif