using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class TimeCounterUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private GameObject _timerObject;
        
        /// <summary>
        /// метод показывающий текущее состояние времени суток
        /// </summary>
        /// <param name="timeProgress">принимает прогресс в float от 0 до 1</param>
        /// <param name="startHour">с какого часа начинает отсчет (при timeProgress = 0)</param>
        /// <param name="endHour">на каком часу будет конец таймера (при timeProgress = 1)</param>
        public void SetTime(float timeProgress, int startHour = 0, int endHour = 6)
        {
            // 1. Ограничиваем прогресс, чтобы он не выходил за пределы 0-1
            float clampledProgress = Mathf.Clamp01(timeProgress / 1);

            // 2. Считаем общую разницу в часах между стартом и эндом
            // Учитываем переход через полночь (например, с 22:00 до 04:00)
            float hoursDiff = endHour >= startHour 
                ? endHour - startHour 
                : (24 - startHour) + endHour;

            // 3. Вычисляем текущее количество пройденных игровых часов
            float elapsedHours = clampledProgress * hoursDiff;

            // 4. Добавляем к стартовому часу и зацикливаем на 24
            float currentHourRaw = (startHour + elapsedHours) % 24f;

            // 5. Разделяем на часы и минуты
            int hours = Mathf.FloorToInt(currentHourRaw);
            int minutes = Mathf.FloorToInt((currentHourRaw - hours) * 60f);

            // 6. Вывод в формате VHS (с эффектом двоеточия)
            string separator = (Mathf.FloorToInt(Time.time * 2f) % 2 == 0) ? ":" : " ";
            _timeText.text = $"{hours:00}{separator}{minutes:00}";
        }

        public void SetTimeState(bool active)
        {
            _timerObject.SetActive(active);
        }
    }
}
