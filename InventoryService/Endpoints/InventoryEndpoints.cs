using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using static Inventory.Data.InventoryContext;

namespace InventoryService.Endpoints
{
    public static class InventoryEndpoints
    {
        public static void MapInventoryEndpoints(this WebApplication app)
        {
            app.MapPost("/products", async (Product product, InventoryContext db) =>
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/products/{product.Id}", product);
            });

            app.MapGet("/products", async (InventoryContext db) =>
            {
                return await db.Products.ToListAsync();
            });

            app.MapGet("/products/{id}", async (int id, InventoryContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                return product != null ? Results.Ok(product) : Results.NotFound();
            });

            app.MapPut("/products/{id}", async (int id, Product updatedProduct, InventoryContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product == null) return Results.NotFound();

                product.Name = updatedProduct.Name;
                product.Stock = updatedProduct.Stock;
                product.Price = updatedProduct.Price;
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            app.MapDelete("/products/{id}", async (int id, InventoryContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product == null) return Results.NotFound();

                db.Products.Remove(product);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
