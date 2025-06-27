using System.ComponentModel.DataAnnotations;
using BudgetApp.Data;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetApp.Pages.Categories;

public class CategoryCreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CategoryCreateModel(AppDbContext db) => _db = db;

    [BindProperty]
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        _db.Categories.Add(new Category { Name = Name });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
