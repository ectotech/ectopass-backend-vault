/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-10
 */

using System.IdentityModel.Tokens.Jwt;

namespace EctoTech.EctoPass.Backend.Vault.Services.Authorization;

public class Authorization
{
	public static string GetUserId(string authorization)
	{
		string idToken = authorization.Replace("Bearer ", "");  // Remove "Bearer" from token
		var jwtHandler = new JwtSecurityTokenHandler();
		var claims = jwtHandler.ReadJwtToken(idToken).Claims;
		foreach (var claim in claims)
		{
			if (claim.Type == "user_id")
			{
				return claim.Value;
			}
		}

		throw new Exception("Invalid token");
	}
}
