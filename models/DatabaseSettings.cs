/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-01
 */

namespace EctoTech.EctoPass.Backend.Vault.Models;

public class DatabaseSettings
{
	public string ConnectionString { get; set; } = null!;

	public string DatabaseName { get; set; } = null!;

	public string CollectionName { get; set; } = null!;
}
