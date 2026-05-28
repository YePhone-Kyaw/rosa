import { CartItem } from "@/types/cart";
import { User } from "@/types/user";
import { create } from "zustand";

interface Store {
    user: User | null;
    token: string | null;
    cart: CartItem[];
    setUser: (user: User | null) => void;
    setToken: (token: string | null) => void;
    setCart: (cart: CartItem[]) => void;
    logout: () => void;
}

export const useStore = create<Store>((set) => ({
    user: null,
    token: null,
    cart: [],
    setUser: (user) => set({ user }),
    setToken: (token) => set({ token }),
    setCart: (cart) => set({ cart }),
    logout: () => {
        localStorage.removeItem('token');
        set({ user: null, token: null, cart: [] });
    }
}))
