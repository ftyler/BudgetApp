namespace BudgetApp.Models
{
    public class BudgetSetting
    {
        public int Id { get; set; } = 1; // always single row
        public decimal MonthlyBudget { get; set; }
    }
}
