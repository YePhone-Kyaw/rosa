import { useCategories } from "@/hooks/useCategories";
import { useRouter, useSearchParams } from "next/navigation";

export default function FilterBar() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { categories } = useCategories();

  const categoryId = searchParams.get("categoryId") || "";
  const sortBy = searchParams.get("sortBy") || "";
  const order = searchParams.get("order") || "";

  const handleCategoryChange = (value: string) => {
    const params = new URLSearchParams(searchParams.toString());
    if (categoryId === value) {
      params.delete("categoryId");
    } else {
      params.set("categoryId", value);
    }
    params.delete("page");
    router.replace(`/products?${params.toString()}`);
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
      params.set("sortBy", "name");
      params.set("order", "asc");
    } else {
      params.delete("sortBy");
      params.delete("order");
    }
    params.delete("page");
    router.replace(`/products?${params.toString()}`);
  };

  const clearFilters = () => {
    router.push("/products");
  };

  return (
    <aside className="w-56 shrink-0">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-bold text-gray-900">Filters</h3>
        <button
          onClick={clearFilters}
          className="text-xs text-rose-600 hover:underline"
        >
          Clear all
        </button>
      </div>

      <div className="mb-6">
        <h4 className="font-medium text-gray-700 mb-3">Category</h4>
        {categories.map((cat) => (
          <label
            key={cat.categoryId}
            className="flex items-center gap-2 py-1.5 cursor-pointer"
          >
            <input
              type="checkbox"
              checked={categoryId === cat.categoryId.toString()}
              onChange={() => handleCategoryChange(cat.categoryId.toString())}
              className="accent-rose-600"
            />
            <span className="text-sm text-gray-900">{cat.categoryName}</span>
            <span className="text-xs text-gray-400 ml-auto">
              ({cat.productCount})
            </span>
          </label>
        ))}
      </div>

      <div>
        <h4 className="font-medium text-gray-700 mb-3">Sort By</h4>
        <select
          value={`${sortBy}-${order}`}
          onChange={(e) => handleSortChange(e.target.value)}
          className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm text-gray-900"
        >
          <option value="-">Latest</option>
          <option value="price-asc">Price: Low to High</option>
          <option value="price-desc">Price: High to Low</option>
          <option value="name-asc">Name: A-Z</option>
        </select>
      </div>
    </aside>
  );
}
