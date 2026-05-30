import api from "@/lib/api";
import { Product } from "@/types/product";
import { useCallback, useEffect, useRef, useState } from "react";

export function useProducts(params?: string) {
  const [products, setProducts] = useState<Product[]>([]);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(true);
  const pageRef = useRef(1);

  useEffect(() => {
    pageRef.current = 1;
    async function fetchProducts() {
      setLoading(true);
      try {
        const query = params ? `${params}&` : "";
        const response = await api.get(`/products?${query}page=1&pageSize=28`);
        setProducts(response.data.products);
        setHasMore(1 < response.data.totalPages);
      } catch (error) {
        console.error("Failed to fetch the products", error);
      }
      setLoading(false);
    }
    fetchProducts();
  }, [params]);

  const loadMore = useCallback(async () => {
    const nextPage = pageRef.current + 1;
    setLoading(true);
    try {
      const query = params ? `${params}&` : "";
      const response = await api.get(
        `/products?${query}page=${nextPage}&pageSize=28`,
      );
      setProducts((prev) => [...prev, ...response.data.products]);
      setHasMore(nextPage < response.data.totalPages);
      pageRef.current = nextPage;
    } catch (error) {
      console.error("Failed to load more products", error);
    }
    setLoading(false);
  }, [params]);

  return { products, hasMore, loading, loadMore };
}
