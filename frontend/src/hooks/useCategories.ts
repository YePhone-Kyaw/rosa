import api from "@/lib/api";
import { Category } from "@/types/category";
import { useEffect, useState } from "react";

export function useCategories() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function fetchCategories() {
            try {
                const response = await api.get('/categories');
                setCategories(response.data);
            } catch (error) {
                console.error('Failed to fetch categories', error);
            }
            setLoading(false);
        }
        fetchCategories();
    }, []);

    return { categories, loading }
}
