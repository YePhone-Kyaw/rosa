export type CartItem = {
  cartItemId: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
};

export type Cart = {
  cartId: number;
  cartItems: CartItem[];
  totalAmount: number;
};
