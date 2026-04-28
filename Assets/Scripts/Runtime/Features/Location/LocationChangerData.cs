using System;
using UnityEngine;

namespace Runtime.Features.Location
{
	[Serializable]
	public class LocationChangerData
	{
		[field: SerializeField, Tooltip("Время обновления появления шторки, раз в X секунд"), Range(0.0001f, 0.05f)]
		public float FadeInSpeed { get; private set; } = 0.005f;
		
		[field:SerializeField] 
		public float StayBlackDuration { get; private set; }

		[field: SerializeField, Tooltip("Время обновления исчезания шторки, раз в X секунд"), Range(0.0001f, 0.05f)]
		public float FadeOutSpeed { get; private set; } = 0.005f;
	}
}