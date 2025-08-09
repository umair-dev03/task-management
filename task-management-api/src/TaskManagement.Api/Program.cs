using TaskManagement.Api.Middleware;
using TaskManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TaskManagement API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<TaskManagement.Application.Common.IAuthService, TaskManagement.Infrastructure.Services.AuthService>();

// Register generic repository for DI
builder.Services.AddScoped(typeof(TaskManagement.Application.Common.IRepository<>), typeof(TaskManagement.Infrastructure.Repositories.Repository<>));

// Register Task Handlers for DI
builder.Services.AddScoped<TaskManagement.Application.Common.IQueryHandler<TaskManagement.Application.Queries.Tasks.GetTasksQuery, TaskManagement.Application.Common.Result<TaskManagement.Application.Common.PagedResult<TaskManagement.Application.Common.TaskDto>>>, TaskManagement.Application.Queries.Tasks.GetTasksQueryHandler>();
builder.Services.AddScoped<TaskManagement.Application.Common.IQueryHandler<TaskManagement.Application.Queries.Tasks.GetTaskByIdQuery, TaskManagement.Application.Common.Result<TaskManagement.Application.Common.TaskDto>>, TaskManagement.Application.Queries.Tasks.GetTaskByIdQueryHandler>();
builder.Services.AddScoped<TaskManagement.Application.Common.ICommandHandler<TaskManagement.Application.Commands.Tasks.CreateTaskCommand, TaskManagement.Application.Common.Result<TaskManagement.Application.Common.TaskDto>>, TaskManagement.Application.Commands.Tasks.CreateTaskCommandHandler>();
builder.Services.AddScoped<TaskManagement.Application.Common.ICommandHandler<TaskManagement.Application.Commands.Tasks.UpdateTaskCommand, TaskManagement.Application.Common.Result<TaskManagement.Application.Common.TaskDto>>, TaskManagement.Application.Commands.Tasks.UpdateTaskCommandHandler>();
builder.Services.AddScoped<TaskManagement.Application.Common.ICommandHandler<TaskManagement.Application.Commands.Tasks.UpdateTaskStatusCommand, TaskManagement.Application.Common.Result<TaskManagement.Application.Common.TaskDto>>, TaskManagement.Application.Commands.Tasks.UpdateTaskStatusCommandHandler>();
builder.Services.AddScoped<TaskManagement.Application.Common.ICommandHandler<TaskManagement.Application.Commands.Tasks.DeleteTaskCommand, TaskManagement.Application.Common.Result<bool>>, TaskManagement.Application.Commands.Tasks.DeleteTaskCommandHandler>();
builder.Services.AddScoped<TaskManagement.Application.Common.ICommandHandler<TaskManagement.Application.Commands.Auth.LoginCommand, TaskManagement.Application.Common.Result<TaskManagement.Application.Common.LoginResponseDto>>, TaskManagement.Application.Commands.Auth.LoginCommandHandler>();

var app = builder.Build();

// SEED DATA
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
    dbContext.Database.Migrate();
    TaskManagement.Infrastructure.SeedData.Initialize(dbContext);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
