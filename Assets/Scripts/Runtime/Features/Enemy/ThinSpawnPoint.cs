using UnityEngine;

namespace Runtime.Features.Enemy
{
	public class ThinSpawnPoint : MonoBehaviour
	{
		[field: SerializeField] public Transform[] PatrolPoints { get; private set; }
	}
}
