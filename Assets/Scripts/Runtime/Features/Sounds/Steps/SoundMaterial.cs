using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.Sounds.Steps
{
    [CreateAssetMenu(menuName = "Material Sound", order = 51)]
    public class SoundMaterial : ScriptableObject
    {
        public List<AudioClip> StepSounds;
    }
}
