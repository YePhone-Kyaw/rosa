"use client";

import { useInitAuth } from "@/hooks/useAuth";

export function AuthProvider({ children }: { children: React.ReactNode }) {
  useInitAuth();
  return <>{children}</>;
}
