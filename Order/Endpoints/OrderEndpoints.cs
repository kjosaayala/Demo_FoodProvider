using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Models;
using System.Text.Json;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/orders", async (OrderRequest orderRequest, OrderContext db, IHttpClientFactory clientFactory) =>
        {
            var inventoryClient = clientFactory.CreateClient("InventoryService");
            var response = await inventoryClient.GetAsync($"products/{orderRequest.ProductId}");

            if (!response.IsSuccessStatusCode)
            {
                return Results.BadRequest("Product is out of stock.");
            }

            // 🔹 Deserializar la respuesta como JsonElement
            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);
            var product = doc.RootElement;

            // 🔹 Extraer valores manualmente
            var productId = product.GetProperty("id").GetInt32();

            var order = new Order.Data.Order
            {
                OrderId = string.Empty,
                ProductId = productId,
                TotalAmount = orderRequest.TotalAmount,
                CustomerId = orderRequest.CustomerId,
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            // Consultar PaymentsService para procesar el pago
            var paymentClient = clientFactory.CreateClient("PaymentsService");
            var paymentResponse = await paymentClient.PostAsJsonAsync("api/Payments/process", new
            {
                orderId = order.Id,
                amount = order.TotalAmount,
                customerId = order.CustomerId,
            });

            if (!paymentResponse.IsSuccessStatusCode)
            {
                db.Orders.Remove(order);
                await db.SaveChangesAsync();

                return Results.BadRequest("Payment failed.");
            }

            return Results.Created($"/orders/{order.Id}", order);
        })
        .WithName("CreateOrder")
        .WithTags("Orders");

        app.MapGet("/orders", async (OrderContext db) =>
        {
            return await db.Orders.ToListAsync();
        })
        .WithName("GetOrders")
        .WithTags("Orders");

        app.MapGet("/orders/{id}", async (int id, OrderContext db) =>
        {
            var order = await db.Orders.FindAsync(id);
            return order != null ? Results.Ok(order) : Results.NotFound();
        })
        .WithName("GetOrderById")
        .WithTags("Orders");

        app.MapPut("/orders/{id}", async (int id, Order.Data.Order updatedOrder, OrderContext db) =>
        {
            var order = await db.Orders.FindAsync(id);
            if (order == null) return Results.NotFound();

            order.CustomerId = updatedOrder.CustomerId;
            order.TotalAmount = updatedOrder.TotalAmount;
            order.Status = updatedOrder.Status;
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateOrder")
        .WithTags("Orders");

        app.MapDelete("/orders/{id}", async (int id, OrderContext db) =>
        {
            var order = await db.Orders.FindAsync(id);
            if (order == null) return Results.NotFound();

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("RemoveOrder")
        .WithTags("Orders");
    }
}
