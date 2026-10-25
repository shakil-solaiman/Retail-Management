using Microsoft.EntityFrameworkCore;
using RetailManagement.API.Data;
using RetailManagement.API.Interfaces;
using RetailManagement.API.Repositories;


var builder = WebApplication.CreateBuilder(args);


// Register database context
builder.Services.AddDbContext<RetailDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// Register controllers
builder.Services.AddControllers();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();


// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();