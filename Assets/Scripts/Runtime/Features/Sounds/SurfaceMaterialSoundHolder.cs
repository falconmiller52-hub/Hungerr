using FMOD.Studio;
using Runtime.Common.Enums;
using UnityEngine;

namespace Runtime.Features.Sounds
{
	public class SurfaceMaterialSoundHolder : MonoBehaviour
	{
		[Header("Surface Type")] [SerializeField]
		private SurfaceType _surfaceType;
		
		public void SetSurfaceSoundEvent(EventInstance eventInstance)
			=> eventInstance.setParameterByName("surface", (int)_surfaceType);
			//=> eventInstance.setParameterByNameWithLabel("surface", _surfaceType.ToString());
	}
}