using System.Text.Json.Serialization;
using Claims;
using Claims.Controllers;
using Claims.Data;
using Claims.Models;
using Claims.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDatabaseDAO>();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDbContext<ClaimsContext>(
    options =>
    {
        var client = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    }
);

builder.Services.AddDbContext<CoversContext>(
    options =>
    {
        var client = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    }
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IClaimsService, ClaimsService>();
builder.Services.AddTransient<ICoversService, CoversService>();
builder.Services.AddTransient<IAuditsService, AuditsService>();


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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }
