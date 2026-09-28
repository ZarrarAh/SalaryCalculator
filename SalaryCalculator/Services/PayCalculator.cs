using SalaryCalculator.Models;
using System.Numerics;

namespace SalaryCalculator.Services
{
	public class PayCalculator
	{


		public (DateTime Start, DateTime End) GetShiftPeriod(Shift shift)
		{
			// Slår sammen dato og klokkeslett til DateTime
			DateTime start = shift.Date.ToDateTime(shift.StartTime);
			DateTime end = shift.Date.ToDateTime(shift.EndTime);

			// Hvis vakten slutter før (eller samtidig som) dem starter, går den over midnatt	
			if (shift.EndTime <= shift.StartTime)
			{
				end = end.AddDays(1);
			}

			return (start, end);
		}

		public decimal GetOverlapHours(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
		{
			// Overlappen starter på det SENESTE av de to startpunktene
			DateTime overlapStart = start1 > start2 ? start1 : start2;

			// Overlappen slutter på det TIDLIGSTE av de to sluttpunktene
			DateTime overlapEnd = end1 < end2 ? end1 : end2;

			// Ingen overlapp
			if(overlapEnd <= overlapStart)
			{
				return 0;
			}

			return (decimal)(overlapEnd - overlapStart).TotalHours;
		}

		public decimal GetWorkedHours(Shift shift)
		{
			var (start, end) = GetShiftPeriod(shift);
			var workedHours = (decimal)(end - start).TotalHours;
			// Trekk fra pausen
			workedHours -= shift.BreakMinutes / 60m;
			return workedHours;
		}

		public decimal GetEveningHours(Shift shift, PaySettings settings) 
		{
			var (start, end) = GetShiftPeriod(shift);
			decimal total = 0;

			// En vakt kan gå over to datoer, så vi sjekker startdagen og dagen etter
			for (int i = 0; i < 2; i++)
			{
				DateOnly day = shift.Date.AddDays(i);

				//Kveldsvinduet: fra EveningStart til midnatt samme dag
				DateTime windowStart = day.ToDateTime(settings.EveningStart);
				DateTime windowEnd = day.AddDays(1).ToDateTime(TimeOnly.MinValue);

				total += GetOverlapHours(start, end, windowStart, windowEnd); 
			}

			return total;
		}

		public decimal GetWeekendHours(Shift shift)
		{
			var (start, end) = GetShiftPeriod(shift);
			decimal total = 0;
	
			//En vakt kan gå over to datoer, så vi sjekker startdagen og dagen etter
			for (int i = 0; i < 2; i++)
			{
				DateOnly day = shift.Date.AddDays(i);

				// Hopp over dager som ikke er lørdag eller søndag
				if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
				{
					continue;
				}

				// Helgevinduet: Hele døgnet, fra 00:00 til 00:00 neste dag
				DateTime windowStart = day.ToDateTime(TimeOnly.MinValue);
				DateTime windowEnd = day.AddDays(1).ToDateTime(TimeOnly.MinValue);

				total += GetOverlapHours(start, end, windowStart, windowEnd);
			}

			return total;
		}

		public ShiftPay CalculateShiftPay(Shift shift, PaySettings settings)
		{
			// 1. Regn ut timene med metodene vi allerede har
			var result = new ShiftPay
			{
				WorkedHours = GetWorkedHours(shift),
				EveningHours = GetEveningHours(shift, settings),
				WeekendHours = GetWeekendHours(shift)
			};

			// Rød dag: alle betalte timer gir tillegg
			result.RedDayHours = shift.IsRedDay ? result.WorkedHours : 0;

			// 2. Gang timene med satsene
			result.BasePay = result.WorkedHours * settings.HourlyWage;
			result.EveningPay = result.EveningHours * settings.EveningAllowance;
			result.WeekendPay = result.WeekendHours * settings.WeekendAllowance;
			result.RedDayPay = result.RedDayHours * settings.HourlyWage * settings.RedDayPercent / 100;

			return result;
		}

		public (DateOnly Start, DateOnly End) GetPayPeriod(DateOnly date)
		{
			// Fra og med den 16. starter en ny periode
			DateOnly start = date.Day >= 16
				? new DateOnly(date.Year, date.Month, 16)
				: new DateOnly(date.Year, date.Month, 16).AddMonths(-1);

			// Slutten er dagen før den 16. neste måned, altså den 15.
			DateOnly end = start.AddMonths(1).AddDays(-1);

			return (start, end);
		}

		public PeriodPay CalculatePeriodPay(List<Shift> shifts, PaySettings settings, DateOnly dateInPeriod)
		{
			var (start, end) = GetPayPeriod(dateInPeriod);
			var result = new PeriodPay { Start = start, End = end };

			foreach (var shift in shifts)
			{
				// Ta bare med vakter som starter innenfor perioden
				if (shift.Date < start || shift.Date > end)
				{
					continue;
				}

				result.Shifts.Add(CalculateShiftPay(shift, settings));
			}

			return result;
		}
	}
}
