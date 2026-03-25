using System;
using UnityEngine;

namespace Runtime.Features.DayNight
{
    [ExecuteInEditMode]
    public class DayCycleVisualChanger : MonoBehaviour
    {
        [SerializeField] private Gradient _direactionalLightGraident;
        [SerializeField] private Gradient _ambientLightGradient;

        [SerializeField, Range(1, 3600)] private float _dayTimeInSeconds = 60f;
        [SerializeField, Range(0, 1f)] private float _timeProgress = 0f;

        [SerializeField] private Light _directionalLight;

        private Vector3 _defaultAngles;

        private void Start()
        {
            _defaultAngles = _directionalLight.transform.localEulerAngles;
        }

        private void Update()
        {
            if (Application.isPlaying)
                _timeProgress += Time.deltaTime / _dayTimeInSeconds;
            
            if (_timeProgress >= 1f)
                _timeProgress = 0f;
            
            _directionalLight.color = _ambientLightGradient.Evaluate(_timeProgress);
            RenderSettings.ambientLight = _ambientLightGradient.Evaluate(_timeProgress);
            
            _directionalLight.transform.localEulerAngles = new Vector3(360f * _timeProgress - 90, _defaultAngles.y, _defaultAngles.z);
        }
    }
}
