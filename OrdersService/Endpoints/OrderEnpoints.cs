using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using static OrdersService.Data.OrderContext;

namespace OrdersService.Endpoints
{
    public static class OrderEnpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            app.MapPost("/orders", async (Order order, OrderContext db) =>
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return Results.Created($"/orders/{order.Id}", order);
            });

            app.MapGet("/orders", async (OrderContext db) =>
            {
                return await db.Orders.ToListAsync();
            });

            app.MapGet("/orders/{id}", async (int id, OrderContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                return order != null ? Results.Ok(order) : Results.NotFound();
            });

            app.MapPut("/orders/{id}", async (int id, Order updatedOrder, OrderContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                if (order == null) return Results.NotFound();

                order.CustomerId = updatedOrder.CustomerId;
                order.TotalAmount = updatedOrder.TotalAmount;
                order.Status = updatedOrder.Status;
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            app.MapDelete("/orders/{id}", async (int id, OrderContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                if (order == null) return Results.NotFound();

                db.Orders.Remove(order);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
