using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore; // Added this using directive
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContextPool<GroceryDeliveryContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//Only run if database is empty - check is handled inside SeedDataAsync method
builder.Services.AddScoped<Seeder>();

// Create a scope to resolve the Seeder service properly
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    await seeder.SeedDataAsync("Seeding\\Dataset\\GroceryStoreDataset\\dataset\\Groceries.CSV");
}

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
