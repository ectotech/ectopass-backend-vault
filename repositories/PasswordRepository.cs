/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-01
 */

using EctoTech.EctoPass.Backend.Vault.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EctoTech.EctoPass.Backend.Vault.Repositories;

public class PasswordRepository
{
	private readonly IMongoCollection<UserPasswords> _passwordCollection;

	public PasswordRepository(IOptions<DatabaseSettings> databaseSettings)
	{
		var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

		var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

		try
		{
			mongoDatabase.CreateCollection(databaseSettings.Value.CollectionName);
		}
		catch { }
		_passwordCollection = mongoDatabase.GetCollection<UserPasswords>(databaseSettings.Value.CollectionName);
	}

	public async Task<List<UserPasswords>> GetAsync(System.Linq.Expressions.Expression<Func<UserPasswords, bool>> filter)
	{
		return (await _passwordCollection.FindAsync(filter)).ToList();
	}

	public async Task CreateAsync(UserPasswords password)
	{
		await _passwordCollection.InsertOneAsync(password);
	}

	public async Task UpdateAsync(string id, UserPasswords password)
	{
		await _passwordCollection.ReplaceOneAsync(i => i.Id == id, password);
	}

	public async Task RemoveAsync(string id)
	{
		await _passwordCollection.DeleteOneAsync(i => i.Id == id);
	}
}
