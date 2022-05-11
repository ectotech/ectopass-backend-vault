/**
 * Copyright (c) ectotech AB - All Rights Reserved
 *
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Created by Lukas Wass dev@lukaswass.se, 2022-05-01
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EctoTech.EctoPass.Backend.Vault.Models;
using EctoTech.EctoPass.Backend.Vault.Repositories;
using EctoTech.EctoPass.Backend.Vault.Services.Authorization;
using MongoDB.Bson;
using EctoTech.EctoPass.Backend.Vault.Models.Api.Password;

namespace EctoTech.EctoPass.Backend.Vault.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PasswordController : ControllerBase
{
	private readonly ILogger<PasswordController> _logger;

	private readonly PasswordRepository _passwordRepository;

	public PasswordController(ILogger<PasswordController> logger, PasswordRepository passwordRepository)
	{
		_logger = logger;
		_passwordRepository = passwordRepository;
	}

	/// <summary>
	/// Get user passwords.
	/// </summary>
	/// <remarks>
	/// Tries to retrieve user passwords.
	/// </remarks>
	/// <response code="200">Retrived passwords</response>
	/// <response code="401">Unauthorized</response>
	/// <response code="404">Could not find any user passwords</response>
	/// <response code="500">Internal server error</response>
	[HttpGet]
	[Produces("application/json")]
	[ProducesResponseType(typeof(List<Password>), 200)]
	[ProducesResponseType(typeof(void), 401)]
	[ProducesResponseType(typeof(void), 404)]
	[ProducesResponseType(typeof(void), 500)]
	public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string authorization)
	{
		try
		{
			string userId = Authorization.GetUserId(authorization);

			UserPasswords? userPasswords = (await _passwordRepository.GetAsync(i => i.OwnerId == userId)).FirstOrDefault();
			if (userPasswords == null)
			{
				Response.StatusCode = 404;
				return new JsonResult("");
			}

			var passwords = userPasswords.Passwords;

			Response.StatusCode = 200;
			return new JsonResult(passwords);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception.Message);
			Response.StatusCode = 500;
			return new JsonResult("");
		}
	}

	/// <summary>
	/// Add a new user password.
	/// </summary>
	/// <remarks>
	/// Adds password to user password list. Cannot be user to update a password.
	/// </remarks>
	/// <response code="200">Added password to user</response>
	/// <response code="401">Unauthorized</response>
	/// <response code="500">Internal server error</response>
	[HttpPost]
	[Produces("application/json")]
	[ProducesResponseType(typeof(Password), 200)]
	[ProducesResponseType(typeof(void), 401)]
	[ProducesResponseType(typeof(void), 500)]
	public async Task<IActionResult> Post(
		[FromHeader(Name = "Authorization")] string authorization,
		PostPasswordInput passwordInput)
	{
		try
		{
			string userId = Authorization.GetUserId(authorization);

			bool createdNewUserPasswords = false;
			UserPasswords? userPasswords = (await _passwordRepository.GetAsync(i => i.OwnerId == userId)).FirstOrDefault();
			if (userPasswords == null)
			{
				createdNewUserPasswords = true;
				userPasswords = new UserPasswords();
				userPasswords.OwnerId = userId;
			}

			Password password = new();
			password.Data = passwordInput.Data;
			userPasswords.Passwords.Add(password);

			if (createdNewUserPasswords)
			{
				await _passwordRepository.CreateAsync(userPasswords);
			}
			else
			{
				await _passwordRepository.UpdateAsync(userPasswords.Id, userPasswords);
			}

			Response.StatusCode = 200;
			return new JsonResult(password);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception.Message);
			Response.StatusCode = 500;
			return new JsonResult("");
		}
	}

	/// <summary>
	/// Update a user password.
	/// </summary>
	/// <remarks>
	/// Tries to update password item in user password list. Cannot be used to add a new password.
	/// </remarks>
	/// <response code="200">Updated user password</response>
	/// <response code="401">Unauthorized</response>
	/// <response code="404">Could not find password to update</response>
	/// <response code="500">Internal server error</response>
	[HttpPut]
	public async Task<IActionResult> Put(
		[FromHeader(Name = "Authorization")] string authorization,
		PutPasswordInput passwordInput)
	{
		try
		{
			string userId = Authorization.GetUserId(authorization);

			UserPasswords? userPasswords = (await _passwordRepository.GetAsync(i => i.OwnerId == userId)).FirstOrDefault();
			if (userPasswords == null)
			{
				Response.StatusCode = 404;
				return new JsonResult("");
			}

			Password passwordRes = new();
			foreach (var password in userPasswords.Passwords)
			{
				if (password.Id == passwordInput.Id)
				{
					password.History.Insert(0, password.Data);
					password.Data = passwordInput.Data;
					password.UpdateDate = DateTime.Now;

					passwordRes = password;
					break;
				}
			}

			await _passwordRepository.UpdateAsync(userPasswords.Id, userPasswords);

			Response.StatusCode = 200;
			return new JsonResult(passwordRes);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception.Message);
			Response.StatusCode = 500;
			return new JsonResult("");
		}
	}

	/// <summary>
	/// Delete a user password.
	/// </summary>
	/// <remarks>
	/// Tries to delete a user password item from password list.
	/// </remarks>
	/// <response code="200">Deleted user password</response>
	/// <response code="401">Unauthorized</response>
	/// <response code="404">Could not find password to delete</response>
	/// <response code="500">Internal server error</response>
	[HttpDelete]
	public async Task<IActionResult> Delete(
		[FromHeader(Name = "Authorization")] string authorization,
		DeletePasswordInput passwordInput)
	{
		try
		{
			string userId = Authorization.GetUserId(authorization);

			UserPasswords? userPasswords = (await _passwordRepository.GetAsync(i => i.OwnerId == userId)).FirstOrDefault();
			if (userPasswords == null)
			{
				Response.StatusCode = 404;
				return new JsonResult("");
			}

			Password? passwordRes = null;
			foreach (var password in userPasswords.Passwords)
			{
				if (password.Id == passwordInput.Id)
				{
					passwordRes = password;
					userPasswords.Passwords.Remove(password);
					break;
				}
			}

			if (passwordRes != null)
			{
				await _passwordRepository.UpdateAsync(userPasswords.Id, userPasswords);

				Response.StatusCode = 200;
				return new JsonResult(passwordRes);
			}
			else
			{
				Response.StatusCode = 404;
				return new JsonResult("");
			}
		}
		catch (Exception exception)
		{
			_logger.LogError(exception.Message);
			Response.StatusCode = 500;
			return new JsonResult("");
		}
	}
}
