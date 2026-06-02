export type OrderItem = {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
};

export type Order = {
  orderId: number;
  status: string;
  totalAmount: number;
  items: OrderItem[];
  createdAt: string;
};
