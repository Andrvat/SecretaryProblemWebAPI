using SecretaryProblemWebAPI;
using SecretaryProblemWebAPI.Generators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<AttemptsDbConfigurator>();
builder.Services.AddDbContext<AttemptsDbContext>();

builder.Services.AddSingleton<AttemptsNumberProvider>();
builder.Services.AddSingleton<ContendersFileGenerator>();
builder.Services.AddSingleton<ContendersDbGenerator>();

builder.Services.AddSingleton<Hall>();
builder.Services.AddSingleton<Friend>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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