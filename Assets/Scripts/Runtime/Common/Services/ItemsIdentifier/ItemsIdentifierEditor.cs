#if UNITY_EDITOR
using Runtime.Common.InspectorFeatures.ButtonEditor;
using Runtime.Features.DayNight;
using UnityEditor;

namespace Runtime.Common.Services.ItemsIdentifier
{
	[CustomEditor(typeof(ItemsIdentifierSO))]
	public class ItemsIdentifierEditor : ButtonEditor
	{
		private void OnEnable()
		{
			SetButtonName(new []{"Reload & Identify"});
		}
	}
	
	[CustomEditor(typeof(DayCycleVisualChanger))]
	public class DayCycleVisualChangerEditor : ButtonEditor
	{
		private void OnEnable()
		{
			SetButtonName(new [] { "Set Night", "Set Day" });
		}
	}
}
#endif