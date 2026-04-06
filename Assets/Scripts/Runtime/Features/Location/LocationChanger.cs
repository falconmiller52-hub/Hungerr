using System;
using System.Collections;
using Runtime.Common.Services.LoadingCurtain;
using UnityEditor.Rendering;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Location
{
	/// <summary>
	/// Скрипт отвечающий за хендл смены позиции игрока, к нему обращаются все компоненты-триггеры
	/// </summary>
	public class LocationChanger : MonoBehaviour
	{
		[Header("Settings")] 
		[SerializeField] private float _defaultFadeInDuration  = 0.2f;
		[SerializeField] private float _defaultStayBlackDuration  = 0.5f;
		[SerializeField] private float _defaultFadeOutDuration = 0.5f;
		
		private ILoadingCurtain _curtain;
		private CharacterController _playerController;
		private LocationChangerData _defaultLocationChangerData;

		[SerializeField] private bool _isProcessing;

		[Inject]
		public void Construct(ILoadingCurtain curtain)
		{
			_curtain = curtain;
		}

		public void Init(CharacterController playerController)
		{
			_playerController = playerController;
			_defaultLocationChangerData = new LocationChangerData(_defaultFadeInDuration, _defaultStayBlackDuration, _defaultFadeOutDuration);
		}

		/// <summary>
		/// Метод который телепортирует персонажа в определенную точку.
		/// </summary>
		/// <param name="targetPoint">Позиция телепорта</param>
		/// <param name="locationChangerData">Данные о времени Fade</param>
		/// <param name="needCurtain">Нужно ли затемнение</param>
		/// <param name="needToDisableCC">Нужно ли выключать character controller</param>
		public void ChangeLocation(Transform targetPoint,LocationChangerData? locationChangerData = null,  bool needCurtain = true, bool needToDisableCC = true )
		{
			if (_isProcessing)
				return;

			if (_playerController == null)
			{
				Debug.LogError("LocationChanger::ChangeLocation() PlayerController is null");
				return;
			}
			
			StartCoroutine(LocationChangeRoutine(targetPoint, locationChangerData, needCurtain, needToDisableCC));
		}

		private IEnumerator LocationChangeRoutine(Transform target,LocationChangerData? locationChangerData = null, bool needCurtain = true, bool needToDisableCC = true)
		{
			_isProcessing = true;
			
			if (needToDisableCC)
				_playerController.enabled = false;

			LocationChangerData finalLocationChangerData = locationChangerData ?? _defaultLocationChangerData;
			
			if (needCurtain)
			{
				_curtain.Show(finalLocationChangerData.FadeOutDuration, false, () => { Teleport(target); });
				yield return new WaitForSeconds(finalLocationChangerData.StayBlackDuration);
				_curtain.Hide(finalLocationChangerData.FadeInDuration, false, () => { _isProcessing = false; });
			}
			else
			{
				Teleport(target);
				_isProcessing = false;
			}

			if (needToDisableCC)
				_playerController.enabled = true;
		}

		private void Teleport(Transform target)
		{
			_playerController.transform.SetPositionAndRotation(target.position, target.rotation);
		}
	}
}