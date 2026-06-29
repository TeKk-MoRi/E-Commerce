using ECommerce.BuildingBlocks.Authentication;
using IdentityAccess.Application;
using IdentityAccess.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Infrastructure.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKeycloakJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in ApplicationPermissions.All)
    {
        options.AddPolicy(permission, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.Requirements.Add(new PermissionRequirement(permission));
        });
    }

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers();

builder.Services
    .RegisterIdentityAccessApplicationServices()
    .RegisterIdentityAccessInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter only your JWT access token. Do not include Bearer."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityAccess API v1");
    });
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();