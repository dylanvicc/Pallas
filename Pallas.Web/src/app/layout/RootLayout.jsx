import { useState } from "react";

export default function RootLayout({ children }) {

    const [menuOpen, setMenuOpen] = useState(false);

    return (
        <div className="min-h-screen bg-gray-900 text-gray-100">
            <header className="bg-gray-800 border-b border-gray-700">
                <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
                    <h1 className="text-xl font-semibold">Pallas</h1>
                    <nav className="hidden md:flex gap-6 text-sm">
                        <a className="hover:text-blue-400" href="/">Home</a>
                        <a className="hover:text-blue-400" href="/inventory">Inventory</a>
                        <a className="hover:text-blue-400" href="/login">Login</a>
                    </nav>
                    <button
                        className="md:hidden p-2 rounded-lg hover:bg-gray-700 transition"
                        onClick={() => setMenuOpen(!menuOpen)}
                    >
                        <span className="text-lg">Menu</span>
                    </button>
                </div>
                {menuOpen && (
                    <nav className="md:hidden px-6 pb-4 flex flex-col gap-3 text-sm bg-gray-800 border-t border-gray-700">
                        <a className="hover:text-blue-400 mt-4" href="/">Home</a>
                        <a className="hover:text-blue-400" href="/inventory">Inventory</a>
                        <a className="hover:text-blue-400" href="/login">Login</a>
                    </nav>
                )}
            </header>
            <main className="max-w-6xl mx-auto px-6 py-10">
                {children}
            </main>
        </div>
    );
}
