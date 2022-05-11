/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-09
 */

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EctoTech.EctoPass.Backend.Vault.Models;

public class UserPasswords
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	[JsonPropertyName("id")]
	public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

	[BsonElement("ownerId")]
	[JsonPropertyName("ownerId")]
	public string OwnerId { get; set; } = "";

	[BsonElement("createdDate")]
	[JsonPropertyName("createdDate")]
	public DateTime? CreatedDate { get; set; } = DateTime.Now;

	[BsonElement("updateDate")]
	[JsonPropertyName("updateDate")]
	public DateTime? UpdateDate { get; set; } = DateTime.Now;

	[BsonElement("passwords")]
	[JsonPropertyName("passwords")]
	public List<Password> Passwords { get; set; } = new();
}
