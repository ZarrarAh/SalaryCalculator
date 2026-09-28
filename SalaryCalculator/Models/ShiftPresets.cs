namespace SalaryCalculator.Models;

public static class ShiftPresets
{
	// Standardtider for hver vakttype
	public static (TimeOnly Start, TimeOnly End) GetTimes(ShiftType type) => type switch
	{
		ShiftType.FirstShift => (new TimeOnly(7, 0), new TimeOnly(15, 0)),
		ShiftType.MiddleShift => (new TimeOnly(10, 0), new TimeOnly(18, 0)),
		ShiftType.SecondShift => (new TimeOnly(15, 0), new TimeOnly(23, 0)),
		ShiftType.NightShift => (new TimeOnly(23, 0), new TimeOnly(7, 0)),
		_ => (new TimeOnly(15, 0), new TimeOnly(23, 0))  // Custom: startverdi du kan endre
	};
}