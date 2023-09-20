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
					new(ClaimTypes.Name, username),
				};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c448d91e57d221aac62742499c6fc9c8a24bee77b9ec7544979dd4d90cbf4658"));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.UtcNow.AddHours(1);

			var token = new JwtSecurityToken(
				"your_issuer",
				"your_audience",
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static string ReadUserFromToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = "your_issuer",
				ValidAudience = "your_audience",
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c448d91e57d221aac62742499c6fc9c8a24bee77b9ec7544979dd4d90cbf4658"))
			};

			try
			{
				var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
				return claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value!;
			}
			catch (Exception)
			{
				return null;
			}

		}

	}
}
