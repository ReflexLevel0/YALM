using System.Text.Json;

namespace Monitor;

internal class Monitor
{
	private static void Main()
	{
		var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");
		Console.WriteLine($"Interval: {config.IntervalInSeconds}\n");

		var logHelper = new LogHelper(config);
		while (true)
		{
			Thread.Sleep(2000);
			var log = logHelper.Log();
			Console.WriteLine(log);
		}
	}
}