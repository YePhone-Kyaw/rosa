"use client";

import { useProducts } from "../hooks/useProducts";
import { useCategories } from "../hooks/useCategories";
import Link from "next/link";
import ProductCard from "@/components/ProductCard";
import Image from "next/image";
import HeroSlideshow from "@/components/HeroSlideshow";

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
      <HeroSlideshow />
      <section className="mb-12">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">Categories</h2>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {categories.map((category) => (
            <Link
              key={category.categoryId}
              href={`/products?categoryId=${category.categoryId}`}
              className="relative rounded-xl overflow-hidden h-20 flex flex-col items-center justify-center hover:scale-105 transition"
            >
              {category.categoryImageUrl ? (
                <Image
                  src={category.categoryImageUrl}
                  alt={category.categoryName}
                  fill
                  className="object-cover group-hover:scale:105 transition duration-300"
                />
              ) : (
                <div className="absolute inset-0 bg-gray-100" />
              )}
              <div className="absolute inset-0 bg-black/30" />
              <div className="relative text-center text-white z-10">
                <h3 className="font-medium text-lg ">
                  {category.categoryName}
                </h3>
                <p className="text-sm">{category.productCount} products</p>
              </div>
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
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {products.map((product) => (
            <ProductCard key={product.productId} product={product} />
          ))}
        </div>
      </section>
    </div>
  );
}
