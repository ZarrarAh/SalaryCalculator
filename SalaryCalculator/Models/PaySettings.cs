using System.ComponentModel.DataAnnotations;

namespace SalaryCalculator.Models
{
	public class PaySettings
	{
		public int Id { get; set; }
		public string UserId { get; set; } = string.Empty;

		[Range(0, 2000, ErrorMessage = "Timelønn må være mellom 0 og 2000")]
		public decimal HourlyWage { get; set; }

		[Range(0, 500)]
		public decimal EveningAllowance { get; set; } = 16.59m;

		public TimeOnly EveningStart { get; set; } = new(17, 0);

		[Range(0, 500)]
		public decimal WeekendAllowance { get; set; } = 31.52m;

		[Range(0, 500)]
		public decimal NightAllowance { get; set; }	
		public TimeOnly NightStart { get; set; }
		public TimeOnly NightEnd { get; set; }

		[Range(0, 300)]
		public decimal RedDayPercent { get; set; } = 100;
	}
}
