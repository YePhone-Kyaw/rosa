"use client";

import ProtectedRoute from "@/components/ProtectedRoute";
import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import { Cart } from "@/types/cart";
import Link from "next/link";
import { useEffect, useState } from "react";

export default function CartPage() {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);

  const { setCart: updateCartStore } = useStore();

  const fetchCart = async () => {
    try {
      const response = await api.get("/cart");
      updateCartStore(response.data.cartItems || []);
      return response.data;
    } catch (error) {
      console.error("Failed to fetch the cart", error);
    }
  };

  useEffect(() => {
    async function loadCart() {
      const data = await fetchCart();
      setCart(data);
      setLoading(false);
    }
    loadCart();
  }, []);

  const updateQuantity = async (cartItemId: number, quantity: number) => {
    try {
      await api.put(`/cart/${cartItemId}`, { quantity });
      const data = await fetchCart();
      setCart(data);
    } catch (error) {
      console.error("Failed to update the quantity", error);
    }
  };

  const removeItem = async (cartItemId: number) => {
    try {
      await api.delete(`/cart/${cartItemId}`);
      const data = await fetchCart();
      setCart(data);
    } catch (error) {
      console.error("Failed to remove item", error);
    }
  };

  const clearCart = async () => {
    try {
      await api.delete("/cart");
      const data = await fetchCart();
      setCart(data);
    } catch (error) {
      console.error("Failed to clear cart", error);
    }
  };

  return (
    <ProtectedRoute>
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-8">Shopping Cart</h1>
        {loading ? (
          <p className="text-center py-12 text-gray-500">Loading...</p>
        ) : !cart || cart.cartItems.length === 0 ? (
          <div className="text-center py-12">
            <span className="text-6xl mb-4 block">🛒</span>
            <p className="text-gray-500 mb-4">Your cart is empty</p>
            <Link
              href="/products"
              className="text-rose-600 hover:underline font-medium"
            >
              Continue Shopping
            </Link>
          </div>
        ) : (
          <div className="flex gap-8">
            <div className="flex-1 space-y-4">
              {cart.cartItems.map((item) => (
                <div
                  key={item.cartItemId}
                  className="bg-white border rounded-xl p-6 flex items-center gap-6"
                >
                  <div className="w-20 h-20 bg-gray-100 rounded-lg flex items-center justify-center">
                    <span className="text-3xl">📦</span>
                  </div>

                  <div className="flex-1">
                    <h3 className="font-medium text-gray-900">
                      {item.productName}
                    </h3>
                    <p className="text-gray-500 text-sm">
                      ${item.unitPrice.toFixed(2)} each
                    </p>
                  </div>

                  <div className="flex items-center border rounded-lg">
                    <button
                      onClick={() =>
                        updateQuantity(item.cartItemId, item.quantity - 1)
                      }
                      className="px-3 py-1 text-gray-600 hover:bg-gray-100"
                    >
                      −
                    </button>
                    <span className="px-3 py-1 font-medium text-gray-900">
                      {item.quantity}
                    </span>
                    <button
                      onClick={() =>
                        updateQuantity(item.cartItemId, item.quantity + 1)
                      }
                      className="px-3 py-1 text-gray-600 hover:bg-gray-100"
                    >
                      +
                    </button>
                  </div>

                  <p className="font-bold text-gray-900 w-24 text-right">
                    ${item.subtotal.toFixed(2)}
                  </p>

                  <button
                    onClick={() => removeItem(item.cartItemId)}
                    className="text-red-500 hover:text-red-700 text-sm"
                  >
                    Remove
                  </button>
                </div>
              ))}

              <button
                onClick={clearCart}
                className="text-sm text-gray-500 hover:text-red-500"
              >
                Clear Cart
              </button>
            </div>

            <div className="w-80 shrink-0">
              <div className="bg-white border rounded-xl p-6 sticky top-32">
                <h2 className="text-lg font-bold text-gray-900 mb-4">
                  Order Summary
                </h2>
                <div className="flex justify-between mb-2">
                  <span className="text-gray-600">
                    Items ({cart.cartItems.length})
                  </span>
                  <span className="text-gray-900">
                    ${cart.totalAmount.toFixed(2)}
                  </span>
                </div>
                <div className="border-t my-4"></div>
                <div className="flex justify-between mb-6">
                  <span className="font-bold text-gray-900">Total</span>
                  <span className="font-bold text-gray-900 text-xl">
                    ${cart.totalAmount.toFixed(2)}
                  </span>
                </div>
                <Link
                  href="/checkout"
                  className="block w-full bg-rose-600 text-white text-center py-3 rounded-lg font-medium hover:bg-rose-700 transition"
                >
                  Proceed to Checkout
                </Link>
              </div>
            </div>
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
