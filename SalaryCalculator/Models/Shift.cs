namespace SalaryCalculator.Models
{
	public class Shift
	{
		public int Id { get; set; }
		public string UserId { get; set; } = string.Empty;
		public DateOnly Date { get; set; }
		public ShiftType ShiftType { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public int BreakMinutes { get; set; } = 30;
		public bool IsRedDay { get; set; }
		public string? Note { get; set; }

	}
}
