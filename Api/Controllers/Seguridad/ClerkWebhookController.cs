using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers.Seguridad;

[ApiController]
[Route("api/webhooks/clerk")]
public class ClerkWebhookController(IUserSyncService userSyncService, ILogger<ClerkWebhookController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        
        try 
        {
            var jsonDoc = JsonDocument.Parse(json);
            var root = jsonDoc.RootElement;
            
            var type = root.GetProperty("type").GetString();
            var data = root.GetProperty("data");

            var clerkId = data.GetProperty("id").GetString();
            
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
                    
                    await userSyncService.SyncUserAsync(clerkId!, email, string.IsNullOrEmpty(fullName) ? email : fullName);
                    break;

                case "user.deleted":
                    await userSyncService.DeleteUserAsync(clerkId!);
                    break;
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Clerk webhook");
            return BadRequest(new { error = ex.Message });
        }
    }
}
