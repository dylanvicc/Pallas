export default function LoginPage() {
    return (
        <div className="flex items-center justify-center min-h-[70vh]">
            <div className="w-full max-w-md bg-gray-800 p-8 rounded-xl shadow border border-gray-700">
                <h2 className="text-2xl font-bold mb-6 text-center">Welcome Back</h2>
                <form className="space-y-5">
                    <div>
                        <label className="block text-sm font-medium mb-1">Email</label>
                        <input
                            type="email"
                            className="w-full px-4 py-2 bg-gray-900 border border-gray-700 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                            placeholder=""
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Password</label>
                        <input
                            type="password"
                            className="w-full px-4 py-2 bg-gray-900 border border-gray-700 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                            placeholder=""
                        />
                    </div>
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

