namespace SalaryCalculator.Models
{
	public class ShiftPay
	{
		// Timer
		public decimal WorkedHours { get; set; }
		public decimal EveningHours { get; set; }
		public decimal WeekendHours { get; set; }
		public decimal RedDayHours { get; set; }

		// Beløp i kroner
		public decimal BasePay { get; set; }
		public decimal EveningPay { get; set; }
		public decimal WeekendPay { get; set; }
		public decimal RedDayPay { get; set; }

		// Totalen regnes ut automatisk
		public decimal Total => BasePay + EveningPay + WeekendPay + RedDayPay;
	}
}
