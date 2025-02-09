using System.Text.Json.Serialization;
using TheBench.Logic.Adapters;
using TheBench.Logic.Database;
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
        config.WithOrigins("http://localhost:3000", "https://the-bench-ui-185160f67ef3.herokuapp.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<UserContext>();
builder.Services.AddScoped<IUserAdapter, UserAdapter>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<IdService>();
Console.WriteLine("Services registered");
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
