"use client";

import GuestRoute from "@/components/GuestRoute";
import { useAuth } from "@/hooks/useAuth";
import { GoogleLogin } from "@react-oauth/google";
import axios from "axios";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";

export default function RegisterPage() {
  const router = useRouter();
  const { register, googleLogin } = useAuth();
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleRegister(formData: FormData) {
    setLoading(true);
    setError("");

    try {
      await register(
        formData.get("name") as string,
        formData.get("email") as string,
        formData.get("password") as string,
      );
      router.push("/");
    } catch (error) {
      if (axios.isAxiosError(error)) {
        setError(error.response?.data.message || "Register failed");
      }
    }
    setLoading(false);
  }
  return (
    <GuestRoute>
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="bg-white p-8 rounded-2xl shadow-sm border w-full max-w-md">
          <h1 className="text-2xl font-bold text-gray-900 mb-2">
            Welcome to Rosa
          </h1>
          <p className="text-gray-500 mb-6">Register your account</p>
          {error && (
            <div className="bg-red-50 text-red-600 px-4 py-3 rounded-lg mb-4 text-sm">
              {error}
            </div>
          )}
          <form action={handleRegister} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Name
              </label>
              <input
                type="text"
                name="name"
                className="w-full border text-black border-gray-300 rounded-lg px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-rose-500"
                placeholder="username"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Email
              </label>
              <input
                type="email"
                name="email"
                className="w-full border text-black border-gray-300 rounded-lg px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-rose-500"
                placeholder="you@email.com"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Password
              </label>
              <input
                type="password"
                name="password"
                className="w-full border text-black border-gray-300 rounded-lg px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-rose-500"
                placeholder="••••••••"
                required
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-rose-600 text-white py-2.5 rounded-lg font-medium hover:bg-rose-700 transition disabled:opacity-50"
            >
              {loading ? "Registering..." : "Register"}
            </button>
          </form>
          <div className="mt-4 flex items-center gap-4">
            <GoogleLogin
              onSuccess={async (response) => {
                await googleLogin(response.credential!);
                router.push("/");
              }}
              onError={() => setError("Google sign up failed")}
            />
            <button
              disabled
              className="w-full border border-gray-300 py-2.5 rounded-lg text-gray-400 cursor-not-allowed"
            >
              Continue with Facebook
            </button>
          </div>

          <p className="text-center text-sm text-gray-500 mt-6">
            Already have an account?{" "}
            <Link
              href="/auth/login"
              className="text-rose-600 font-medium hover:underline"
            >
              Login
            </Link>
          </p>
        </div>
      </div>
    </GuestRoute>
  );
}
