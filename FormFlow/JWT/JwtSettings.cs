namespace FormFlow.JWT
{
	public class JwtSettings
	{
		public string SecretKey { get; set; }
		public TimeSpan TokenExpiration { get; set; }
	}
}
