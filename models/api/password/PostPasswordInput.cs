/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-11
 */

using System.Text.Json.Serialization;

namespace EctoTech.EctoPass.Backend.Vault.Models.Api.Password;

public class PostPasswordInput
{
	[JsonPropertyName("data")]
	public string Data { get; set; } = "";
}
