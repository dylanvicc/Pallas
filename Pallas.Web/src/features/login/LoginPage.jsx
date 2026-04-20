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
                        className="w-full bg-blue-600 text-white py-2 mt-4 rounded-lg font-medium hover:bg-blue-700 transition"
                    >
                        Sign In
                    </button>
                </form>
            </div>
        </div>
    );
}