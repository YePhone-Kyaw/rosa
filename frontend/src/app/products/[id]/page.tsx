"use client";

import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import { Product } from "@/types/product";
import { useRouter } from "next/navigation";
import { use, useEffect, useState } from "react";

export default function ProductDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { user } = useStore();
  const router = useRouter();

  const [product, setProduct] = useState<Product | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const response = await api.get(`/products/${id}`);
        const data = response.data;
        setProduct(data);
      } catch (error) {
        console.error("Failed to fetch the product", error);
      }
      setLoading(false);
    };
    fetchProduct();
  }, [id]);

  const handleAddToCart = async () => {
    if (!user) {
      router.push("/auth/login");
      return;
    }
    try {
      await api.post("/cart", {
        productId: product?.productId,
        quantity: quantity,
      });
      router.push("/cart");
    } catch (error) {
      console.error("Failed to add to cart", error);
    }
  };

  const handleBuyNow = async () => {
    if (!user) {
      router.push("/auth/login");
      return;
    }
    try {
      await api.post("/cart", {
        productId: product?.productId,
        quantity: quantity,
      });
      router.push("/checkout");
    } catch (error) {
      console.error("Failed to buy now", error);
    }
  };

  return (
    <div>
      {loading ? (
        <p className="text-center py-12 text-gray-500">Loading...</p>
      ) : !product ? (
        <p className="text-center py-12 text-gray-500">Product not found</p>
      ) : (
        <div className="flex gap-12">
          <div className="w-1/2 aspect-square bg-gray-100 rounded-xl flex items-center justify-center">
            {product.imageUrl ? (
              <img
                src={product.imageUrl}
                alt={product.productName}
                className="w-full h-full object-cover rounded-xl"
              />
            ) : (
              <span className="text-8xl">📦</span>
            )}
          </div>

          <div className="w-1/2">
            <p className="text-sm text-rose-600 mb-2">{product.categoryName}</p>
            <h1 className="text-3xl font-bold text-gray-900 mb-4">
              {product.productName}
            </h1>
            <p className="text-3xl font-bold text-gray-900 mb-6">
              ${product.price.toFixed(2)}
            </p>
            <h3 className="text-gray-600 font-bold">About this item</h3>
            <p className="text-gray-600 mb-8">{product.description}</p>

            {product.stock > 0 ? (
              <p className="text-green-600 text-sm mb-4">
                In stock ({product.stock} available)
              </p>
            ) : (
              <p className="text-red-500 text-sm mb-4">Out of stock</p>
            )}

            {product.stock > 0 && (
              <div className="flex items-center gap-4 mb-6">
                <span className="text-gray-700 font-medium">Quantity:</span>
                <div className="flex items-center border rounded-lg">
                  <button
                    onClick={() => setQuantity((prev) => Math.max(1, prev - 1))}
                    disabled={quantity <= 1}
                    className="px-4 py-2 text-gray-600 hover:bg-gray-100 disabled:opacity-50"
                  >
                    −
                  </button>
                  <span className="px-4 py-2 font-medium text-gray-900 min-w-12 text-center">
                    {quantity}
                  </span>
                  <button
                    onClick={() =>
                      setQuantity((prev) => Math.min(product.stock, prev + 1))
                    }
                    disabled={quantity >= product.stock}
                    className="px-4 py-2 text-gray-600 hover:bg-gray-100 disabled:opacity-50"
                  >
                    +
                  </button>
                </div>
                <span className="text-sm text-gray-400">
                  (max {product.stock})
                </span>
              </div>
            )}

            <div className="flex gap-4">
              <button
                onClick={handleAddToCart}
                disabled={product.stock === 0}
                className="flex-1 bg-white text-rose-600 border-2 border-rose-600 py-3 rounded-lg font-medium hover:bg-rose-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {product.stock > 0 ? "Add to Cart" : "Out of Stock"}
              </button>
              <button
                onClick={handleBuyNow}
                disabled={product.stock === 0}
                className="flex-1 bg-rose-600 text-white py-3 rounded-lg font-medium hover:bg-rose-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Buy Now
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
