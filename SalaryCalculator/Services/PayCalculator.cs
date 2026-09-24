using SalaryCalculator.Models;

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
	}
}
