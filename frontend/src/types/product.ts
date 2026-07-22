export type ProductImage = {
  productImageId: number;
  productImageUrl: string;
  displayOrder: number;
}

export type Product = {
  productId: number;
  productName: string;
  description: string;
  price: number;
  stock: number;
  images: ProductImage[];
  categoryName: string;
  createdAt: string;
};
