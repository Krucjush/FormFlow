using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FormFlow.Services
{
	public class JwtTokenService
	{
		public static string GenerateJwtToken(string username)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, username),
				// Dodaj inne informacje o użytkowniku, jeśli są potrzebne
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c448d91e57d221aac62742499c6fc9c8a24bee77b9ec7544979dd4d90cbf4658")); // Zastąp tajnym kluczem
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.UtcNow.AddHours(1); // Ustal czas ważności tokena

			var token = new JwtSecurityToken(
				"twoj_issuer", // Zastąp wartością wystawcy (issuer)
				"twoj_audience", // Zastąp wartością odbiorcy (audience)
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
