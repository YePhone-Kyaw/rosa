"use client";

import FilterBar from "@/components/FilterBar";
import ProductCard from "@/components/ProductCard";
import { useProducts } from "@/hooks/useProducts";
import { useSearchParams } from "next/navigation";

export default function ProductsPage() {
  const searchParams = useSearchParams();

  const search = searchParams.get("search") || "";
  const categoryId = searchParams.get("categoryId") || "";
  const sortBy = searchParams.get("sortBy") || "";
  const order = searchParams.get("order") || "";

  // Build query params
  const params = new URLSearchParams();
  if (search) params.append("search", search);
  if (categoryId) params.append("categoryId", categoryId);
  if (sortBy) params.append("sortBy", sortBy);
  if (order) params.append("order", order);

  const { products, hasMore, loading, loadMore } = useProducts(
    params.toString(),
  );

  return (
    <div className="flex gap-8">
      <FilterBar />
      <div className="flex-1">
        <h1 className="text-2xl font-bold text-gray-900 mb-8">
          {search ? `Results for ${search}` : "All Products"}
        </h1>
        {loading && products.length === 0 ? (
          <p className="text-center py-12 text-gray-500">Loading...</p>
        ) : products.length === 0 ? (
          <p className="text-center py-12 text-gray-500">No products found</p>
        ) : (
          <div className="grid grid-cols-4 gap-6">
            {products.map((product) => (
              <ProductCard key={product.productId} product={product} />
            ))}
          </div>
        )}
        {hasMore && !loading && (
          <div className="flex justify-center mt-12">
            <button
              onClick={loadMore}
              className="bg-rose-600 text-white px-8 py-3 rounded-lg hover:bg-rose-700 transition font-medium"
            >
              Show More Products
            </button>
          </div>
        )}
        {loading && products.length > 0 && (
          <p className="text-center py-6 text-gray-500">Loading more...</p>
        )}
      </div>
    </div>
  );
}
