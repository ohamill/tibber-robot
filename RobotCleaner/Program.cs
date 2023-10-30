using System.Text.Json.Serialization;
using RobotCleaner.Db;
using RobotCleaner.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Allow enums to be deserialized from strings instead of integers (i.e. users can specify the robot's direction as "North", "East" etc instead of 0, 1, etc)
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var postgresPw = File.ReadAllText(Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_FILE")!);
// The DB service and client are added as singletons because they can be reused across requests
builder.Services.AddSingleton<IDbClient, PostgresClient>(sp => new PostgresClient(Environment.GetEnvironmentVariable("HOSTNAME"), Environment.GetEnvironmentVariable("POSTGRES_USER"), postgresPw, Environment.GetEnvironmentVariable("POSTGRES_DB")));
builder.Services.AddSingleton<IDbHandler, PostgresHandler>();
// The robot is added as a scoped object because it should be unique per HTTP request
builder.Services.AddScoped<IRobot, Robot>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();