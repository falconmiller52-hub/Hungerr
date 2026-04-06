using System;
using UnityEngine;

namespace Runtime.Features.Location
{
	[Serializable]
	public struct LocationChangerData
	{
		[field: SerializeField] public float FadeInDuration { get; private set; }
		[field:SerializeField] public float StayBlackDuration { get; private set; }
		[field:SerializeField] public float FadeOutDuration { get; private set; }
		
		public LocationChangerData(float  fadeInDuration, float stayBlackDuration, float fadeOutDuration)
		{
			FadeInDuration = fadeInDuration;
			StayBlackDuration = stayBlackDuration;
			FadeOutDuration = fadeOutDuration;
		}
	}
}