import { CartItem } from "@/types/cart";
import { User } from "@/types/user";
import { create } from "zustand";

interface Store {
  user: User | null;
  cart: CartItem[];
  authLoading: boolean;
  setUser: (user: User | null) => void;
  setCart: (cart: CartItem[]) => void;
  setAuthLoading: (loading: boolean) => void;
}

export const useStore = create<Store>((set) => ({
  user: null,
  cart: [],
  authLoading: true,
  setUser: (user) => set({ user }),
  setCart: (cart) => set({ cart }),
  setAuthLoading: (loading) => set({ authLoading: loading }),
}));
