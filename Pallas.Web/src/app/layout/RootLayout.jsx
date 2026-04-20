import { useState } from "react";
import { AuthenticationProvider, useAuthentication } from "../../features/login/AuthenticationContext";

function Navigation() {

    const { authentication, logout } = useAuthentication();

    return (
        <>
            <nav className="hidden md:flex gap-6 text-sm">
                {authentication && (
                    <>
                        <a className="hover:text-blue-400" href="/location">Location</a>
                        <a className="hover:text-blue-400" href="/item">Item</a>
                        <a className="hover:text-blue-400" href="/inventory">Inventory</a>
                        <button
                            onClick={() => { logout(); window.location.href = "/login"; }}
                            className="hover:text-red-400"
                        >
                            Logout
                        </button>
                    </>
                )}

                {!authentication && (
                    <a className="hover:text-blue-400" href="/login">Login</a>
                )}
            </nav>
        </>
    );
}

export default function RootLayout({ children }) {

    const [menuOpen, setMenuOpen] = useState(false);

    return (
        <AuthenticationProvider>
            <div className="min-h-screen bg-gray-900 text-gray-100">
                <header className="bg-gray-800 border-b border-gray-700">
                    <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
                        <a className="text-xl font-semibold" href="/">Pallas</a>

                        <Navigation />

                        <button
                            className="md:hidden p-2 rounded-lg hover:bg-gray-700 transition"
                            onClick={() => setMenuOpen(!menuOpen)}
                        >
                            <span className="text-lg">Menu</span>
                        </button>
                    </div>

                    {menuOpen && (
                        <nav className="md:hidden px-6 pb-4 flex flex-col gap-3 text-sm bg-gray-800 border-t border-gray-700">
                            <Navigation />
                        </nav>
                    )}
                </header>

                <main className="max-w-6xl mx-auto px-6 py-10">
                    {children}
                </main>
            </div>
        </AuthenticationProvider>
    );
}
