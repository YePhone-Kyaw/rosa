"use client";

import PaymentForm from "@/components/PaymentForm";
import ProtectedRoute from "@/components/ProtectedRoute";
import { useCart } from "@/hooks/useCart";
import api from "@/lib/api";
import { Elements } from "@stripe/react-stripe-js";
import { loadStripe, Stripe } from "@stripe/stripe-js";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function CheckoutPage() {
  const { cart, loading, loadCart } = useCart();
  const router = useRouter();

  const [placing, setPlacing] = useState(false);
  const [error, setError] = useState("");

  const [stripePromise, setStripePromise] = useState<Stripe | null>(null);
  const [clientSecret, setClientSecret] = useState("");
  const [orderId, setOrderId] = useState<number | null>(null);

  useEffect(() => {
    loadCart();
  }, [loadCart]);

  useEffect(() => {
    if (!loading && (!cart || cart.cartItems.length === 0) && !clientSecret) {
      router.push("/cart");
    }
  }, [loading, router, cart, clientSecret]);

  const handlePlaceOrder = async () => {
    if (!cart || cart.cartItems.length === 0) return;
    setPlacing(true);
    setError("");

    try {
      // Create order
      const orderResponse = await api.post("/orders", {
        items: cart.cartItems.map((cartItem) => ({
          productId: cartItem.productId,
          quantity: cartItem.quantity,
        })),
      });
      setOrderId(orderResponse.data.orderId);

      // Create payment intent
      const paymentResponse = await api.post(
        `/payment/${orderResponse.data.orderId}`,
      );
      setClientSecret(paymentResponse.data.clientSecret);

      // Load stripe
      const stripe = await loadStripe(paymentResponse.data.publishableKey);
      setStripePromise(stripe);
    } catch (error) {
      console.error("Failed to place order", error);
      setError("Failed to place order. Please try again.");
    }
    setPlacing(false);
  };

  return (
    <ProtectedRoute>
      <div className="max-w-2xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-900 mb-8">Checkout</h1>

        {loading ? (
          <p className="text-center py-12 text-gray-500">Loading...</p>
        ) : clientSecret && stripePromise && orderId ? (
          <div className="bg-white border rounded-xl p-6">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-lg font-bold text-gray-900">Payment</h2>
              <button
                onClick={() => router.push("/cart")}
                className="text-sm text-gray-500 hover:text-rose-600"
              >
                ← Back to Cart
              </button>
            </div>
            <Elements stripe={stripePromise} options={{ clientSecret }}>
              <PaymentForm />
            </Elements>
          </div>
        ) : (
          <div>
            {!cart || cart.cartItems.length === 0 ? (
              <p className="text-center py-12 text-gray-500">
                Your cart is empty
              </p>
            ) : (
              <div className="space-y-6">
                <div className="bg-white border rounded-xl p-6">
                  <h2 className="text-lg font-bold text-gray-900 mb-4">
                    Order Summary
                  </h2>
                  {cart.cartItems.map((item) => (
                    <div
                      key={item.cartItemId}
                      className="flex justify-between text-sm py-2"
                    >
                      <span className="text-gray-600">
                        {item.productName} x {item.quantity}
                      </span>
                      <span className="text-gray-900 font-medium">
                        ${item.subtotal.toFixed(2)}
                      </span>
                    </div>
                  ))}
                  <div className="border-t pt-3 mt-3 flex justify-between">
                    <span className="font-bold text-gray-900">Total</span>
                    <span className="font-bold text-gray-900 text-xl">
                      ${cart.totalAmount.toFixed(2)}
                    </span>
                  </div>
                </div>

                {error && (
                  <p className="text-red-500 text-sm bg-red-50 px-4 py-3 rounded-lg">
                    {error}
                  </p>
                )}

                <button
                  onClick={handlePlaceOrder}
                  disabled={placing}
                  className="w-full bg-rose-600 text-white py-3 rounded-lg font-medium hover:bg-rose-700 transition disabled:opacity-50"
                >
                  {placing ? "Placing Order..." : "Place Order & Pay"}
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
