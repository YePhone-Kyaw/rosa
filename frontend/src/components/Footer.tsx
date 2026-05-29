import Link from "next/link";

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-400 mt-20">
      <div className="max-w-7xl mx-auto px-4 py-12 grid grid-cols-1 md:grid-cols-4 gap-8">
        <div>
          <h3 className="text-white text-lg font-bold mb-4">Rosa</h3>
          <p className="text-sm">Discover amazing products at great prices.</p>
        </div>
        <div>
          <h4 className="text-white font-medium mb-4">Shop</h4>
          <div className="space-y-2 text-sm">
            <Link href="/products" className="block hover:text-white">
              All Products
            </Link>
            <Link
              href="/products?categoryId=1"
              className="block hover:text-white"
            >
              Electronics
            </Link>
            <Link
              href="/products?categoryId=2"
              className="block hover:text-white"
            >
              Furniture
            </Link>
          </div>
        </div>
        <div>
          <h4 className="text-white font-medium mb-4">Account</h4>
          <div className="space-y-2 text-sm">
            <Link href="/auth/login" className="block hover:text-white">
              Login
            </Link>
            <Link href="/auth/register" className="block hover:text-white">
              Register
            </Link>
            <Link href="/profile" className="block hover:text-white">
              Profile
            </Link>
            <Link href="/orders" className="block hover:text-white">
              Orders
            </Link>
          </div>
        </div>
        <div>
          <h4 className="text-white font-medium mb-4">Contact</h4>
          <div className="space-y-2 text-sm">
            <p>support@rosa.com</p>
            <p>Calgary, AB, Canada</p>
          </div>
        </div>
      </div>
      <div className="border-t border-gray-800 py-6 text-center text-sm">
        © {new Date().getFullYear()} Rosa. All rights reserved.
      </div>
    </footer>
  );
}
