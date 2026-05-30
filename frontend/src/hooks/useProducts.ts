import api from "@/lib/api";
import { Product } from "@/types/product";
import { useEffect, useState } from "react";

export function useProducts(params?: string) {
  const [products, setProducts] = useState<Product[]>([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchProducts() {
      setProducts([]);
      setPage(1);
      setHasMore(true);
      setLoading(true);
      try {
        const response = await api.get(
          `/products${params ? `?${params}` : ""}`,
        );
        setProducts(response.data.products || []);
        setPage(response.data.page || 1);
        setHasMore(response.data.hasMore || false);
      } catch (error) {
        console.error("Failed to fetch the products", error);
      }
      setLoading(false);
    }
    fetchProducts();
  }, [params]);

  return { products, page, hasMore, loading };
}
