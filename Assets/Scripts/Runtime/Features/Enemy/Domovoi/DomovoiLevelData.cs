using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.Enemy.Domovoi
{
	[Serializable]
	public class DomovoiLevelData
	{
		[Tooltip("Максимальная сытость")] 
		public int MaxSatiety;
        
		[Tooltip("C какого дня активируется уровень")] 
		public int MinDayForLevel;
        
		[Tooltip("Насколько уменьшается голод с каждой ночью")] 
		public int DailySatietyLoss;
        
		[Tooltip("Уровень сытости при котором (и ниже) начинается активация")] 
		public int SatietyTreshholdForActivation;
        
		[Tooltip("Паттерны поведения при текущем уровне (если активация при низкой сытости есть)")] 
		public List<DomovoiPattern> Patterns;
	}
}