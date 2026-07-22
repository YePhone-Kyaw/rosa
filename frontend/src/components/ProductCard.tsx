import { Product } from "@/types/product";
import Image from "next/image";
import Link from "next/link";

export default function ProductCard({ product }: { product: Product }) {
  const mainImage = product.images?.[0]?.productImageUrl;
  
  return (
    <Link
      href={`/products/${product.productId}`}
      className="bg-white border rounded-xl overflow-hidden hover:shadow-md transition"
    >
      <div className="relative aspect-square bg-gray-100 flex items-center justify-center">
        {mainImage ? (
          <Image 
            src={mainImage}
            alt={product.productName}
            fill
            className="object-cover"
          />
        ) : (
          <span className="text-gray-400 text-4xl">📦</span>
        )}
      </div>
      <div className="p-4">
        <p className="text-xs text-rose-600 mb-1">{product.categoryName}</p>
        <h3 className="font-medium text-gray-900 mb-1">
          {product.productName}
        </h3>
        <p className="text-lg font-bold text-gray-900">
          ${product.price.toFixed(2)}
        </p>
      </div>
    </Link>
  );
}
