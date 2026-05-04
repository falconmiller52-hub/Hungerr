using System;
using FMODUnity;
using UnityEngine;

namespace Runtime.Common.Services.Audio.Ost
{
	[Serializable]
	[CreateAssetMenu(fileName = "OstData", menuName = "Sound/Ost events")]
	public class OstData : ScriptableObject
	{
		[Header("Main menu")]
		[field: SerializeField] public EventReference OstMainMenu { get; private set; }
		
		[Header("Day theme")]
		[field: SerializeField] public EventReference OstDayHomeOne { get; private set; }
		[field: SerializeField] public EventReference OstDayHomeTwo { get; private set; }
		
		[Header("Ost Homie")]
		[field: SerializeField] public EventReference OstHomie { get; private set; }
		
		[Header("Ost player death")]
		[field: SerializeField] public EventReference OsTheEnd{ get; private set; }
	}
}