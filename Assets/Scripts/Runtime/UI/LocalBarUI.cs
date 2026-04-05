using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class LocalBarUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void UpdateUI(float currentValue, float maxValue)
        {
            _slider.value = currentValue / maxValue;
        }
    }
}
