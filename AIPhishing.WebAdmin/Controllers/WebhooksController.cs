using AIPhishing.Business.Attacks;
using AIPhishing.Business.Attacks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class WebhooksController(IAttackBusiness attackBusiness)
    : ControllerBase
{
    // [HttpPost("opened")]
    // public async ValueTask<IActionResult> Opened([FromBody] MailerSendWebhookModel request)
    // {
    //     if (request.Data.Type is "opened" or "clicked")
    //     {
    //         if (request.Data.Email.Tags.Length > 0)
    //         {
    //             if (Guid.TryParse(request.Data.Email.Tags[0], out var emailId))
    //             {
    //                 if (request.Data.Type == "opened")
    //                     await attackBusiness.EmailOpenedAsync(emailId);
    //                 else if (request.Data.Type == "clicked")
    //                     await attackBusiness.EmailClickedAsync(emailId);
    //             }
    //         }
    //     }
    //
    //     return Ok();
    // }
    
    [HttpGet("clicked/{emailId:guid}")]
    public async Task<IActionResult> Clicked([FromRoute] Guid emailId)
    {
        await attackBusiness.EmailClickedAsync(emailId);

        return Content("<html><body><h1>Congratulations.. You have been hacked..</h1> <p>But don't worry, this was harmless and be careful for the next time.</p></body></html>", "text/html");
    }

    [HttpPost("replied")]
    public async Task<IActionResult> Replied([FromBody] AttackEmailRepliedModel model)
    {
        await attackBusiness.EmailReplied(model);

        return Ok();
    }
}