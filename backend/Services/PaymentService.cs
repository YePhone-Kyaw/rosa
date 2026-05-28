using backend.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace backend.Services;

public class PaymentService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public PaymentService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<string> CreatePaymentIntent(int orderId)
    {
        var order =  await _db.Orders.FindAsync(orderId);
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
            }
        }
    }
}
