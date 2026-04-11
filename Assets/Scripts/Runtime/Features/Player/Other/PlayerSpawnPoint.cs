using System;
using UnityEngine;

namespace Runtime.Features.Player.Other
{
	// ASK: Есть вопрос о том не стоит ли эту точку сделать более абстрактной?  Я в данный момент не придумал куда её
	// ASK: можно было бы её положить, поэтому пока что так.
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