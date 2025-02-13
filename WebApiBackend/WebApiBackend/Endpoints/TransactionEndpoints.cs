using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Database;
using WebApiBackend.Database.Domain;
using ParrotWings.Models.Dto.Transactions;
using ParrotWings.Models.Dto.Transactions.GetTransactions;
using ParrotWings.Models.Dto.Users;

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
            routes.MapGet("/api/transactions",
                    async Task<Results<Ok<GetTransactionsResponseDto>, UnauthorizedHttpResult>> (
                        HttpContext http,
                        UserManager<AppUser> userManager,
                        PwDbContext db,
                        ILogger<Endpoints> logger,
                        [FromQuery] string? filter,
                        [FromQuery] int startIndex,
                        [FromQuery] int count,
                        [FromQuery] string sortBy = "name",
                        [FromQuery] string sortOrder = "asc"
                    ) =>
                    {
                        logger.LogInformation("GET /api/transactions invoked.");

                        // Получаем email пользователя из данных аутентификации.
                        var email = http.User.FindFirst(ClaimTypes.Email)?.Value;
                        if (string.IsNullOrEmpty(email))
                        {
                            logger.LogWarning("Unauthorized access: No user identity found.");
                            return TypedResults.Unauthorized();
                        }

                        logger.LogInformation("Retrieving transactions for user with email: {UserEmail}", email);

                        var user = await userManager.FindByEmailAsync(email);
                        if (user == null)
                        {
                            logger.LogWarning("User not found for email: {UserEmail}", email);
                            return TypedResults.Unauthorized();
                        }

                        logger.LogInformation("User found. UserId: {UserId}", user.Id);

                        // Выбираем транзакции, где пользователь является отправителем или получателем.
                        var query = db.Transactions.AsQueryable()
                            .Where(t => t.Sender.Id == user.Id || t.Recipient.Id == user.Id);

                        // Применяем фильтрацию, если параметр filter передан.
                        if (!string.IsNullOrEmpty(filter))
                        {
                            logger.LogInformation("Applying filter: {Filter}", filter);

                            // Если фильтр можно преобразовать в число, фильтруем по сумме транзакции.
                            if (decimal.TryParse(filter, out var amount))
                            {
                                query = query.Where(t => t.Amount == amount);
                            }

                            // Фильтруем по email отправителя или получателя.
                            query = query.Where(t =>
                                t.Sender.Email!.Contains(filter) ||
                                t.Recipient.Email!.Contains(filter));
                        }

                        // Логика сортировки.
                        query = sortBy.ToLower() switch
                        {
                            "amount" => sortOrder.ToLower() == "asc"
                                ? query.OrderBy(t => t.Amount)
                                : query.OrderByDescending(t => t.Amount),
                            _ => sortOrder.ToLower() == "asc"
                                ? query.OrderBy(t => t.TransactionDate)
                                : query.OrderByDescending(t => t.TransactionDate)
                        };

                        // Применяем пагинацию.
                        logger.LogInformation("Applying pagination: startIndex = {StartIndex}, count = {Count}",
                            startIndex, count);
                        var totalCount = query.Count();
                        var transactions = await query.Skip(startIndex).Take(count).ToListAsync();
                        logger.LogInformation("Returning {Count} transactions.", transactions.Count);

                        return TypedResults.Ok(new GetTransactionsResponseDto
                        {
                            Transactions = transactions.Select(t => new TransactionDto
                            {
                                Id = t.Id,
                                Amount = t.Amount,
                                Recipient = new UserDto
                                {
                                    Id = t.Recipient.Id,
                                    Email = t.Recipient.Email!,
                                    FullName = t.Recipient.FullName
                                },
                                Sender = new UserDto
                                {
                                    Id = t.Sender.Id,
                                    Email = t.Sender.Email!,
                                    FullName = t.Sender.FullName
                                },
                                TransactionDate = t.TransactionDate
                            }),
                            TotalCount = totalCount
                        });
                    })
                .RequireAuthorization();


            // POST /api/transaction – creates a new transaction (transfer).
            routes.MapPost(
                    "/api/transaction",
                    async Task<Results<Ok<string>, BadRequest<string>, UnauthorizedHttpResult, StatusCodeHttpResult>>
                    (
                        HttpContext http,
                        CreateTransactionRequestDto model,
                        UserManager<AppUser> userManager,
                        PwDbContext db,
                        ILogger<Endpoints> logger
                    ) =>
                    {
                        logger.LogInformation("POST /api/transaction invoked.");
                        var email = http.User.FindFirst(ClaimTypes.Email)?.Value;
                        if (string.IsNullOrEmpty(email))
                        {
                            logger.LogWarning("Unauthorized access: No user identity found for transaction creation.");
                            return TypedResults.Unauthorized();
                        }

                        var sender = await userManager.FindByEmailAsync(email);
                        if (sender == null)
                        {
                            logger.LogWarning("User not found for transaction creation with email: {UserEmail}",
                                email);
                            return TypedResults.Unauthorized();
                        }

                        // Check if the sender has sufficient balance.
                        if (sender.Balance < model.Amount)
                        {
                            logger.LogWarning(
                                "Insufficient funds for user {UserEmail}. Available balance: {Balance}, Requested: {Amount}",
                                email, sender.Balance, model.Amount);
                            return TypedResults.BadRequest("Insufficient funds.");
                        }

                        // Find the recipient by email.
                        var recipient = await userManager.FindByIdAsync(model.RecipientId);
                        if (recipient == null)
                        {
                            logger.LogWarning("Recipient not found: {RecipientId}", model.RecipientId);
                            return TypedResults.BadRequest("Recipient not found.");
                        }

                        logger.LogInformation(
                            "Initiating transaction from {SenderEmail} to {RecipientEmail} for amount {Amount}",
                            sender.Email, recipient.Email, model.Amount);

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
                                Sender = sender,
                                Recipient = recipient,
                                Amount = model.Amount,
                                TransactionDate = DateTime.UtcNow
                            };
                            db.Transactions.Add(transactionRecord);

                            await db.SaveChangesAsync();
                            await transaction.CommitAsync();

                            logger.LogInformation("Transaction completed successfully. TransactionId: {TransactionId}",
                                transactionRecord.Id);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error occurred during transaction processing.");
                            await transaction.RollbackAsync();
                            return TypedResults.StatusCode(500);
                        }

                        return TypedResults.Ok("Transaction completed successfully.");
                    })
                .RequireAuthorization();

            return routes;
        }
    }
}