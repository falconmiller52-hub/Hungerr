using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Features.DayNight
{
	/// <summary>
	/// фасадный скрипт который дает апи для манипуляции с временем суток
	/// </summary>
	[ExecuteInEditMode]
	public class DayCycleVisualChanger : MonoBehaviour
	{
		[SerializeField] private Light _directionalLight;
		[SerializeField] private Light _moon;
		
		[Header("Day Settings")] [SerializeField]
		private Material _daySkybox;

		[SerializeField] private Color _dayLightColor;
		[SerializeField] private Color _dayAmbientColor;
		[SerializeField] private float _dayLightXRotation = 80f;

		[Header("Night Gradient Settings")] 
		[SerializeField] private float _nightStartLightXRotation = -90f;
		[SerializeField] private float _nightStartLightXRotationMoon = 90f;
		[SerializeField] private float _startMoonIntensity = 4.0f;

		[SerializeField] private Material _nightSkybox;
		[SerializeField] private Gradient _direactionalLightGraident;
		[SerializeField] private Gradient _ambientLightGradient;
		
		private Vector3 _defaultAngles;
		private Vector3 _defaultMoonAngles;

		private void Start()
		{
			_defaultAngles = _directionalLight.transform.localEulerAngles;
			_defaultMoonAngles = _moon.transform.localEulerAngles;
			RenderSettings.sun = _directionalLight;
			RenderSettings.ambientMode = AmbientMode.Trilight;
		}

		/// <summary>
		/// обновляет визуал ночи от начала ночной фазы до рассвета 
		/// </summary>
		/// <param name="timeProgress">от 0 до 1, где 0 это условно полночь а 1 это рассвет</param>
		public void UpdateNightCycle(float timeProgress)
		{
			_directionalLight.color = _ambientLightGradient.Evaluate(timeProgress);
			_moon.color = _ambientLightGradient.Evaluate(timeProgress);
			RenderSettings.ambientLight = _ambientLightGradient.Evaluate(timeProgress);

			float xAngle = Mathf.Lerp(_nightStartLightXRotation, 0f, timeProgress);
			float xMoonAngle = Mathf.Lerp(_nightStartLightXRotationMoon, 180, timeProgress);
			float intensity = Mathf.Lerp(_startMoonIntensity, 0f, timeProgress);
			
			_moon.intensity = intensity;
			_moon.transform.localEulerAngles = new Vector3(xMoonAngle, _defaultMoonAngles.y, _defaultMoonAngles.z);
			_directionalLight.transform.localEulerAngles = new Vector3(xAngle, _defaultAngles.y, _defaultAngles.z);
		}

		public void SetNight()
		{
			RenderSettings.skybox = _nightSkybox;
			_moon.intensity = _startMoonIntensity;
			
			_moon.gameObject.SetActive(true);
			
			Vector3 nightStartRotation = _defaultAngles;
			nightStartRotation.x = _nightStartLightXRotation;
			_directionalLight.transform.localEulerAngles = nightStartRotation;

			nightStartRotation = _defaultMoonAngles;
			nightStartRotation.x = _nightStartLightXRotationMoon;
			_moon.transform.localEulerAngles = nightStartRotation;
		}

		public void SetDay()
		{
			_moon.gameObject.SetActive(false);
			
			RenderSettings.skybox = _daySkybox;
			_directionalLight.color = _dayLightColor;
			RenderSettings.ambientLight = _dayAmbientColor;

			Vector3 dayRotation = _defaultAngles;
			dayRotation.x = _dayLightXRotation;
			_directionalLight.transform.localEulerAngles = dayRotation;
		}
	}
}