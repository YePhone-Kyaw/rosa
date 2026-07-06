"use client";

import { useAuth } from "@/hooks/useAuth";
import { useStore } from "@/store/useStore";
import Link from "next/link";
import SearchBar from "./SearchBar";
import { Suspense, useEffect, useRef, useState } from "react";
import RosaLogo from "./RosaLog";

export default function Navbar() {
  const { user, cart } = useStore();
  const { logout } = useAuth();
  const [showDropdown, setShowDropdown] = useState(false);
  const [showMobileMenu, setShowMobileMenu] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 768) setShowMobileMenu(false);
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  return (
    <nav className="bg-white shadow-sm border-b sticky top-0 z-50">
      <div className="px-4 md:px-10 flex items-center justify-between h-16">
        <Link href="/" className="text-2xl font-bold text-rose-600">
          <RosaLogo />
        </Link>

        <div className="flex-1 max-w-xl mx-4">
          <Suspense>
            <SearchBar />
          </Suspense>
        </div>

        <div className="hidden md:flex items-center gap-4">
          {user ? (
            <>
              <Link href="/cart" className="relative">
                <span className="text-gray-600 hover:text-rose-600 transition">
                  🛒
                </span>
                {cart.length > 0 && (
                  <span className="absolute -top-2 -right-2 bg-rose-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                    {cart.length}
                  </span>
                )}
              </Link>
              <Link
                href="/orders"
                className="text-gray-600 hover:text-rose-600 transition text-sm"
              >
                Orders
              </Link>

              <div className="relative" ref={dropdownRef}>
                <button
                  onClick={() => setShowDropdown(!showDropdown)}
                  className="flex items-center gap-1 text-gray-600 hover:text-rose-600 transition"
                >
                  <span>{user.name}</span>
                  <span className="text-xs">▼</span>
                </button>

                {showDropdown && (
                  <div className="absolute right-0 top-full mt-2 w-48 bg-white border rounded-xl shadow-lg z-50">
                    <Link
                      href="/profile"
                      onClick={() => setShowDropdown(false)}
                      className="block px-4 py-3 text-sm text-gray-700 hover:bg-gray-50 rounded-t-xl"
                    >
                      Profile
                    </Link>
                    <Link
                      href="/orders"
                      onClick={() => setShowDropdown(false)}
                      className="block px-4 py-3 text-sm text-gray-700 hover:bg-gray-50"
                    >
                      My Orders
                    </Link>
                    <button
                      onClick={() => {
                        setShowDropdown(false);
                        logout();
                      }}
                      className="block w-full text-left px-4 py-3 text-sm text-red-600 hover:bg-gray-50 rounded-b-xl"
                    >
                      Logout
                    </button>
                  </div>
                )}
              </div>
            </>
          ) : (
            <div className="flex items-center gap-2">
              <Link
                href="/auth/login"
                className="text-gray-600 hover:text-rose-600 transition text-sm"
              >
                Login
              </Link>
              <Link
                href="/auth/register"
                className="bg-rose-600 text-white px-4 py-2 rounded-lg hover:bg-rose-700 transition text-sm"
              >
                Register
              </Link>
            </div>
          )}
        </div>

        <div className="flex md:hidden items-center gap-3">
          {user && (
            <Link href="/cart" className="relative">
              <span className="text-gray-600">🛒</span>
              {cart.length > 0 && (
                <span className="absolute -top-2 -right-2 bg-rose-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                  {cart.length}
                </span>
              )}
            </Link>
          )}
          <button
            onClick={() => setShowMobileMenu(!showMobileMenu)}
            className="text-gray-600 p-1"
          >
            {showMobileMenu ? (
              <svg
                className="w-6 h-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            ) : (
              <svg
                className="w-6 h-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M4 6h16M4 12h16M4 18h16"
                />
              </svg>
            )}
          </button>
        </div>
      </div>

      {showMobileMenu && (
        <div className="md:hidden border-t bg-white">
          {user ? (
            <div className="border-t">
              <div className="px-4 py-3 text-sm text-gray-500">
                Signed in as {user.name}
              </div>
              <Link
                href="/orders"
                onClick={() => setShowMobileMenu(false)}
                className="block px-4 py-3 text-sm text-gray-700 hover:bg-gray-50"
              >
                Orders
              </Link>
              <Link
                href="/profile"
                onClick={() => setShowMobileMenu(false)}
                className="block px-4 py-3 text-sm text-gray-700 hover:bg-gray-50"
              >
                Profile
              </Link>
              <button
                onClick={() => {
                  setShowMobileMenu(false);
                  logout();
                }}
                className="block w-full text-left px-4 py-3 text-sm text-red-600 hover:bg-gray-50"
              >
                Logout
              </button>
            </div>
          ) : (
            <div className="border-t px-4 py-3 flex flex-col gap-2">
              <Link
                href="/auth/login"
                onClick={() => setShowMobileMenu(false)}
                className="text-center py-2 text-gray-600 hover:text-rose-600 transition text-sm"
              >
                Login
              </Link>
              <Link
                href="/auth/register"
                onClick={() => setShowMobileMenu(false)}
                className="text-center bg-rose-600 text-white px-4 py-2 rounded-lg hover:bg-rose-700 transition text-sm"
              >
                Register
              </Link>
            </div>
          )}
        </div>
      )}
    </nav>
  );
}
