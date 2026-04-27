using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Features.Enemy.Domovoi
{
	[Serializable]
	public class DomovoiLevelData
	{
		[Tooltip("Максимальная сытость")] 
		public int MaxSatiety = 60;
        
		[Tooltip("C какого дня активируется уровень")] 
		public int MinDayForLevel = 1;
        
		[Tooltip("Насколько уменьшается голод с каждой ночью")] 
		public int DailySatietyLoss = 30;
        
		[Tooltip("Уровень сытости при котором (и ниже) начинается активация")] 
		public int SatietyTreshholdForActivation = 20;
        
		[Tooltip("Сколько дней может быть 0 сытости до того как игрок получит наказание")] 
		public int NotFeededDaysAvailableBeforePunishment = 2;
		
		[Tooltip("Паттерны поведения при текущем уровне (если активация при низкой сытости есть)")] 
		public List<DomovoiPattern> Patterns;
	}
}