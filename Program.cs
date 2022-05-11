/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-01
 */

using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using EctoTech.EctoPass.Backend.Vault.Models;
using EctoTech.EctoPass.Backend.Vault.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(@"./FirebaseServiceAccount.json") });

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = configuration["Authentication:Firebase:ValidIssuer"];
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["Authentication:Firebase:ValidIssuer"],
			ValidAudience = configuration["Authentication:Firebase:ValidAudience"]
		};
	});

services.Configure<DatabaseSettings>(configuration.GetSection("PasswordDatabase"));
services.AddSingleton<PasswordRepository>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "ectopass backend vault microservices", Version = "v1" });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "JWT Authorization header using Bearer scheme. Example \"Authorization: Bearer {token}\"",
		Scheme = "Bearer",
		BearerFormat = "JWT",
		Type = SecuritySchemeType.ApiKey,
		In = ParameterLocation.Header
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});

	string filePath = Path.Combine(System.AppContext.BaseDirectory, "EctoTech.EctoPass.Backend.Vault.xml");
	c.IncludeXmlComments(filePath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
