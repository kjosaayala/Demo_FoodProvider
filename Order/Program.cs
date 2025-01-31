using Microsoft.EntityFrameworkCore;
using Order.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("InventoryService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7138/"); // URL del InventoryService
});

builder.Services.AddHttpClient("PaymentsService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7259/"); // URL del PaymentsService
});

builder.Services.AddDbContext<OrderContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDB")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();

app.Run();
