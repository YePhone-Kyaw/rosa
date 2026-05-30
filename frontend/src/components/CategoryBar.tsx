"use client";

import { useCategories } from "@/hooks/useCategories";
import Link from "next/link";
import { useSearchParams } from "next/navigation";

export default function CategoryBar() {
  const { categories } = useCategories();
  const searchParams = useSearchParams();
  const activeCategoryId = searchParams.get('categoryId') || '';

  return (
    <div className="bg-gray-900 text-white sticky top-16 z-40">
      <div className="px-10 flex items-center gap-6 h-10 overflow-x-auto">
        <div className="flex gap-6">    
        <div className="flex items-center gap-6">
          <Link
            href="/products"
            className={`text-sm whitespace-nowrap hover:text-rose-400 transition ${
              !activeCategoryId ? "text-rose-400 font-medium" : ""
            }`}
          >
            All
          </Link>
          <span className="text-gray-400">|</span>
        </div>
        {categories.map((category, index) => (
          <div key={category.categoryId} className="flex items-center gap-6">
            <Link
              href={`/products?categoryId=${category.categoryId}`}
              className={`text-sm whitespace-nowrap hover:text-rose-400 transition ${
                activeCategoryId === category.categoryId.toString()
                  ? "text-rose-400 font-medium"
                  : ""
              }`}
            >
              {category.categoryName}
            </Link>
            {index < categories.length - 1 && (
              <span className="text-gray-400">|</span>
            )}
          </div>
        ))}
        </div>
      </div>
    </div>
  );
}
