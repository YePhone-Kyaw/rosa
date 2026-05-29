import api from "@/lib/api";
import { PaginatedProducts, Product } from "@/types/product";
import { useEffect, useState } from "react";

export function useProducts(params?: string) {
  const [data, setData] = useState<PaginatedProducts>({
    products: [],
    page: 1,
    pageSize: 12,
    totalItems: 0,
    totalPages: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchProducts() {
      try {
        const response = await api.get(
          `/products${params ? `?${params}` : ""}`,
        );
        setData(response.data);
      } catch (error) {
        console.error("Failed to fetch the products", error);
      }
      setLoading(false);
    }
    fetchProducts();
  }, [params]);

  return { ...data, loading };
}
