# TaskDemo Web

ASP.NET Core Razor Pages frontend for the ToDoList application. Built with .NET 10, it communicates with the [TaskDemo API](../api/README.md) over HTTP to display and manage tasks.

## Overview

The web app is a single-page Razor application (`Index.cshtml`) that lets users:

- View all tasks, ordered newest first.
- Add a new task by entering a title.
- Mark a task as completed.
- Delete a task.

All data operations are delegated to the backend Azure Functions API via `TaskApiClient`.

## Pages

| Page    | Route | Description                                      |
|---------|-------|--------------------------------------------------|
| `Index` | `/`   | Main task list with add / complete / delete actions. |

### Page Handlers (`Index`)

| Handler         | Method   | Description                                 |
|-----------------|----------|---------------------------------------------|
| `OnGetAsync`    | `GET`    | Loads and displays all tasks from the API.  |
| `OnPostCreateAsync` | `POST` | Creates a new task; requires a non-blank `title`. |
| `OnPostCompleteAsync` | `POST` | Marks a task as completed by `id`.     |
| `OnPostDeleteAsync`   | `POST` | Deletes a task by `id`.                |

## Services

### `TaskApiClient`

Typed `HttpClient` wrapper registered in DI. Exposes four methods that map directly to the API endpoints:

| Method                        | API Call                        |
|-------------------------------|---------------------------------|
| `GetTasksAsync()`             | `GET  /api/tasks`               |
| `CreateTaskAsync(title)`      | `POST /api/tasks`               |
| `CompleteTaskAsync(id)`       | `PATCH /api/tasks/{id}`         |
| `DeleteTaskAsync(id)`         | `DELETE /api/tasks/{id}`        |

## Configuration

Settings are loaded from `appsettings.json`, overridden by environment-specific files (e.g. `appsettings.Development.json`), and finally by environment variables at runtime.

| Setting                                   | Required | Default                       | Description |
|------------------------------------------|----------|-------------------------------|-------------|
| `API_URL`                                 | Yes      | `http://localhost:7071/`      | Base URL of the TaskDemo API. The trailing slash is handled automatically. |
| `Logging:LogLevel:Default`               | No       | `Information`                 | Minimum log level for the application. Set to `Debug` in Development. |
| `Logging:LogLevel:Microsoft.AspNetCore`  | No       | `Warning`                     | Log level for ASP.NET Core framework messages. |
| `APPLICATIONINSIGHTS_CONNECTION_STRING`  | No       | *(not set)*                   | Application Insights connection string. When set, telemetry is automatically collected. |

### `appsettings.json` (production defaults)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "API_URL": "http://localhost:7071/"
}
```

### `appsettings.Development.json` (local overrides)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```

Update `API_URL` in `appsettings.json` (or via an environment variable) to point to the deployed API when running in Azure.

### Azure deployment

Set the `API_URL` application setting to the base URL of the deployed Azure Functions app (e.g. `https://func-<name>.azurewebsites.net/`).

## Local Development

1. Start Azurite and the **TaskDemo API** project (default port `7071`).
2. Run the web app:
   ```bash
   dotnet run --project src/web
   ```
3. Open `https://localhost:5000` in a browser.

The default `API_URL` in `appsettings.json` already points to `http://localhost:7071/`, so no additional configuration is needed for local development.

## Telemetry

Application Insights is configured via `AddApplicationInsightsTelemetry()`. Set the `APPLICATIONINSIGHTS_CONNECTION_STRING` application setting to enable telemetry collection.

## Project Structure

```
src/web/
├── Models/
│   └── TaskItem.cs               # View model matching the API response
├── Pages/
│   ├── Index.cshtml              # Main task list page (Razor)
│   ├── Index.cshtml.cs           # Page model with handlers
│   └── Shared/
│       └── _Layout.cshtml        # Shared HTML layout (Bootstrap)
├── Services/
│   └── TaskApiClient.cs          # Typed HTTP client for the API
├── Properties/
│   └── launchSettings.json       # Local launch profiles
├── appsettings.json              # Production configuration
├── appsettings.Development.json  # Development overrides
├── Program.cs                    # Host builder and DI configuration
└── TaskDemo.Web.csproj           # Project file (.NET 10, ASP.NET Core)
```
