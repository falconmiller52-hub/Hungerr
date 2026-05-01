using Runtime.Common.InspectorFeatures.ButtonEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Features.DayNight
{
	/// <summary>
	/// фасадный скрипт который дает апи для манипуляции с временем суток
	/// </summary>
	[ExecuteInEditMode]
	public class DayCycleVisualChanger : MonoBehaviour, IButtonPressedHandler
	{
		[Header(" Light Sources")]
		[SerializeField] private Light _directionalLight;
		[SerializeField] private Light _moon;
		
		[Header("Fog Colors")]
		[SerializeField] private Color _dayFogColor;
		[SerializeField] private Gradient _nightFogColorGradient;
		
		[Header("Day Settings")] 
		[SerializeField] private Material _daySkybox;
		[SerializeField] private Color _dayLightColor;
		[SerializeField] private Color _dayAmbientColor;
		[SerializeField, Tooltip(" поворот солнца днем")] private float _sunDayXRotation = 80f;

		[Header("Night Gradient Settings")] 
		[SerializeField, Tooltip("стартовый поворот луны ночью")] private float _moonStartXRotation = 90f;
		[SerializeField] private float _moonEndXRotation = 180f;
		
		[SerializeField, Tooltip("интенсивность луны в начале ночи")] private float _startMoonIntensity = 4.0f;
		[SerializeField] private AnimationCurve _moonIntensityCurve;
		
		[SerializeField] private Material _nightSkybox;
		[SerializeField] private Gradient _directionalLightGraident;
		[SerializeField] private Gradient _ambientLightGradient;
		
		[Header("Debug")]
		[SerializeField, Range(0f, 1f)] private float _nightTimeProgressDebug = 0;
		
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
			_moon.color = _directionalLightGraident.Evaluate(timeProgress);
			
			RenderSettings.ambientLight = _ambientLightGradient.Evaluate(timeProgress);
			RenderSettings.fogColor = _nightFogColorGradient.Evaluate(timeProgress);
			
			float xMoonAngle = Mathf.Lerp(_moonStartXRotation, _moonEndXRotation, timeProgress);
			float intensity = _startMoonIntensity * _moonIntensityCurve.Evaluate(timeProgress);
			
			_moon.intensity = intensity;
			_moon.transform.localEulerAngles = new Vector3(xMoonAngle, _defaultMoonAngles.y, _defaultMoonAngles.z);
		}

		public void SetNight()
		{
			RenderSettings.skybox = _nightSkybox;
			_moon.intensity = _startMoonIntensity;
			
			_moon.gameObject.SetActive(true);
			
			// Vector3 nightStartRotation = _defaultAngles;
			// nightStartRotation.x = _sunStartXRotation;
			// _directionalLight.transform.localEulerAngles = nightStartRotation;
			
			_directionalLight.gameObject.SetActive(false);

			Vector3 nightStartRotation = _defaultMoonAngles;
			nightStartRotation.x = _moonStartXRotation;
			_moon.transform.localEulerAngles = nightStartRotation;
		}
		
		public void SetDay()
		{
			_moon.gameObject.SetActive(false);
			_directionalLight.gameObject.SetActive(true);
			
			RenderSettings.fogColor = _dayFogColor;
			RenderSettings.skybox = _daySkybox;
			_directionalLight.color = _dayLightColor;
			RenderSettings.ambientLight = _dayAmbientColor;

			Vector3 dayRotation = _defaultAngles;
			dayRotation.x = _sunDayXRotation;
			_directionalLight.transform.localEulerAngles = dayRotation;
		}
		
		// Debug
		
		public void OnButtonPressed(string name)
		{
#if UNITY_EDITOR
			if (name.Equals("Set Night"))
			{
				SetNight();
				UpdateNightCycle(_nightTimeProgressDebug);
				Debug.Log("Intensity of Moon: " + (_startMoonIntensity * _moonIntensityCurve.Evaluate(_nightTimeProgressDebug)));
			}
			else if (name.Equals("Set Day"))
			{
				SetDay();
			}
#endif
		}
	}
}