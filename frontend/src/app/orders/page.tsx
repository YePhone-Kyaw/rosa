"use client";

import ProtectedRoute from "@/components/ProtectedRoute";
import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import { Order } from "@/types/order";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import Link from "next/link";
import { useEffect, useRef, useState } from "react";

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const { user, setCart } = useStore();
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    async function loadOrders() {
      try {
        const response = await api.get("/orders");
        const data = response.data;
        setOrders(data.filter((order: Order) => order.status !== "pending"));
        const cartResponse = await api.get("/cart");
        setCart(cartResponse.data.cartItems);
      } catch (error) {
        console.error("Failed to fetch the orders", error);
      }
      setLoading(false);
    }
    loadOrders();
  }, [setCart]);

  useEffect(() => {
    if (!user) return;

    const connection = new HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_URL}/hubs/orders`)
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        connection.invoke("JoinUserGroup", user.userId.toString());
      })
      .catch((error) => console.error("SignalR connection failed", error));

    connection.on(
      "OrderStatusUpdated",
      (data: { orderId: number; status: string }) => {
        setOrders((prev) =>
          prev.map((order) =>
            order.orderId === data.orderId
              ? { ...order, status: data.status }
              : order,
          ),
        );
        setCart([]);
      },
    );
    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [user, setCart]);

  return (
    <ProtectedRoute>
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-8">My Orders</h1>

        {loading ? (
          <p className="text-center py-12 text-gray-500">Loading...</p>
        ) : orders.length === 0 ? (
          <div className="text-center py-12">
            <span className="text-6xl mb-4 block">📋</span>
            <p className="text-gray-500 mb-4">No orders yet</p>
            <Link
              href="/products"
              className="text-rose-600 hover:underline font-medium"
            >
              Start Shopping
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {orders.map((order) => (
              <div
                key={order.orderId}
                className="bg-white border rounded-xl p-6"
              >
                <div className="flex items-center justify-between mb-4">
                  <div>
                    <p className="text-sm text-gray-500">
                      Order #{order.orderId}
                    </p>
                    <p className="text-sm text-gray-400">
                      {new Date(order.createdAt).toLocaleDateString("en-US", {
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                      })}
                    </p>
                  </div>
                  <div className="flex items-center gap-4">
                    <span
                      className={`text-xs px-3 py-1 rounded-full font-medium ${
                        order.status === "completed"
                          ? "bg-green-100 text-green-700"
                          : order.status === "pending"
                            ? "bg-yellow-100 text-yellow-700"
                            : order.status === "cancelled"
                              ? "bg-red-100 text-red-700"
                              : "bg-gray-100 text-gray-700"
                      }`}
                    >
                      {order.status}
                    </span>
                    <p className="font-bold text-gray-900 text-lg">
                      ${order.totalAmount.toFixed(2)}
                    </p>
                  </div>
                </div>

                <div className="border-t pt-4 space-y-2">
                  {order.items.map((item) => (
                    <div
                      key={item.productId}
                      className="flex justify-between text-sm"
                    >
                      <span className="text-gray-600">
                        {item.productName} × {item.quantity}
                      </span>
                      <span className="text-gray-900">
                        ${item.subtotal.toFixed(2)}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
