using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Features.Player.Movement
{
	public class PlayerEndurance : MonoBehaviour
	{
		[Header("Setting")] [SerializeField] private float _fadeDuration = 0.3f;

		[SerializeField] private PlayerStance _playerStance;
		[SerializeField] private Image _imgEndurance;

		private void OnEnable()
		{
			_playerStance.OnStaminaChanged += ChangeEnduranceImage;
		}

		private void OnDisable()
		{
			_playerStance.OnStaminaChanged -= ChangeEnduranceImage;
		}

		private void ShowEndurance()
		{
			if (!_imgEndurance.IsActive())
			{
				_imgEndurance.enabled = true;

				var color = _imgEndurance.color;
				color.a = 0;
				_imgEndurance.color = color;

				Tween.Alpha(_imgEndurance, 1, _fadeDuration);
			}
		}

		private void HideEndurance()
		{
			if (_imgEndurance.enabled)
			{
				Tween.Alpha(_imgEndurance, 0f, _fadeDuration)
								.OnComplete(() => _imgEndurance.enabled = false);
			}
		}

		private void ChangeEnduranceImage(float value)
		{
			_imgEndurance.fillAmount = value / _playerStance.MaximumStamina;

			if (_imgEndurance.fillAmount < 1)
				ShowEndurance();

			if (_imgEndurance.fillAmount >= 1f)
				HideEndurance();
		}
	}
}