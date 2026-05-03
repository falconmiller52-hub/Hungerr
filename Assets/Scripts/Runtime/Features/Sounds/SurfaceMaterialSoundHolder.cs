using FMOD.Studio;
using FMODUnity;
using Runtime.Common.Enums;
using UnityEngine;

namespace Runtime.Features.Sounds
{
	public class SurfaceMaterialSoundHolder : MonoBehaviour
	{
		[Header("Surface Type")] [SerializeField]
		private SurfaceType _surfaceType;

		[Header("Surface sounds events")] [SerializeField]
		private EventReference _surfaceSoundEvent;

		public void SetSurfaceSoundEvent(EventInstance eventInstance)
			=> eventInstance.setParameterByName("surface", (int)_surfaceType);
	}
}