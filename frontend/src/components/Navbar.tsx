'use client'

import { useStore } from "@/store/useStore"
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function Navbar() {
    const { user, cart, logout } = useStore();
    const router = useRouter();

    const handleLogout = () => {
        logout();
        router.push("/");
    }

    return (
         <nav className="bg-white shadow-sm border-b sticky top-0 z-50">
            <div className="max-w-7xl mx-auto px-4 flex items-center justify-between h-16">
                
                <Link href="/" className="text-2xl font-bold text-rose-600">
                    Rosa
                </Link>

                <div className="hidden md:flex items-center gap-6">
                    <Link href="/products" className="text-gray-600 hover:text-rose-600 transition">
                        Products
                    </Link>
                </div>

                <div className="flex items-center gap-4">
                    
                    <Link href="/cart" className="relative">
                        <span className="text-gray-600 hover:text-rose-600 transition">🛒</span>
                        {cart.length > 0 && (
                            <span className="absolute -top-2 -right-2 bg-rose-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                {cart.length}
                            </span>
                        )}
                    </Link>

                    {/* Auth */}
                    {user ? (
                        <div className="flex items-center gap-3">
                            <Link href="/profile" className="text-gray-600 hover:text-rose-600 transition">
                                {user.name}
                            </Link>
                            <button
                                onClick={handleLogout}
                                className="bg-rose-600 text-white px-4 py-2 rounded-lg hover:bg-rose-700 transition text-sm">
                                Logout
                            </button>
                        </div>
                    ) : (
                        <div className="flex items-center gap-2">
                            <Link href="/auth/login"
                                className="text-gray-600 hover:text-rose-600 transition text-sm">
                                Login
                            </Link>
                            <Link href="/auth/register"
                                className="bg-rose-600 text-white px-4 py-2 rounded-lg hover:bg-rose-700 transition text-sm">
                                Register
                            </Link>
                        </div>
                    )}
                </div>
            </div>
        </nav>
    )
};
