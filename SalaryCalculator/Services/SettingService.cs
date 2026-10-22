using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SalaryCalculator.Data;
using SalaryCalculator.Models;

namespace SalaryCalculator.Services;

public class SettingsService(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	AuthenticationStateProvider authStateProvider)
{
	private async Task<string> GetUserIdAsync()
	{
		var authState = await authStateProvider.GetAuthenticationStateAsync();
		var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

		return userId ?? throw new InvalidOperationException("Ingen bruker er logget inn.");
	}

	// Henter innstillingene til innlogget bruker. Finnes de ikke, lages de med standardverdier.
	public async Task<PaySettings> GetSettingsAsync()
	{
		var userId = await GetUserIdAsync();
		using var db = await dbFactory.CreateDbContextAsync();

		var settings = await db.PaySettings.FirstOrDefaultAsync(s => s.UserId == userId);

		if (settings is null)
		{
			settings = new PaySettings { UserId = userId };
			db.PaySettings.Add(settings);
			await db.SaveChangesAsync();
		}

		return settings;
	}

	public async Task SaveSettingsAsync(PaySettings settings)
	{
		var userId = await GetUserIdAsync();
		using var db = await dbFactory.CreateDbContextAsync();

		// Samme sikkerhetssjekk som i ShiftService
		bool isOwner = await db.PaySettings.AnyAsync(s => s.Id == settings.Id && s.UserId == userId);
		if (!isOwner)
		{
			throw new UnauthorizedAccessException("Du kan bare endre dine egne innstillinger.");
		}

		settings.UserId = userId;
		db.PaySettings.Update(settings);
		await db.SaveChangesAsync();
	}
}