using GroceryDeliveryAPI.Context;
using GroceryDeliveryAPI.Models;
using GroceryDeliveryAPI.Managers;
using GroceryDeliveryAPI.Seeding;
using Microsoft.EntityFrameworkCore; // Added this using directive
using System;
using Microsoft.OpenApi.Models;
using GroceryDeliveryAPI.Helpers;
using GroceryDeliveryAPI.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
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

// Register AuthHelper
builder.Services.AddScoped<AuthHelpers>();

// Register OrderManager
builder.Services.AddScoped<OrderManager>();

// Register DeliveryManager
builder.Services.AddScoped<DeliveryManager>();

// Register DeliveryPersonsManager
builder.Services.AddScoped<DeliveryPersonsManager>();

// Register background services
builder.Services.AddHostedService<UnassignedDeliveryBackgroundService>();


//Only run if database is empty - check is handled inside SeedDataAsync method

//builder.Services.AddScoped<Seeder>();
builder.Services.AddScoped<GroceryDataSeeder>();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    // Get the db context
    var dbContext = scope.ServiceProvider.GetRequiredService<GroceryDeliveryContext>();

    // Ensure database exists and is up to date
    await dbContext.Database.MigrateAsync();

    // Now proceed with data import only if database is empty
    var seeder = scope.ServiceProvider.GetRequiredService<GroceryDataSeeder>();
    await seeder.ImportDataAsync(Path.Combine(Directory.GetCurrentDirectory(), "Seeding/GroceryStoreDataset/dataset/Groceries.csv"));
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

var AllowSpecificOrigins = "AllowFrontendOrigin";
builder.Services.AddCors(o => o.AddPolicy(AllowSpecificOrigins, builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(AllowSpecificOrigins);
}

// Configure static files for product images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Seeding/GroceryStoreDataset/dataset/iconic-images-and-descriptions")),
    RequestPath = "/iconic-images-and-descriptions"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
