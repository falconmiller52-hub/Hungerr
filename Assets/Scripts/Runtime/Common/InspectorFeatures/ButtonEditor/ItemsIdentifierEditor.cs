#if UNITY_EDITOR
using Runtime.Common.Services.ItemsIdentifier;
using Runtime.Features.DayNight;
using Runtime.Features.ItemSpawner;
using UnityEditor;

namespace Runtime.Common.InspectorFeatures.ButtonEditor
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
	
	[CustomEditor(typeof(ItemSpawnPoint))]
	public class ItemSpawnPointEditor : ButtonEditor
	{
		private void OnEnable()
		{
			SetButtonName(new [] { "Generate Uniqe ID" });
		}
	}
}
#endif