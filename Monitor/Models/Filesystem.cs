namespace Monitor.Models;

public class Filesystem
{
	public string? Name { get; }
	public string? Version { get; }
	
	public Filesystem(string? name, string? version)
	{
		Name = name;
		Version = version;
	}

	public override string ToString() => $"{Name} {Version}";
}