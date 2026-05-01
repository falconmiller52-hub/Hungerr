using Runtime.Common.InspectorFeatures.ButtonEditor;
using UnityEngine;

namespace Runtime.Features.ItemSpawner
{
	public class ItemSpawnPoint : MonoBehaviour, IButtonPressedHandler
	{
		[SerializeField, Tooltip("Информация о том, какие предметы и какие шансы у предметов для этой точки")]
		private ItemSpawnTierData _tierData;

		[SerializeField, Tooltip("Размер предмета который будет заспавнен")]
		private Vector3 _itemScale = Vector3.one;

		[Tooltip("Уникальный ID")]
		[SerializeField, ReadOnly] private string _spawnPointGuid;
		public ItemSpawnTierData TierData => _tierData;
		public Vector3 ItemScale => _itemScale;
		public string ID => _spawnPointGuid;

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (string.IsNullOrEmpty(_spawnPointGuid) || IsDuplicateGuidInScene())
			{
				Debug.LogError($"{gameObject.name} has an invalid guid ==> please Generate new");
			}
		}

		private bool IsDuplicateGuidInScene()
		{
			var all = FindObjectsByType<ItemSpawnPoint>(FindObjectsSortMode.None);
			int same = 0;
			
			foreach (var sp in all)
			{
				if (sp == this) 
					continue;

				if (sp._spawnPointGuid == this._spawnPointGuid)
				{
					Debug.LogWarning($"Duplicate GUID found on {sp.name}", sp); 
					same++;
				}
			}

			return same > 0;
		}
#endif
		
		public void OnButtonPressed(string name)
		{
#if UNITY_EDITOR
			GenerateUniqID();
#endif
		}

		private void GenerateUniqID()
		{
			_spawnPointGuid = System.Guid.NewGuid().ToString();
			// UnityEditor.EditorUtility.SetDirty(this);
		}
	}
}