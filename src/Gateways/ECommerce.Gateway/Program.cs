var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet(
    "/",
    () => Results.Ok(new
    {
        Service = "ECommerce Gateway",
        Status = "Running"
    }));

app.MapGet(
    "/health",
    () => Results.Ok(new
    {
        Status = "Healthy"
    }));

app.MapReverseProxy();

app.Run();