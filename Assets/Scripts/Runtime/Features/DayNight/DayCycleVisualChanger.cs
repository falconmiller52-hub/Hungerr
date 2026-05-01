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
		[Header(" Light Sources")]
		[SerializeField] private Light _directionalLight;
		[SerializeField] private Light _moon;
		
		[Header("Fog Colors")]
		[SerializeField] private Color _dayFogColor;
		[SerializeField] private Color _nightFogColor;
		
		[Header("Day Settings")] 
		[SerializeField] private Material _daySkybox;
		[SerializeField] private Color _dayLightColor;
		[SerializeField] private Color _dayAmbientColor;
		[SerializeField, Tooltip(" поворот солнца днем")] private float _sunDayXRotation = 80f;

		[Header("Night Gradient Settings")] 
		[SerializeField, Tooltip("стартовый поворот солнца ночью")] private float _sunStartXRotation = -90f;
		[SerializeField] private float _sunEndXRotation = 0f;
		[SerializeField, Tooltip("стартовый поворот луны ночью")] private float _moonStartXRotation = 90f;
		[SerializeField] private float _moonEndXRotation = 180f;
		
		[SerializeField, Tooltip("интенсивность луны в начале ночи")] private float _startMoonIntensity = 4.0f;

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

			float xAngle = Mathf.Lerp(_sunStartXRotation, _sunEndXRotation, timeProgress);
			float xMoonAngle = Mathf.Lerp(_moonStartXRotation, _moonEndXRotation, timeProgress);
			float intensity = Mathf.Lerp(_startMoonIntensity, 0f, timeProgress);
			
			_moon.intensity = intensity;
			_moon.transform.localEulerAngles = new Vector3(xMoonAngle, _defaultMoonAngles.y, _defaultMoonAngles.z);
			_directionalLight.transform.localEulerAngles = new Vector3(xAngle, _defaultAngles.y, _defaultAngles.z);
		}

		public void SetNight()
		{
			RenderSettings.fogColor = _nightFogColor;
			RenderSettings.skybox = _nightSkybox;
			_moon.intensity = _startMoonIntensity;
			
			_moon.gameObject.SetActive(true);
			
			Vector3 nightStartRotation = _defaultAngles;
			nightStartRotation.x = _sunStartXRotation;
			_directionalLight.transform.localEulerAngles = nightStartRotation;

			nightStartRotation = _defaultMoonAngles;
			nightStartRotation.x = _moonStartXRotation;
			_moon.transform.localEulerAngles = nightStartRotation;
		}
		
		public void SetDay()
		{
			_moon.gameObject.SetActive(false);
			
			RenderSettings.fogColor = _dayFogColor;
			RenderSettings.skybox = _daySkybox;
			_directionalLight.color = _dayLightColor;
			RenderSettings.ambientLight = _dayAmbientColor;

			Vector3 dayRotation = _defaultAngles;
			dayRotation.x = _sunDayXRotation;
			_directionalLight.transform.localEulerAngles = dayRotation;
		}
		
		// Debug
		
		[ContextMenu("Set Day")]
		public void SetDayDebug()
		{
			SetDay();
		}

		[ContextMenu("Set Night")]
		public void SetNightDebug()
		{
			SetNight();
			
			_directionalLight.color = _ambientLightGradient.Evaluate(0);
			_moon.color = _ambientLightGradient.Evaluate(0);
			RenderSettings.ambientLight = _ambientLightGradient.Evaluate(0);
			
		}
	}
}