"use client";

import { useAuth } from "@/hooks/useAuth";
import { useStore } from "@/store/useStore";
import Link from "next/link";
import SearchBar from "./SearchBar";
import { useRef, useState } from "react";

export default function Navbar() {
  const { user, cart } = useStore();
  const { logout } = useAuth();
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  return (
    <nav className="bg-white shadow-sm border-b sticky top-0 z-50">
      <div className="px-10 flex items-center justify-between h-16">
        <Link href="/" className="text-2xl font-bold text-rose-600">
          Rosa
        </Link>

        <div className="flex-1 max-w-xl">
          <SearchBar />
        </div>

        <div className="flex items-center gap-4">
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
      </div>
    </nav>
  );
}
