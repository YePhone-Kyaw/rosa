// src/hooks/useCart.ts
import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import { Cart } from "@/types/cart";
import { useState } from "react";

export function useCart() {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);
  const { setCart: updateStore } = useStore();

  const fetchCart = async () => {
    try {
      const response = await api.get("/cart");
      setCart(response.data);
      updateStore(response.data?.cartItems || []);
      return response.data;
    } catch (error) {
      console.error("Failed to fetch cart", error);
      return null;
    }
  };

  const loadCart = async () => {
    await fetchCart();
    setLoading(false);
  };

  const updateQuantity = async (cartItemId: number, quantity: number) => {
    await api.put(`/cart/${cartItemId}`, { quantity });
    await fetchCart();
  };

  const removeItem = async (cartItemId: number) => {
    await api.delete(`/cart/${cartItemId}`);
    await fetchCart();
  };

  const clearCart = async () => {
    await api.delete("/cart");
    await fetchCart();
  };

  return {
    cart,
    loading,
    loadCart,
    fetchCart,
    updateQuantity,
    removeItem,
    clearCart,
  };
}
