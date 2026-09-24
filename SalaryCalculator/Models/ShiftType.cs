namespace SalaryCalculator.Models
{
	public enum ShiftType
	{
		FirstShift,   // 1v 07:00-15:00
		MiddleShift,  // mv 10:00-18:00
		SecondShift,  // 2v 15:00-23:00
		NightShift,   // nv 23:00-07:00
		Custom        // egne tider, f.eks. 06:30-14:00
	}
}
