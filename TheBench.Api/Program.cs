using System.Text.Json.Serialization;
using TheBench.Logic.Adapters;
using TheBench.Logic.Database;
using TheBench.Logic.Models;
using TheBench.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", config =>
    {
        config.WithOrigins(
                "http://localhost:5173", 
                "https://staging.the-bench.us",
                "https://www.the-bench.us"
                )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<UserContext>();
builder.Services.AddScoped<IUserAdapter, UserAdapter>();
builder.Services.AddScoped<ITeamAdapter, TeamAdapter>();
builder.Services.AddScoped<IGameAdapter, GameAdapter>();
builder.Services.AddScoped<ISubstituteRequestAdapter, SubstituteRequestAdapter>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IdService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
