using System.Collections.Generic;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Features.Sounds
{
	public class SurfaceMaterialSoundHolder : MonoBehaviour
	{
		[SerializeField, Label("Surface Material Sound")]
		private List<EventReference> _materialSounds;
		
		public List<EventReference> MaterialSounds => _materialSounds;
	}
}