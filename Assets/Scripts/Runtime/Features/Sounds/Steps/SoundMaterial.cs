using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Material Sound", order = 51)]
public class SoundMaterial : ScriptableObject
{
    public List<AudioClip> StepSounds;
}
