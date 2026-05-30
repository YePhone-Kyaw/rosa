"use client";

import { useProducts } from "../hooks/useProducts";
import { useCategories } from "../hooks/useCategories";
import Link from "next/link";
import ProductCard from "@/components/ProductCard";

export default function HomePage() {
  const { products, loading: productLoading } = useProducts(
    "sortBy=createdAt&order=desc",
  );
  const { categories, loading: categoryLoading } = useCategories();

  if (productLoading || categoryLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p className="text-gray-500">Loading...</p>
      </div>
    );
  }
  return (
    <div>
      <section className="bg-linear-to-r from-rose-600 to-rose-800 rounded-2xl p-12 text-white mb-12">
        <h1 className="text-4xl font-bold mb-4">Welcome to Rosa</h1>
        <p className="text-lg text-rose-100 mb-6">
          Discover amazing products at great prices
        </p>
        <Link
          href="/products"
          className="bg-white text-rose-600 px-6 py-3 rounded-lg font-medium hover:bg-rose-50 transition"
        >
          Shop Now
        </Link>
      </section>
      <section className="mb-12">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">Categories</h2>
        <div className="grid grid-cols-4 gap-4">
          {categories.map((category) => (
            <Link
              key={category.categoryId}
              href={`/products?categoryId=${category.categoryId}`}
              className="bg-white border rounded-xl p-6 text-center hover:shadow-md transition"
            >
              <h3 className="font-medium text-gray-900">
                {category.categoryName}
              </h3>
              <p className="text-sm text-gray-500 mt-1">
                {category.productCount} products
              </p>
            </Link>
          ))}
        </div>
      </section>
      <section>
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl font-bold text-gray-900">Latest Products</h2>
          <Link href="/products" className="text-rose-600 hover:underline">
            View All
          </Link>
        </div>
        <div className="grid grid-cols-4 gap-6">
          {products.map((product) => (
            <ProductCard key={product.productId} product={product} />
          ))}
        </div>
      </section>
    </div>
  );
}
