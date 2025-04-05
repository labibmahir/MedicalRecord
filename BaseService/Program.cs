using BaseService.Services;
using Domain.Entities;
using Domain.MongoEntities;
using Infrastructure.Commands.Patients.Create;
using Infrastructure.Contexts;
using Infrastructure.Contracts;
using Infrastructure.Repositories;
using JWTAuthenticationManager;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(DataContext).Assembly);
});

var mongoClient = new MongoClient("mongodb://localhost:27017"); // Replace with your MongoDB connection string
var mongoDatabase = mongoClient.GetDatabase("MRBasic"); // Replace with your MongoDB database name

builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddSingleton<IMongoCollection<ReadPatient>>(sp =>
    mongoDatabase.GetCollection<ReadPatient>("patient"));

builder.Services.AddHostedService<MRBasicSyncBackgroundService>(); ;

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<JwtTokenHandler>();


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
