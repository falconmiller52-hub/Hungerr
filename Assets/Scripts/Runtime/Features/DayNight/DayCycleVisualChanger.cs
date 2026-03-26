using System;
using UnityEngine;


// if (Application.isPlaying)
//     _timeProgress += Time.deltaTime / _dayTimeInSeconds;
//             
// if (_timeProgress >= 1f)
//     _timeProgress = 0f;

namespace Runtime.Features.DayNight
{
    [ExecuteInEditMode]
    public class DayCycleVisualChanger : MonoBehaviour
    {
        [SerializeField] private Gradient _direactionalLightGraident;
        [SerializeField] private Gradient _ambientLightGradient;

        [SerializeField] private Light _directionalLight;

        private Vector3 _defaultAngles;

        private void Start()
        {
            _defaultAngles = _directionalLight.transform.localEulerAngles;
        }

        public void UpdateDayCycle(float timeProgress)
        {
            _directionalLight.color = _ambientLightGradient.Evaluate(timeProgress);
            RenderSettings.ambientLight = _ambientLightGradient.Evaluate(timeProgress);
            
            _directionalLight.transform.localEulerAngles = new Vector3(360f * timeProgress - 90, _defaultAngles.y, _defaultAngles.z);
        }
    }
}
