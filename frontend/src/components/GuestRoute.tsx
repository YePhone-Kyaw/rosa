import { useStore } from "@/store/useStore";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export default function GuestRoute({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user } = useStore();
  const router = useRouter();

  useEffect(() => {
    if (user) {
      router.push("/");
    }
  }, [user, router]);

  if (user) return null;
  return <>{children}</>;
}
