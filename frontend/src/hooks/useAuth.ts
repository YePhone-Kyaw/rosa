import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export function useAuth() {
  const router = useRouter();
  const { setUser, setCart } = useStore();

  const login = async (email: string, password: string) => {
    await api.post("/auth/login", { email, password });
    const profileResponse = await api.get("/user/profile");
    setUser(profileResponse.data);
    const cartResponse = await api.get("/cart");
    if (cartResponse.data.cartItems) {
      setCart(cartResponse.data.cartItems);
    }
    return profileResponse.data;
  };

  const register = async (name: string, email: string, password: string) => {
    await api.post("/auth/register", {
      name,
      email,
      password,
    });
    const profileResponse = await api.get("/user/profile");
    setUser(profileResponse.data);
    return profileResponse.data;
  };

  const logout = async () => {
    await api.post("/auth/logout");
    setUser(null);
    setCart([]);
    router.push("/");
  };

  return { login, register, logout };
}

export function useInitAuth() {
  const { setUser, setCart, setAuthLoading } = useStore();

  useEffect(() => {
    api
      .get("/user/profile")
      .then(async (response) => {
        setUser(response.data);
        const cartResponse = await api.get("/cart");
        if (cartResponse.data.cartItems) {
          setCart(cartResponse.data.cartItems);
        }
      })
      .catch(() => {})
      .finally(() => setAuthLoading(false));
  }, [setUser, setCart, setAuthLoading]);
}
