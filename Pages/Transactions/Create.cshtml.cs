using System.ComponentModel.DataAnnotations;
using BudgetApp.Data;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Pages.Transactions;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public TransactionInput Input { get; set; } = new();

    
    public SelectList CategoryList { get; set; } = default!;

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        var tx = new Transaction
        {
            Date = Input.Date,
            Payee = Input.Payee,
            Amount = Input.Amount,
            
            CategoryId = Input.CategoryId,
            Notes = Input.Notes
        };

        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Index");
    }

    private async Task LoadSelectListsAsync()
    {
        
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        
        categories.Add(new Category{ Id = -1, Name = "+ Add new..."});
        CategoryList = new SelectList(categories, "Id", "Name");
    }

    public class TransactionInput
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [StringLength(100)]
        public string Payee { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "1000000")]
        public decimal Amount { get; set; }

        

        [Required]
        public int CategoryId { get; set; }

        public string? Notes { get; set; }
    }
}
