import { useStore } from "@/store/useStore";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export default function ProtectedRoute({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, authLoading } = useStore();
  const router = useRouter();

  useEffect(() => {
    if (!user && !authLoading) {
      router.push("/auth/login");
    }
  }, [user, router, authLoading]);

  if (authLoading) return;
  if (!user) return null;
  return <>{children}</>;
}
