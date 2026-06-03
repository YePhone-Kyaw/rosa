using backend.Data;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace backend.Services;

public class PaymentService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly IHubContext<OrderHub> _hubContext;
    public PaymentService(AppDbContext db, IConfiguration config, IHubContext<OrderHub> hubContext)
    {
        _db = db;
        _config = config;
        _hubContext = hubContext;
    }

    public async Task<string> CreatePaymentIntent(int orderId)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order == null) throw new Exception("Order not found");

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(order.TotalAmount * 100),
            Currency = "cad",
            Metadata = new Dictionary<string, string>
            {
                { "orderId", orderId.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        // save the payment record
        var payment = new backend.Models.Payment
        {
            OrderId = orderId,
            PaymentIntentId = paymentIntent.Id,
            Status = "pending",
            Amount = order.TotalAmount
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        return paymentIntent.ClientSecret;
    }
    public async Task HandleWebhook(string payload, string stripeSignature)
    {
        var webhookSecret = _config["Stripe:WebhookSecret"];
        var stripeEvent = Stripe.EventUtility.ConstructEvent(
            payload, stripeSignature, webhookSecret);

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;

            // Check if orderId exists in metadata
            if (!paymentIntent!.Metadata.ContainsKey("orderId"))
                return;

            var orderId = int.Parse(paymentIntent!.Metadata["orderId"]);

            // Update payment status
            var payment = await _db.Payments
                .FirstOrDefaultAsync((payment) => payment.PaymentIntentId == paymentIntent.Id);
            if (payment != null)
            {
                payment.Status = "succeeded";
                await _db.SaveChangesAsync();
            }

            // Update order status
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = "paid";
                await _db.SaveChangesAsync();

                var cart = await _db.Carts
                    .Include((cart) => cart.CartItems)
                    .FirstOrDefaultAsync((cart) => cart.UserId == order.UserId);

                if (cart != null)
                {
                    _db.CartItems.RemoveRange(cart.CartItems);
                    await _db.SaveChangesAsync();
                }

                await _hubContext.Clients.Group($"user_{order.UserId}")
                    .SendAsync("OrderStatusUpdated", new
                    {
                        OrderId = orderId,
                        Status = "paid"
                    });
            }
        }
    }
}
