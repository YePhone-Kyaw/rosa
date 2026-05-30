"use client";

import ProductCard from "@/components/ProductCard";
import { useCategories } from "@/hooks/useCategories";
import { useProducts } from "@/hooks/useProducts";
import { useRouter, useSearchParams } from "next/navigation";

export default function ProductsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const search = searchParams.get("search") || "";
  const categoryId = searchParams.get("categoryId") || "";
  const sortBy = searchParams.get("sortBy") || "";
  const order = searchParams.get("order") || "";
  const page = Number(searchParams.get("page")) || 1;

  // Build query params
  const params = new URLSearchParams();
  if (search) params.append("search", search);
  if (categoryId) params.append("categoryId", categoryId);
  if (sortBy) params.append("sortBy", sortBy);
  if (order) params.append("order", order);
  params.append("page", page.toString());
  params.append("pageSize", "12");

  const { categories } = useCategories();
  const { products, hasMore, loading } = useProducts(params.toString());

  const handleCategoryChange = (value: string) => {
    const params = new URLSearchParams(searchParams.toString());
    if (value) {
      params.set("categoryId", value);
    } else {
      params.delete("categoryId");
    }
    params.set("page", "1");
    router.push(`/products?${params.toString()}`);
  };

  const handleSortChange = (value: string) => {
    const params = new URLSearchParams(searchParams.toString());
    if (value === "price-asc") {
      params.set("sortBy", "price");
      params.set("order", "asc");
    } else if (value === "price-desc") {
      params.set("sortBy", "price");
      params.set("order", "desc");
    } else if (value === "name-asc") {
      params.set("sortBy", "price");
      params.set("order", "asc");
    } else {
        params.delete('sortBy');
        params.delete('order');
    }
    params.set('page', '1');
    router.replace(`/products?${params.toString()}`);
  };

  const loadMore = () => {

  }

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-900 mb-8">All Products</h1>

      <div>
        <select
          value={`${sortBy}-${order}`}
          onChange={(e) => handleSortChange(e.target.value)}
        >
          <option value="">Latest</option>
          <option value="price-asc">Price: Low to High</option>
          <option value="price-desc">Price: High to Low</option>
          <option value="name-asc">Name: A-Z</option>
        </select>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {products.map((product) => (
            <ProductCard key={product.productId} product={product} />
          ))}
        </div>
      )}

      {hasMore ? (
        <div className="flex justify-center mt-12">
        <button onClick={loadMore}
            className="bg-rose-600 text-white px-8 py-3 rounded-lg hover:bg-rose-700 transition font-medium">
            Show More Products
        </button>
    </div>
      ) : (<></>)}
    </div>
  );
}
