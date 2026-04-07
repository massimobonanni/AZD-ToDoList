using Azure;
using Azure.Data.Tables;

namespace TaskDemo.Api.Models;

public class TaskEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "task";
    public string RowKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static TaskItem ToModel(TaskEntity e) => new()
    {
        Id = e.RowKey,
        Title = e.Title,
        IsCompleted = e.IsCompleted,
        CreatedAt = e.CreatedAt
    };
}

public class TaskItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
}
