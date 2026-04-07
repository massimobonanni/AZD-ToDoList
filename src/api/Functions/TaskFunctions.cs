using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskDemo.Api.Models;

namespace TaskDemo.Api.Functions;

public class TaskFunctions
{
    private readonly TableServiceClient _tableService;
    private readonly ILogger<TaskFunctions> _logger;
    private const string TableName = "tasks";

    public TaskFunctions(TableServiceClient tableService, ILogger<TaskFunctions> logger)
    {
        _tableService = tableService;
        _logger = logger;
    }

    [Function("GetTasks")]
    public async Task<IActionResult> GetTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks")] HttpRequest req)
    {
        _logger.LogInformation("Getting all tasks");

        var tableClient = _tableService.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();

        var tasks = new List<TaskItem>();
        await foreach (var entity in tableClient.QueryAsync<TaskEntity>(filter: "PartitionKey eq 'task'"))
        {
            tasks.Add(TaskEntity.ToModel(entity));
        }

        return new OkObjectResult(tasks.OrderByDescending(t => t.CreatedAt));
    }

    [Function("CreateTask")]
    public async Task<IActionResult> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonSerializer.Deserialize<CreateTaskRequest>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (string.IsNullOrWhiteSpace(input?.Title))
            return new BadRequestObjectResult(new { error = "Title is required" });

        var tableClient = _tableService.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();

        var entity = new TaskEntity
        {
            PartitionKey = "task",
            RowKey = Guid.NewGuid().ToString(),
            Title = input.Title.Trim(),
            IsCompleted = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await tableClient.AddEntityAsync(entity);
        _logger.LogInformation("Created task {Id}: {Title}", entity.RowKey, entity.Title);

        return new ObjectResult(TaskEntity.ToModel(entity)) { StatusCode = StatusCodes.Status201Created };
    }

    [Function("CompleteTask")]
    public async Task<IActionResult> CompleteTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "tasks/{id}")] HttpRequest req,
        string id)
    {
        var tableClient = _tableService.GetTableClient(TableName);

        try
        {
            var response = await tableClient.GetEntityAsync<TaskEntity>("task", id);
            var entity = response.Value;
            entity.IsCompleted = true;
            await tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);

            _logger.LogInformation("Completed task {Id}", id);
            return new OkObjectResult(TaskEntity.ToModel(entity));
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return new NotFoundObjectResult(new { error = $"Task '{id}' not found" });
        }
    }

    [Function("DeleteTask")]
    public async Task<IActionResult> DeleteTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "tasks/{id}")] HttpRequest req,
        string id)
    {
        var tableClient = _tableService.GetTableClient(TableName);

        try
        {
            await tableClient.DeleteEntityAsync("task", id);
            return new NoContentResult();
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return new NotFoundObjectResult(new { error = $"Task '{id}' not found" });
        }
    }
}
