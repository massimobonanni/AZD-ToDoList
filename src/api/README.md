# TaskDemo API

Azure Functions backend for the ToDoList application. Built with .NET 10 (isolated worker model) and Azure Table Storage for persistence.

## Overview

The API exposes a simple REST interface to manage tasks. All endpoints are HTTP-triggered Azure Functions with **anonymous** authorization level and are grouped under the `/api/tasks` route prefix.

## Endpoints

| Method   | Route              | Function       | Description                         |
|----------|--------------------|----------------|-------------------------------------|
| `GET`    | `/api/tasks`       | `GetTasks`     | Returns all tasks ordered by creation date (newest first). |
| `POST`   | `/api/tasks`       | `CreateTask`   | Creates a new task. Returns `201 Created` with the created item. |
| `PATCH`  | `/api/tasks/{id}`  | `CompleteTask` | Marks the task identified by `{id}` as completed. |
| `DELETE` | `/api/tasks/{id}`  | `DeleteTask`   | Deletes the task identified by `{id}`. Returns `204 No Content`. |

### Request / Response Models

**`TaskItem`** (response)
```json
{
  "id": "string (GUID)",
  "title": "string",
  "isCompleted": false,
  "createdAt": "2026-04-09T10:00:00+00:00"
}
```

**`CreateTaskRequest`** (body for `POST /api/tasks`)
```json
{
  "title": "string"
}
```
> `title` is required. The request returns `400 Bad Request` if it is missing or blank.

## Data Storage

Tasks are persisted in an **Azure Table Storage** table named `tasks`, using `"task"` as the fixed `PartitionKey` and a new GUID as the `RowKey` for each item.

## Configuration

Settings are defined in `local.settings.json` for local development and as application settings when deployed to Azure.

| Setting                        | Required | Description |
|-------------------------------|----------|-------------|
| `AzureWebJobsStorage`          | Yes      | Connection string for the Azure Functions internal storage (jobs, timers, etc.). Use `UseDevelopmentStorage=true` with Azurite locally. |
| `FUNCTIONS_WORKER_RUNTIME`     | Yes      | Must be `dotnet-isolated`. |
| `TABLE_SERVICE_URI`            | No*      | URI of the Azure Table Storage service (e.g. `https://<account>.table.core.windows.net`). When set, the app authenticates using **Managed Identity** (`DefaultAzureCredential`). Takes priority over `TABLE_SERVICE_CONNECTIONSTRING`. |
| `TABLE_SERVICE_CONNECTIONSTRING` | No*   | Connection string for Azure Table Storage. Used only when `TABLE_SERVICE_URI` is not set. Use `UseDevelopmentStorage=true` with the Azurite emulator locally. |

\* At least one of `TABLE_SERVICE_URI` or `TABLE_SERVICE_CONNECTIONSTRING` must be provided; otherwise the app throws at startup.

### Local development (`local.settings.json`)

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "TABLE_SERVICE_URI": "",
    "TABLE_SERVICE_CONNECTIONSTRING": "UseDevelopmentStorage=true"
  }
}
```

With both values set as shown above, the app falls back to using Azurite (local storage emulator) via the connection string.

### Azure deployment

In Azure, set `TABLE_SERVICE_URI` to the Table Storage endpoint of your storage account and leave `TABLE_SERVICE_CONNECTIONSTRING` empty. The function app's Managed Identity must be assigned the **Storage Table Data Contributor** role on the storage account.

## Telemetry

Application Insights is configured via `AddApplicationInsightsTelemetryWorkerService()`. Set the `APPLICATIONINSIGHTS_CONNECTION_STRING` application setting to enable telemetry collection.

## Project Structure

```
src/api/
├── Functions/
│   └── TaskFunctions.cs      # HTTP-triggered function definitions
├── Models/
│   └── TaskModels.cs         # TaskEntity (Table Storage), TaskItem, CreateTaskRequest
├── Program.cs                # Host builder and DI configuration
├── host.json                 # Azure Functions host configuration
├── local.settings.json       # Local development settings (not published)
└── TaskDemo.Api.csproj       # Project file (.NET 10, Azure Functions v4)
```
