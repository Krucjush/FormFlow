namespace FormFlow.Data
{
	public class MongoDBSettings
	{
		public string ConnectionURI { get; set; } = null;
		public string DatabaseName { get; set; } = null;
		public string User { get; set; } = null;
		public string Form { get; set; } = null;
		public string FormResponse { get; set; } = null;
		public string Question { get; set; } = null;
		public string ResponseEntry { get; set; } = null;
	}
}
