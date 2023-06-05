namespace FormFlow.Models
{
	public class AuthenticationModels
	{
		public class AuthenticateRequest
		{
			public string Email { get; set; }
			public string Password { get; set; }
		}

		public class AuthenticateResponse
		{
			public string Id { get; set; }
			public string Email { get; set; }
			public string Token { get; set; }

			public AuthenticateResponse(User user, string token)
			{
				Id = user.Id;
				Email = user.Email;
				Token = token;
			}
		}
	}
}
