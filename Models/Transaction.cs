using System;

namespace BudgetApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Payee { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? Notes { get; set; }
    }
}
