namespace SalaryCalculator.Models
{
	public class PeriodPay
	{
		public DateOnly Start { get; set; }
		public DateOnly End { get; set; }
		public List<ShiftPay> Shifts { get; set; } = new();

		// Summene regnes ut fra alle vaktene
		public decimal WorkedHours => Shifts.Sum(s => s.WorkedHours);
		public decimal EveningHours => Shifts.Sum(s => s.EveningHours);
		public decimal WeekendHours => Shifts.Sum(s => s.WeekendHours);
		public decimal RedDayHours => Shifts.Sum(s => s.RedDayHours);

		public decimal BasePay => Shifts.Sum(s => s.BasePay);
		public decimal EveningPay => Shifts.Sum(s => s.EveningPay);
		public decimal WeekendPay => Shifts.Sum(s => s.WeekendPay);
		public decimal RedDayPay => Shifts.Sum(s => s.RedDayPay);

		public decimal Total => Shifts.Sum(s => s.Total);
	}
}
