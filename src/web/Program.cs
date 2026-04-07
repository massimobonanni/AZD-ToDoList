using TaskDemo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();

var apiUrl = builder.Configuration["API_URL"] ?? "http://localhost:7071/";
builder.Services.AddHttpClient<TaskApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl.TrimEnd('/') + '/');
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
