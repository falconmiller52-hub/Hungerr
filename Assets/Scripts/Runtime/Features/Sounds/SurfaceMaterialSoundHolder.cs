using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Features.Sounds
{
	public class SurfaceMaterialSoundHolder : MonoBehaviour
	{
		[SerializeField, Label("Surface Material Sound")]
		private List<SoundData> _materialSounds;

		public List<SoundData> MaterialSounds => _materialSounds;
	}
}