using UnityEngine;

namespace Runtime.Features.ItemSpawner
{
	public class ItemSpawnPoint : MonoBehaviour
	{
		[SerializeField, Tooltip("Информация о том, какие предметы и какие шансы у предметов для этой точки")]
		private ItemSpawnTierData _tierData;

		[SerializeField, Tooltip("Размер предмета который будет заспавнен")]
		private Vector3 _itemScale = Vector3.one;

		public ItemSpawnTierData TierData => _tierData;
		public Vector3 ItemScale => _itemScale;
		public int ID => gameObject.scene.name.GetHashCode() + gameObject.name.GetHashCode();
	}
}