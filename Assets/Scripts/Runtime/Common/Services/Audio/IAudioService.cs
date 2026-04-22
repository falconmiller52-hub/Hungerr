using System;
using FMODUnity;
using UnityEngine;

namespace Runtime.Common.Services.Audio
{
	public interface IAudioService
	{
		/// <summary>
		/// Простой проигрыш звука в 3D пространстве или 2D (если position default).
		/// Идеально для коротких эффектов (клики, удары). Нельзя остановить вручную.
		/// </summary>
		public void PlaySfx(EventReference eventRef, Vector3 position = default, Action<EventReference> onEnd = null, float volume = 1f);

		/// <summary>
		/// Запуск фонового звука или музыки с плавным переходом (Crossfade).
		/// </summary>
		/// <param name="eventRef">Ссылка на событие FMOD</param>
		/// <param name="fadeDuration">Длительность перехода в секундах</param>
		/// <param name="targetVolume">Целевая громкость нового звука</param>
		void PlayAmbient(EventReference eventRef, float fadeDuration = 1f, float targetVolume = 1f);

		/// <summary>
		/// Останавливает все активные инстансы указанного события.
		/// </summary>
		/// <param name="eventRef">Какое событие остановить</param>
		/// <param name="immediate">True — оборвать мгновенно, False — проиграть Fade-out/Release, настроенный в FMOD Studio</param>
		void StopPlaying(EventReference eventRef, bool immediate = false);

		/// <summary>
		/// Полная остановка текущего эмбиента.
		/// </summary>
		void StopAmbient(float fadeDuration = 1f);
	}
}