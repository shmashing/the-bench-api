using System.Text.Json.Serialization;
using TheBench.Logic.Adapters;
using TheBench.Logic.Config;
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

var serviceConfiguration = LoadConfiguration(builder);

builder.Services.AddSingleton(serviceConfiguration);
builder.Services.AddSingleton(serviceConfiguration.MailGunConfiguration);
builder.Services.AddDbContext<UserContext>();
builder.Services.AddScoped<IUserAdapter, UserAdapter>();
builder.Services.AddScoped<ITeamAdapter, TeamAdapter>();
builder.Services.AddScoped<IGameAdapter, GameAdapter>();
builder.Services.AddScoped<INotificationAdapter, NotificationAdapter>();
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

static ApiConfiguration LoadConfiguration(WebApplicationBuilder builder)
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
    if (env == "Development")
    {
        Console.WriteLine("Loading configuration from appsettings.json");
        var config = builder.Configuration.GetSection("ApiConfiguration").Get<ApiConfiguration>() 
               ?? throw new InvalidOperationException("API configuration is missing or invalid.");
        Console.WriteLine($"Config: {config}");
        return config;
    }
    
    var baseUiUri = Environment.GetEnvironmentVariable("BASE_UI_URI") 
                    ?? throw new InvalidOperationException("Base UI URL is not set in environment variables.");
    var databaseConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                   ?? throw new InvalidOperationException("Database connection string is not set in environment variables.");
    var mailgunBaseUri = Environment.GetEnvironmentVariable("MAILGUN_BASE_URI") 
                         ?? throw new InvalidOperationException("Mailgun base URI is not set in environment variables.");
    var mailgunDomain = Environment.GetEnvironmentVariable("MAILGUN_DOMAIN") 
                        ?? throw new InvalidOperationException("Mailgun domain is not set in environment variables.");
    var mailgunApiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY") 
                        ?? throw new InvalidOperationException("Mailgun API key is not set in environment variables.");
    
    var mailGunConfig = new MailGunConfiguration
    {
        BaseUri = mailgunBaseUri,
        Domain = mailgunDomain,
        ApiKey = mailgunApiKey
    };
    
    return new ApiConfiguration(baseUiUri,databaseConnectionString,mailGunConfig);
}