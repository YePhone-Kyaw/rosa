import { CartItem } from "@/types/cart";
import { User } from "@/types/user";
import { create } from "zustand";

interface Store {
  user: User | null;
  cart: CartItem[];
  setUser: (user: User | null) => void;
  setCart: (cart: CartItem[]) => void;
}

export const useStore = create<Store>((set) => ({
  user: null,
  cart: [],
  setUser: (user) => set({ user }),
  setCart: (cart) => set({ cart }),
}));
