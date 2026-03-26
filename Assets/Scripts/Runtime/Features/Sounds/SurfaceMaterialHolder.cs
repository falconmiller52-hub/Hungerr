using NaughtyAttributes;
using Runtime.Features.Sounds.Steps;
using UnityEngine;

namespace Runtime.Features.Sounds
{
    public class SurfaceMaterialHolder : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Surface Material Sound")] private SoundMaterial _materialSound;

        //Геттеры и сеттеры
        public SoundMaterial MaterialSound => _materialSound;
    }
}
