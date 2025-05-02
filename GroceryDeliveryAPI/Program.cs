using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Seeding;

using Microsoft.EntityFrameworkCore; // Added this using directive
using System;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroceryDelivery API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddDbContextPool<GroceryDeliveryContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ProductManager
builder.Services.AddScoped<ProductManager>();

// Register CategoryManager
builder.Services.AddScoped<CategoryManager>();

// Register UserManager
builder.Services.AddScoped<UserManager>();


//Only run if database is empty - check is handled inside SeedDataAsync method

//builder.Services.AddScoped<Seeder>();
builder.Services.AddScoped<GroceryDataSeeder>();

using(var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    // Get the db context
    var dbContext = scope.ServiceProvider.GetRequiredService<GroceryDeliveryContext>();

    // Ensure database exists and is up to date
    await dbContext.Database.MigrateAsync();

    // Now proceed with data import only if database is empty
    var seeder = scope.ServiceProvider.GetRequiredService<GroceryDataSeeder>();
    await seeder.ImportDataAsync("Seeding\\GroceryStoreDataset\\dataset\\Groceries.CSV");
}
/*
builder.Services.AddScoped<Seeder>();


// Create a scope to resolve the Seeder service properly
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    await seeder.SeedDataAsync("Seeding\\Dataset\\GroceryStoreDataset\\dataset\\Groceries.CSV");
}
*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure static files for product images
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
