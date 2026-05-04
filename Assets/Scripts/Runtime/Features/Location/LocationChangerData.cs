using System;
using UnityEngine;

namespace Runtime.Features.Location
{
	[Serializable]
	public class LocationChangerData
	{
		[field: SerializeField, Tooltip("Длительность появления шторки, в секундах")]
		public float FadeInDuration { get; private set; } = 0.5f;
		
		[field:SerializeField] 
		public float StayBlackDuration { get; private set; }

		[field: SerializeField, Tooltip("Длительность исчезания шторки, в секундах")]
		public float FadeOutSpeed { get; private set; } = 0.5f;
	}
}