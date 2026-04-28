import { useState } from "react";
import { useAuthentication } from "./AuthenticationContext";
import { api } from "../../lib/api";

export default function LoginPage() {

    const { login } = useAuthentication();

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const submit = async (event) => {

        event.preventDefault();
        setError("");

        const res = await api("/api/authenticate/token", {
            method: "POST",
            body: JSON.stringify({ username, password })
        });

        if (!res.ok) {
            setError("Invalid username or password");
            return;
        }

        const data = await res.json();
        login(data.token);

        window.location.href = "/inventory";
    };

    return (
        <div className="flex items-center justify-center min-h-[70vh]">
            <div className="w-full max-w-md bg-gray-800 p-8 rounded-xl shadow border border-gray-700">

                <div className="flex justify-center mb-6 gap-4 flex-wrap">
                    <div className="w-10 h-10">
                        <svg viewBox="0 0 40 40" fill="none">
                            <rect width="40" height="40" rx="8" fill="#0f172a" />
                            <rect x="8" y="8" width="6" height="6" fill="#94a3b8" />
                            <rect x="16" y="8" width="6" height="6" fill="#94a3b8" />
                            <rect x="24" y="8" width="6" height="6" fill="#6366f1" />
                            <rect x="8" y="16" width="6" height="6" fill="#94a3b8" />
                            <rect x="16" y="16" width="6" height="6" fill="#6366f1" />
                            <rect x="8" y="24" width="6" height="6" fill="#94a3b8" />
                        </svg>
                    </div>
                </div>

                <h2 className="text-2xl font-bold mb-6 text-center">Welcome Back</h2>

                <form onSubmit={submit} className="space-y-5">
                    <div>
                        <label className="block text-sm font-medium mb-1">Username</label>
                        <input
                            type="text"
                            className="w-full px-4 py-2 bg-gray-900 border border-gray-700 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                            value={username}
                            onChange={(event) => setUsername(event.target.value)}
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium mb-1">Password</label>
                        <input
                            type="password"
                            className="w-full px-4 py-2 bg-gray-900 border border-gray-700 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                            value={password}
                            onChange={(event) => setPassword(event.target.value)}
                        />
                    </div>

                    {error && (
                        <div className="text-red-400 text-sm font-medium">{error}</div>
                    )}

                    <button
                        type="submit"
                        className="w-full bg-blue-600 text-white py-2.5 rounded-lg font-medium hover:bg-blue-700 transition active:scale-[0.99]"
                    >
                        Sign In
                    </button>
                    <p className="text-center text-sm text-gray-400">
                        <a
                            href="/forgot-password"
                            className="hover:text-gray-200 underline underline-offset-2"
                        >
                            Forgot your password?
                        </a>
                    </p>
                </form>
            </div>
        </div>
    );
}