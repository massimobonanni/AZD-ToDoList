using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskDemo.Web.Models;
using TaskDemo.Web.Services;

namespace TaskDemo.Web.Pages;

public class IndexModel : PageModel
{
    private readonly TaskApiClient _api;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<TaskItem> Tasks { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public IndexModel(TaskApiClient api, ILogger<IndexModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Tasks = await _api.GetTasksAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch tasks from API");
            ErrorMessage = "Could not load tasks. Is the API running?";
        }
    }

    public async Task<IActionResult> OnPostCreateAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return RedirectToPage();

        try
        {
            await _api.CreateTaskAsync(title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create task");
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCompleteAsync(string id)
    {
        try
        {
            await _api.CompleteTaskAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete task {Id}", id);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        try
        {
            await _api.DeleteTaskAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete task {Id}", id);
        }

        return RedirectToPage();
    }
}
