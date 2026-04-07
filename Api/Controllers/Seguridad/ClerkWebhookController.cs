using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers.Seguridad;

[ApiController]
[Route("api/webhooks/clerk")]
[AllowAnonymous]
public class ClerkWebhookController(IUserSyncService userSyncService, ILogger<ClerkWebhookController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var json = await reader.ReadToEndAsync();

        logger.LogInformation("Clerk webhook received. Body length: {Length}", json.Length);

        if (string.IsNullOrWhiteSpace(json))
        {
            logger.LogWarning("Clerk webhook received empty body");
            return BadRequest(new { error = "Empty body" });
        }

        try
        {
            var jsonDoc = JsonDocument.Parse(json);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("type", out var typeProp) || !root.TryGetProperty("data", out var data))
            {
                logger.LogWarning("Clerk webhook missing 'type' or 'data' fields. Body: {Body}", json);
                return BadRequest(new { error = "Invalid webhook payload" });
            }

            var type = typeProp.GetString();

            if (!data.TryGetProperty("id", out var idProp))
            {
                logger.LogWarning("Clerk webhook 'data' missing 'id'. Type: {Type}", type);
                return BadRequest(new { error = "Missing user id in data" });
            }

            var clerkId = idProp.GetString();

            if (string.IsNullOrEmpty(clerkId))
            {
                logger.LogWarning("Clerk webhook 'id' is null or empty. Type: {Type}", type);
                return Ok(); // Clerk a veces envía id null para objetos ya eliminados
            }

            logger.LogInformation("Processing Clerk webhook event: {Type} for User: {ClerkId}", type, clerkId);

            switch (type)
            {
                case "user.created":
                case "user.updated":
                    var emailAddresses = data.GetProperty("email_addresses");
                    var email = emailAddresses[0].GetProperty("email_address").GetString() ?? "";

                    var firstName = data.GetProperty("first_name").GetString() ?? "";
                    var lastName = data.GetProperty("last_name").GetString() ?? "";
                    var fullName = $"{firstName} {lastName}".Trim();

                    await userSyncService.SyncUserAsync(clerkId, email, string.IsNullOrEmpty(fullName) ? email : fullName);
                    break;

                case "user.deleted":
                    logger.LogInformation("Deleting user from DB for ClerkId: {ClerkId}", clerkId);
                    await userSyncService.DeleteUserAsync(clerkId);
                    logger.LogInformation("User deleted successfully for ClerkId: {ClerkId}", clerkId);
                    break;

                default:
                    logger.LogInformation("Unhandled Clerk webhook event type: {Type}", type);
                    break;
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Clerk webhook. Body: {Body}", json);
            return StatusCode(500, new { error = "Internal error processing webhook" });
        }
    }
}
