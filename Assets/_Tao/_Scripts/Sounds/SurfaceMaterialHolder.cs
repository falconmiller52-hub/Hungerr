using UnityEngine;
using NaughtyAttributes;

public class SurfaceMaterialHolder : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Surface Material Sound")] private SoundMaterial _materialSound;

    //Геттеры и сеттеры
    public SoundMaterial MaterialSound => _materialSound;
}
