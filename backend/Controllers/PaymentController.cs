using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("payment")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly IConfiguration _config;
    public PaymentController(PaymentService paymentService, IConfiguration config)
    {
        _paymentService = paymentService;
        _config = config;
    }

    [HttpPost("{orderId}")]
    [Authorize]
    public async Task<IActionResult> CreatePaymentIntent(int orderId)
    {
        var clientSecret = await _paymentService.CreatePaymentIntent(orderId);

        return Ok(new PaymentIntentResponseDto
        {
            ClientSecret = clientSecret,
            PublishableKey = _config["Stripe:PublishableKey"]!
        });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];

        try
        {
            await _paymentService.HandleWebhook(payload, stripeSignature!);
            return Ok();
        } 
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });    
        }
    }
}
