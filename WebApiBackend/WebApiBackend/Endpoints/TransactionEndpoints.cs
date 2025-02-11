using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParrotWings.Models.Dto.Transactions;
using ParrotWings.Models.Dto.Transactions.CreateTransaction;
using WebApiBackend.Database;
using WebApiBackend.Database.Domain;

namespace WebApiBackend.Endpoints
{
    /// <summary>
    /// Contains endpoints for handling transactions.
    /// </summary>
    public static class TransactionEndpoints
    {
        public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder routes)
        {
            // GET /api/transactions – retrieves the list of transactions for the current user with pagination, sorting, and optional filtering.
            routes.MapGet("/api/transactions", async (HttpContext http, UserManager<AppUser> userManager, PwDbContext db, int startIndex = 0, int count = 10, string? sortBy = "date", string? sortOrder = "desc", string? filter = null) =>
            {
                // Retrieve the current user's email from the authentication cookies.
                var userEmail = http.User.Identity?.Name;
                if (userEmail == null)
                {
                    return Results.Unauthorized();
                }

                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                // Select transactions where the user is either the sender or the recipient.
                var query = db.Transactions.AsQueryable()
                    .Where(t => t.SenderId == user.Id || t.RecipientId == user.Id);

                // If a filter is provided, additional filtering logic can be applied.
                if (!string.IsNullOrEmpty(filter))
                {
                    // TODO: Implement filtering based on required parameters.
                }

                // Sort by the specified parameter (default is date).
                query = sortBy?.ToLower() switch
                {
                    "amount" => sortOrder?.ToLower() == "asc" ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount),
                    _ => sortOrder?.ToLower() == "asc" ? query.OrderBy(t => t.TransactionDate) : query.OrderByDescending(t => t.TransactionDate)
                };

                // Apply pagination.
                var transactions = await query.Skip(startIndex).Take(count).ToListAsync();

                return Results.Ok(transactions);
            })
            .RequireAuthorization();

            // POST /api/transactions – creates a new transaction (transfer).
            routes.MapPost("/api/transaction", async (HttpContext http, CreateTransactionRequestDto model, UserManager<AppUser> userManager, PwDbContext db) =>
            {
                // Retrieve the current user (sender) based on the authentication cookie.
                var userEmail = http.User.Identity?.Name;
                if (userEmail == null)
                {
                    return Results.Unauthorized();
                }

                var sender = await userManager.FindByEmailAsync(userEmail);
                if (sender == null)
                {
                    return Results.Unauthorized();
                }

                // Check if the sender has sufficient balance.
                if (sender.Balance < model.Amount)
                {
                    return Results.BadRequest("Insufficient funds.");
                }

                // Find the recipient by email.
                var recipient = await userManager.FindByEmailAsync(model.RecipientEmail);
                if (recipient == null)
                {
                    return Results.BadRequest("Recipient not found.");
                }

                // Begin a database transaction.
                await using var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    // Deduct the amount from the sender and add it to the recipient.
                    sender.Balance -= model.Amount;
                    recipient.Balance += model.Amount;

                    await userManager.UpdateAsync(sender);
                    await userManager.UpdateAsync(recipient);

                    // Create a transaction record.
                    var transactionRecord = new Transaction
                    {
                        SenderId = sender.Id,
                        RecipientId = recipient.Id,
                        Amount = model.Amount,
                        TransactionDate = DateTime.UtcNow
                    };
                    db.Transactions.Add(transactionRecord);

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return Results.StatusCode(500);
                }

                return Results.Ok(new { message = "Transaction completed successfully." });
            })
            .RequireAuthorization();

            return routes;
        }
    }
}