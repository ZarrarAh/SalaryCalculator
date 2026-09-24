namespace SalaryCalculator.Models
{
	public class PaySettings
	{
		public int Id { get; set; }
		public string UserId { get; set; } = string.Empty;
		public decimal HourlyWage { get; set; }
		public decimal EveningAllowance { get; set; } = 16.59m;
		public TimeOnly EveningStart { get; set; } = new(17, 0);
		public decimal WeekendAllowance { get; set; } = 31.52m;
		public decimal NightAllowance { get; set; }	
		public TimeOnly NightStart { get; set; }
		public TimeOnly NightEnd { get; set; }
		public decimal RedDayPercent { get; set; } = 100;
	}
}
