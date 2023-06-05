namespace FormFlow.Data
{
	public class MongoDBSettings
	{
		public string ConnectionURI { get; set; } = null;
		public string DatabaseName { get; set; } = null;
		public Dictionary<string, string> Collections { get; set; } = new();
	}
}
