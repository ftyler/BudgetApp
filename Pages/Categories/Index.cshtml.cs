using BudgetApp.Data;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Pages.Categories;

public class CategoriesIndexModel : PageModel
{
    private readonly AppDbContext _db;
    public CategoriesIndexModel(AppDbContext db) => _db = db;

    public IList<Category> Categories { get; set; } = new List<Category>();
    [BindProperty]
    public int DeleteId { get; set; }
    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var cat = await _db.Categories.Include(c => c.Transactions)
                                       .FirstOrDefaultAsync(c => c.Id == DeleteId);
        if (cat == null)
        {
            return RedirectToPage();
        }
        if (cat.Transactions.Any())
        {
            ErrorMessage = "Cannot delete a category that has transactions.";
            return RedirectToPage();
        }
        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
