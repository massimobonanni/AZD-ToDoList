using System.Net.Http.Json;
using TaskDemo.Web.Models;

namespace TaskDemo.Web.Services;

public class TaskApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TaskApiClient> _logger;

    public TaskApiClient(HttpClient httpClient, ILogger<TaskApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<TaskItem>>("api/tasks") ?? [];
    }

    public async Task CreateTaskAsync(string title)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tasks", new { title });
        response.EnsureSuccessStatusCode();
    }

    public async Task CompleteTaskAsync(string id)
    {
        var response = await _httpClient.PatchAsync($"api/tasks/{id}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTaskAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }
}
