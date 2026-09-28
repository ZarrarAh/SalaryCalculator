using SalaryCalculator.Models;
using SalaryCalculator.Services;

namespace SalaryCalculator.Tests
{
	public class PayCalculatorTests 
	{
		private readonly PayCalculator _calculator = new();

		[Fact]
		public void GetWorkedHours_SecondShiftWithBreak_Returns7_5()
		{
			// Arrange
			var shift = new Shift
			{
				Date = new DateOnly(2025, 12, 19),
				StartTime = new TimeOnly(15, 0),
				EndTime = new TimeOnly(23, 0),
				BreakMinutes = 30,
			};

			// Act
			decimal result = _calculator.GetWorkedHours(shift);

			// Assert
			Assert.Equal(7.5m, result);
		}

		[Fact]
		public void GetWeekendHours_NightShiftFridayToSaturday_Returns7()
		{
			// Arrange
			var shift = new Shift
			{
				Date = new DateOnly(2025, 12, 19),   // fredag
				StartTime = new TimeOnly(23, 0),
				EndTime = new TimeOnly(7, 0),
				BreakMinutes = 30
			};

			// Act
			decimal result = _calculator.GetWeekendHours(shift);

			// Assert
			Assert.Equal(7m, result);
		}
	}
}