namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionInput
{
	public string DiskLabel { get; set; }
	public string Uuid { get; set; }
	public string? FilesystemName { get; set; }
	public string? FilesystemVersion { get; set; }
	public string? PartitionLabel { get; set; }
	public string? Mountpath { get; set; }

	public PartitionInput(string diskLabel, string uuid)
	{
		DiskLabel = diskLabel;
		Uuid = uuid;
	}
}