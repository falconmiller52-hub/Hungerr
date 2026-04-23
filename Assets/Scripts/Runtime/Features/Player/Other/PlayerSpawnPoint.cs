using UnityEngine;

namespace Runtime.Features.Player.Other
{
	public class PlayerSpawnPoint : MonoBehaviour
	{
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, 0.5f);
			Gizmos.color = Color.white;
		}
#endif
	}
}