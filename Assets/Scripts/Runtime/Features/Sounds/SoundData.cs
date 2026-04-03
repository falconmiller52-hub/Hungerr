using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.Features.Sounds
{
	[CreateAssetMenu(fileName = "NewSoundData", menuName = "Custom Audio/Sound Data")]
	public class SoundData : ScriptableObject
	{
		[field: SerializeField] public AudioClip Clip { get; private set; }
		[field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
		[field: SerializeField, Range(0, 1)] public float Volume { get; private set; } = 1f;
		[field: SerializeField, MinMaxSlider(-3f, 3f)] public Vector2 PitchRange { get; private set; } = new Vector2(1f, 1f);
		[field: SerializeField] public bool Loop { get; private set; }

		[field: Header("3D Settings")]
		[field: SerializeField, Range(0, 1)] public float SpatialBlend { get; private set; } = 1f; // 1 = 3D, 0 = 2D
		[field: SerializeField] public float MinDistance { get; private set; } = 1f;
		[field: SerializeField] public float MaxDistance { get; private set; } = 20f;
		[field: SerializeField] public AudioRolloffMode RolloffMode { get; private set; } = AudioRolloffMode.Logarithmic;
	}
}