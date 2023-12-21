namespace YALM.Monitor.Models.StorageJSON;

public class BlockDevice : BlockDeviceChild
{
	public List<BlockDeviceChild> Children { get; set; }
}