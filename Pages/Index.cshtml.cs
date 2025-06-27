using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using BudgetApp.Data;
using BudgetApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _db;

    public decimal Budget { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining { get; set; }
    public IList<Transaction>? RecentTransactions { get; set; }
    public IList<Category> AllCategories { get; set; } = new List<Category>();

    [BindProperty]
    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal InputBudget { get; set; }

    [BindProperty]
    public int TxId { get; set; }
    [BindProperty]
    public int NewCategoryId { get; set; }
    [BindProperty]
    public int DeleteId { get; set; }
    [BindProperty]
    public decimal NewAmount { get; set; }
    [BindProperty]
    public string? NewPayee { get; set; }

    // new transaction fields
    [BindProperty]
    public DateTime NewTxDate { get; set; }
    [BindProperty]
    public string? NewTxPayee { get; set; }
    [BindProperty]
    public decimal NewTxAmount { get; set; }
    [BindProperty]
    public int NewTxCategoryId { get; set; }
    [BindProperty]
    public string? NewDateStr { get; set; }

    public IndexModel(ILogger<IndexModel> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task OnGetAsync()
    {
        Budget = (await _db.BudgetSettings.FirstOrDefaultAsync())?.MonthlyBudget ?? 0m;
        InputBudget = Budget;
        Spent = _db.Transactions.AsEnumerable().Sum(t => t.Amount);
        Remaining = Budget - Spent;
        RecentTransactions = _db.Transactions
            .OrderByDescending(t => t.Date)
            .Take(10)
            .Include(t => t.Category)
            .ToList();
        AllCategories = _db.Categories.OrderBy(c => c.Name).ToList();
    }

    public async Task<IActionResult> OnPostUpdateCategoryAsync()
    {
        var tx = await _db.Transactions.FindAsync(TxId);
        if (tx == null) return RedirectToPage();
        tx.CategoryId = NewCategoryId;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateDateAsync()
    {
        var tx = await _db.Transactions.FindAsync(TxId);
        if(tx!=null){
            DateOnly parsed;
            var formats = new[]{"dd/MM/yyyy","yyyy-MM-dd"};
            if(DateOnly.TryParseExact(NewDateStr?.Trim() ?? string.Empty, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed))
                tx.Date = parsed.ToDateTime(TimeOnly.MinValue);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdatePayeeAsync()
    {
        var tx = await _db.Transactions.FindAsync(TxId);
        if(tx!=null && !string.IsNullOrWhiteSpace(NewPayee)){
            tx.Payee = NewPayee;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddTxAsync()
    {
        if(NewTxAmount<=0 || string.IsNullOrWhiteSpace(NewTxPayee) || NewTxCategoryId<=0){
            await OnGetAsync();
            ModelState.AddModelError(string.Empty, "All fields required");
            return Page();
        }
        var tx = new Transaction{
            Date = NewTxDate==default? DateTime.Today: NewTxDate,
            Payee = NewTxPayee!,
            Amount = NewTxAmount,
            CategoryId = NewTxCategoryId
        };
        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAmountAsync()
    {
        var tx = await _db.Transactions.FindAsync(TxId);
        if(tx!=null){
            tx.Amount = NewAmount;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var tx = await _db.Transactions.FindAsync(DeleteId);
        if(tx!=null){
            _db.Transactions.Remove(tx);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var setting = await _db.BudgetSettings.FirstOrDefaultAsync();
        if (setting == null)
        {
            setting = new BudgetSetting { MonthlyBudget = InputBudget };
            _db.BudgetSettings.Add(setting);
        }
        else
        {
            setting.MonthlyBudget = InputBudget;
            _db.BudgetSettings.Update(setting);
        }
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}

