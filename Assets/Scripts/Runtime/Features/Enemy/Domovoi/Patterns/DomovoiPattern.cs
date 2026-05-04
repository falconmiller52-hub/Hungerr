using UnityEngine;

namespace Runtime.Features.Enemy.Domovoi.Patterns
{
	public class DomovoiPattern : MonoBehaviour
	{
		public virtual void Trigger()
		{
			Debug.Log("DomovoiPattern triggered!");
		}

		public virtual void Clear()
		{
			// каждый паттерн сам хендлит свое уничтожение
		}
	}
}