using UnityEngine;


namespace Runtime.Features.DayNight
{
    [ExecuteInEditMode]
    public class DayCycleVisualChanger : MonoBehaviour
    {
        [SerializeField] private Light _directionalLight;
        
        [Header("Day Settings")]
        [SerializeField] private Material _daySkybox;
        [SerializeField] private Color _dayLightColor;
        [SerializeField] private Color _dayAmbientColor;
        [SerializeField] private float _dayLightXRotation = 80f;
        
        [Header("Night Gradient Settings")]
        [SerializeField] private float _nightStartLightXRotation = -90f;
        [SerializeField] private Material _nightSkybox;
        [SerializeField] private Gradient _direactionalLightGraident;
        [SerializeField] private Gradient _ambientLightGradient;


        private Vector3 _defaultAngles;

        private void Start()
        {
            _defaultAngles = _directionalLight.transform.localEulerAngles;
        }

        /// <summary>
        /// обновляет визуал ночи от начала ночной фазы до рассвета 
        /// </summary>
        /// <param name="timeProgress">от 0 до 1, где 0 это условно полночь а 1 это рассвет</param>
        public void UpdateNightCycle(float timeProgress)
        {
            _directionalLight.color = _ambientLightGradient.Evaluate(timeProgress);
            RenderSettings.ambientLight = _ambientLightGradient.Evaluate(timeProgress);
            
            float xAngle = Mathf.Lerp(_nightStartLightXRotation, 0f, timeProgress);

            _directionalLight.transform.localEulerAngles = new Vector3(xAngle, _defaultAngles.y, _defaultAngles.z);
        }

        public void SetNight()
        {
            RenderSettings.skybox = _nightSkybox;
            Vector3 nightStartRotation = _defaultAngles;
            nightStartRotation.x = _nightStartLightXRotation;
            _directionalLight.transform.localEulerAngles = nightStartRotation;
        }
        
        public void SetDay()
        {
            RenderSettings.skybox = _daySkybox;
            _directionalLight.color =  _dayLightColor;
            RenderSettings.ambientLight = _dayAmbientColor;

            Vector3 dayRotation = _defaultAngles;
            dayRotation.x = _dayLightXRotation;
            _directionalLight.transform.localEulerAngles = dayRotation;
        }
    }
}
