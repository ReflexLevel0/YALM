namespace Monitor;

public class Config
{
	public int IntervalInSeconds { get; set; }
	public bool Cpu { get; set; }
	public bool Memory { get; set; }
	public bool Network { get; set; }
	public bool Storage { get; set; }
	public List<string>? Services { get; set; }
}