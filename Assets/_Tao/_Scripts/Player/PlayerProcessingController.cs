using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PlayerStance))]
public class PlayerProcessingController : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Post Processing Object")] private Volume _postProcessingObject;

    [Space, SerializeField, MinMaxSlider(0f, 1f), Label("Crouch Vignette Strength")] private Vector2 _crouchVignetteStrength;
    [SerializeField, Label("Crouch Depth of Field Strength")] private Vector2 _crouchDofStrength;

    //Внутренние переменные
    private float _nextVignetteStrength;
    private float _nextDofStrength;

    //Кэшированные переменные
    PlayerStance _playerStance;
    private Vignette _vignette;
    private DepthOfField _depthOfField;

    //Методы Моно
    private void Start()
    {
        _playerStance = GetComponent<PlayerStance>();
        _postProcessingObject.profile.TryGet(out _vignette);
        _postProcessingObject.profile.TryGet(out _depthOfField);

        _nextVignetteStrength = _crouchVignetteStrength.y;
        _nextDofStrength = _crouchDofStrength.y;
    }

    private void Update()
    {
        _nextVignetteStrength = _playerStance.CurrentStance == PlayerStance.Stance.Crouching ? _crouchVignetteStrength.y : _crouchVignetteStrength.x;
        _nextDofStrength = _playerStance.CurrentStance == PlayerStance.Stance.Crouching ? _crouchDofStrength.x : _crouchDofStrength.y;

        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _nextVignetteStrength, Time.deltaTime * _playerStance.CrouchSpeed);
        _depthOfField.gaussianEnd.value = Mathf.Lerp(_depthOfField.gaussianEnd.value, _nextDofStrength, Time.deltaTime * _playerStance.CrouchSpeed);
    }

    //Методы скрипта


    //Геттеры и сеттеры
    public Vector2 CrouchVignetteStrength
    {
        get => _crouchVignetteStrength;
        set => _crouchVignetteStrength = value;
    }

    public Vector2 CrouchDepthOfFieldStrength
    {
        get => _crouchDofStrength;
        set => _crouchDofStrength = value;
    }

    public float NextVignetteStrength
    {
        get => _nextVignetteStrength;
        set => _nextVignetteStrength = value;
    }

    public float NextDepthOfFieldStrength
    {
        get => _nextDofStrength;
        set => _nextDofStrength = value;
    }
}