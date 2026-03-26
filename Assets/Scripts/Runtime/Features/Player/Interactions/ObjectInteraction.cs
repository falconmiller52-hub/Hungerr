using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Features.Player.Interactions
{
    [RequireComponent(typeof(Outline))]
    public class ObjectInteraction : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Interaction Events")] private UnityEvent _interactionEvents;

        [SerializeField, Label("Outline Mode")] private Outline.Mode _outlineMode = Outline.Mode.OutlineAll;
        [SerializeField, Label("Outline Color")] private Color _outlineColor = Color.white;
        [SerializeField, Label("Outline Width")] private float _outlineWidth = 25f;

        //Внутренние переменные
        private float _currentOutlineWidth = 0f;

        //Кэшированные переменные
        private Outline _outline;

        //Методы Моно
        private void Start()
        {
            _outline = GetComponent<Outline>();
        }

        //Методы скрипта
        private void Update()
        {
            _outline.OutlineMode = _outlineMode;
            _outline.OutlineColor = _outlineColor;
            _outline.OutlineWidth = _currentOutlineWidth; 
        }

        //Геттеры и сеттеры
        public UnityEvent InteractionEvents
        {
            get => _interactionEvents;
            set => _interactionEvents = value;
        }

        public Outline.Mode OutlineMode
        {
            get => _outlineMode;
            set => _outlineMode = value;
        }

        public Color OutlineColor
        {
            get => _outlineColor;
            set => _outlineColor = value;
        }

        public float OutlineWidth
        {
            get => _outlineWidth;
            set => _outlineWidth = value;
        }

        public float CurrentOutlineWidth
        {
            get => _currentOutlineWidth;
            set => _currentOutlineWidth = value;
        }
    }
}