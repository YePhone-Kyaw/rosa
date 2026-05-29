import api from "@/lib/api";
import { useStore } from "@/store/useStore";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export function useAuth() {
  const router = useRouter();
  const { setUser, setCart } = useStore();

  const login = async (email: string, password: string) => {
    const response = await api.post("/auth/login", { email, password });
    setUser(response.data);
    return response.data;
  };

  const register = async (name: string, email: string, password: string) => {
    const response = await api.post("/auth/register", {
      name,
      email,
      password,
    });
    setUser(response.data);
    return response.data;
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
  const { setUser } = useStore();

  useEffect(() => {
    api
      .get("/user/profile")
      .then((response) => setUser(response.data))
      .catch(() => {});
  }, [setUser]);
}
