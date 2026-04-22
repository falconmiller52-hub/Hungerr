using NaughtyAttributes;
using Runtime.Features.Player.Movement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Runtime.Features.Player.Other
{
	[RequireComponent(typeof(PlayerStance))]
	public class PlayerProcessingController : MonoBehaviour
	{
		[SerializeField, Label("Post Processing Object")]
		private Volume _postProcessingObject;

		[Space, SerializeField, MinMaxSlider(0f, 1f), Label("Crouch Vignette Strength")]
		private Vector2 _crouchVignetteStrength;

		[SerializeField, MinMaxSlider(0f, 500f), Label("Exhaustion Depth of Field Strength")]
		private Vector2 _exhaustionDofStrength;

		private float _nextVignetteStrength;
		private float _nextDofStrength;

		private PlayerStance _playerStance;
		private Vignette _vignette;
		private DepthOfField _depthOfField;

		//Методы Моно
		private void Start()
		{
			_playerStance = GetComponent<PlayerStance>();
			_postProcessingObject.profile.TryGet(out _vignette);
			_postProcessingObject.profile.TryGet(out _depthOfField);

			_nextVignetteStrength = _crouchVignetteStrength.y;
			_nextDofStrength = _exhaustionDofStrength.y;
		}

		private void Update()
		{
			_nextVignetteStrength = _playerStance.CurrentStance == PlayerStance.Stance.Crouching
							? _crouchVignetteStrength.y
							: _crouchVignetteStrength.x;
			_nextDofStrength = _playerStance.IsExhausted ? _exhaustionDofStrength.x : _exhaustionDofStrength.y;

			_vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _nextVignetteStrength,
							Time.deltaTime * _playerStance.CrouchSpeed);
			_depthOfField.gaussianEnd.value = Mathf.Lerp(_depthOfField.gaussianEnd.value, _nextDofStrength,
							Time.deltaTime * _playerStance.ExhaustionDuration / 2);
		}
	}
}