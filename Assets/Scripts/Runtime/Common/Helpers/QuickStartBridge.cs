namespace Runtime.Common.Helpers
{
	// класс хелпер связующее звено для полноценной работы механики корректного запуска плей мода с нужной сцены
	public static class QuickStartBridge
	{
		public static bool IsQuickStart { get; set; }
		public static string SceneName { get; set; }
	}
}