using UnityEngine;

namespace Runtime.Features.Enemy.Domovoi
{
	public class DomovoiPattern : MonoBehaviour
	{
		public virtual void Trigger()
		{
			Debug.Log("DomovoiPattern triggered!");
		}
	}
}