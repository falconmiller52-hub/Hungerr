namespace Runtime.Common.Helpers
{
	/// <summary>
	/// ивент дата для данных о триггере для начала ночи (aka том при каких обстоятельствах произошел триггер ночи)
	/// </summary>
	public class StartDayTriggerEventData
	{
		public bool ForceNightEnd;
	}
	
	/// <summary>
	/// ивент дата для передачи данных о начале ночи
	/// </summary>
	public class StartNightEventData
	{
		public int CurrentDay;
	}
}