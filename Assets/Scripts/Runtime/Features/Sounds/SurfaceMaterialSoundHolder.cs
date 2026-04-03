using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Features.Sounds
{
    public class SurfaceMaterialSoundHolder : MonoBehaviour
    {
        //Переменные инспектора
        [SerializeField, Label("Surface Material Sound")] private List<SoundData> _materialSounds;

        //Геттеры и сеттеры
        public List<SoundData> MaterialSounds => _materialSounds;
    }
}
