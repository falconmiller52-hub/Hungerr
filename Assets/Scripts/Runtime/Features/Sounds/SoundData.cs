using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.Features.Sounds
{
	[CreateAssetMenu(fileName = "NewSoundData", menuName = "Custom Audio/Sound Data")]
	public class SoundData : ScriptableObject
	{
		[field: SerializeField, BoxGroup("Main Settings"), Tooltip("Аудио-файл, который будет воспроизведен.")]
		public AudioClip Clip { get; private set; }

		[field: SerializeField, BoxGroup("Main Settings"), Tooltip("Группа в Audio Mixer (Master, Music, SFX), через которую пойдет звук для управления эффектами и общей громкостью.")]
		public AudioMixerGroup MixerGroup { get; private set; }

		[field: SerializeField, BoxGroup("Main Settings"), Range(0, 1), Tooltip("Базовая громкость этого конкретного звука (умножается на громкость группы в микшере).")]
		public float Volume { get; private set; } = 1f;

		[field: SerializeField, BoxGroup("Main Settings"), MinMaxSlider(-3f, 3f),
		        Tooltip("Диапазон случайного изменения высоты звука (Pitch). Одинаковые значения (1, 1) отключают рандомизацию. Помогает избежать монотонности шагов или выстрелов.")]
		public Vector2 PitchRange { get; private set; } = new Vector2(1f, 1f);

		[field: SerializeField, BoxGroup("Main Settings"), Tooltip("Если включено, звук будет повторяться бесконечно (удобно для эмбиента или огня).")]
		public bool Loop { get; private set; }

		[field: Header("3D Settings")]
		[field: SerializeField, Range(0, 1), Tooltip("Степень влияния 3D пространства. 0 — звук слышно везде одинаково (UI, Музыка), 1 — звук зависит от позиции источника относительно игрока.")]
		public float SpatialBlend { get; private set; } = 1f;

		[field: SerializeField, Min(0), Tooltip("Расстояние, ближе которого звук всегда звучит на 100% громкости. Внутри этого радиуса затухание не работает.")]
		public float MinDistance { get; private set; } = 1f;

		[field: SerializeField, Tooltip("Расстояние, на котором звук полностью затихает. За пределами этого радиуса игроку ничего не слышно.")]
		public float MaxDistance { get; private set; } = 20f;

		[field: SerializeField, Tooltip("Математическая модель затухания. Logarithmic — реалистично (быстро затихает вблизи), Linear — затихает равномерно с расстоянием.")]
		public AudioRolloffMode RolloffMode { get; private set; } = AudioRolloffMode.Logarithmic;
	}
}