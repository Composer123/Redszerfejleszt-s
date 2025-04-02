using Microsoft.EntityFrameworkCore;
using Raktar.DataContext;
using Raktar.Services;

#region Builder services

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Test
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
{
    options.UseSqlServer("Server=localhost;Database=WarehouseDB;Trusted_Connection=True;TrustServerCertificate=True;");
});

builder.Services.AddScoped<IAddressService, AddressService>();

// Services
builder.Services.AddScoped<IBlockService, BlockService>();

//Test 
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

#endregion

#region Request

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion