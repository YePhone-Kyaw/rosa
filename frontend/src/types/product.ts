export type Product = {
  productId: number;
  productName: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string;
  categoryName: string;
  createdAt: string;
};

export type PaginatedProducts = {
  products: Product[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};
