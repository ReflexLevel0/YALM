namespace YALM.Common.Models.Graphql.InputModels;

public class DiskInput : DiskIdInput
{
	public string? Type { get; set; }
	public string? Serial { get; set; }
	public string? Path { get; set; }
	public string? Vendor { get; set; }
	public string? Model { get; set; }
	public long? BytesTotal { get; set; }
	
	public DiskInput(int serverId, string uuid, string? type, string? serial, string? path, string? vendor, string? model, long? bytesTotal) : base(serverId, uuid)
	{
		ServerId = serverId;
		Uuid = uuid;
		Type = type;
		Serial = serial;
		Path = path;
		Vendor = vendor;
		Model = model;
		BytesTotal = bytesTotal;
	}
}