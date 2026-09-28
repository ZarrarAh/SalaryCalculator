using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SalaryCalculator.Data;
using SalaryCalculator.Models;

namespace SalaryCalculator.Services;

public class ShiftService(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	AuthenticationStateProvider authStateProvider)
{
	// Finner id-en til brukeren som er logget inn
	private async Task<string> GetUserIdAsync()
	{
		var authState = await authStateProvider.GetAuthenticationStateAsync();
		var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

		return userId ?? throw new InvalidOperationException("Ingen bruker er logget inn.");
	}

	// Henter alle vakter for innlogget bruker mellom to datoer
	public async Task<List<Shift>> GetShiftsAsync(DateOnly start, DateOnly end)
	{
		var userId = await GetUserIdAsync();
		using var db = await dbFactory.CreateDbContextAsync();

		return await db.Shifts
			.Where(s => s.UserId == userId && s.Date >= start && s.Date <= end)
			.OrderBy(s => s.Date)
			.ToListAsync();
	}

	// Lagrer en ny vakt, eller oppdaterer en som finnes
	public async Task SaveShiftAsync(Shift shift)
	{
		var userId = await GetUserIdAsync();
		using var db = await dbFactory.CreateDbContextAsync();

		if (shift.Id == 0)
		{
			// Ny vakt: Id er 0 fordi databasen ikke har gitt den en id ennå
			shift.UserId = userId;
			db.Shifts.Add(shift);
		}
		else
		{
			// Eksisterende vakt: sjekk at den faktisk tilhører denne brukeren
			bool isOwner = await db.Shifts.AnyAsync(s => s.Id == shift.Id && s.UserId == userId);
			if (!isOwner)
			{
				throw new UnauthorizedAccessException("Du kan bare endre dine egne vakter.");
			}

			shift.UserId = userId;
			db.Shifts.Update(shift);
		}

		await db.SaveChangesAsync();
	}

	// Sletter en vakt, men bare hvis den tilhører innlogget bruker
	public async Task DeleteShiftAsync(int id)
	{
		var userId = await GetUserIdAsync();
		using var db = await dbFactory.CreateDbContextAsync();

		await db.Shifts
			.Where(s => s.Id == id && s.UserId == userId)
			.ExecuteDeleteAsync();
	}
}