import api from "@/lib/api";
import { Product } from "@/types/product";
import { useRouter } from "next/navigation";
import React, { useEffect, useRef, useState } from "react";

export default function SearchBar() {
  const [input, setInput] = useState("");
  const [suggestions, setSuggestions] = useState<Product[]>([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const [activeIndex, setActiveIndex] = useState(-1);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const router = useRouter();

  // Debounced search
  useEffect(() => {
    const fetchSuggestions = async () => {
      if (input.length < 1) {
        setSuggestions([]);
        setShowDropdown(false);
        return;
      }
      try {
        const response = await api.get(`/products?search=${input}&pageSize=12`);
        setSuggestions(response.data.products);
        setShowDropdown(true);
        setActiveIndex(-1);
      } catch {
        setSuggestions([]);
      }
    };
    fetchSuggestions();
  }, [input]);

  //   Close dropdown when clicking outside
  useEffect(() => {
    const handleClick = (e: MouseEvent) => {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(e.target as Node)
      ) {
        setInput("");
        setShowDropdown(false);
        setActiveIndex(-1);
      }
    };
    document.addEventListener("click", handleClick);
    return () => document.removeEventListener("click", handleClick);
  }, []);

  const handleSelect = (productName: string) => {
    setInput(productName);
    setSuggestions([]);
    setShowDropdown(false);
    router.push(`/products?search=${productName}`);
  };

  const handleSubmit = () => {
    setSuggestions([]);
    setShowDropdown(false);
    router.push(`/products?search=${input}`);
  };

  const handleKeys = (e: React.KeyboardEvent) => {
    if (!showDropdown || suggestions.length === 0) {
      if (e.key === "Enter") handleSubmit();
      return;
    }

    if (e.key === "ArrowDown") {
      e.preventDefault();
      setActiveIndex((prev) => (prev < suggestions.length - 1 ? prev + 1 : 0));
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      setActiveIndex((prev) => (prev > 0 ? prev - 1 : suggestions.length - 1));
    } else if (e.key === "Enter") {
      e.preventDefault();
      if (activeIndex >= 0 && suggestions[activeIndex]) {
        handleSelect(suggestions[activeIndex].productName);
      } else {
        handleSubmit();
      }
    }
  };

  return (
    <div className="relative flex-1" ref={dropdownRef}>
      <div className="flex gap-2">
        <input
          type="text"
          placeholder="Search products..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={handleKeys}
          className="w-full border border-gray-300 rounded-lg px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-rose-500 text-gray-900"
        />
      </div>
      {showDropdown && suggestions.length > 0 && (
        <div className="absolute top-full left-0 right-0 mt-1 bg-white text-black border rounded-lg shadow-lg z-50">
          {suggestions.map((product, index) => (
            <button
              key={product.productId}
              onClick={() => handleSelect(product.productName)}
              className={`w-full text-left px-4 py-3 text-gray-900 border-b last:border-b-0 ${index === activeIndex ? "bg-gray-100" : "hover:bg-gray-50"}`}
            >
              {product.productName}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
