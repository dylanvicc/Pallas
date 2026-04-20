import { createContext, useContext, useState } from "react";

const AuthenticationContext = createContext(null);

export function AuthenticationProvider({ children }) {

    const [authentication, setAuthentication] = useState(() => {
        const token = localStorage.getItem("token");
        return token ? { token } : null;
    });

    const login = (token) => {
        localStorage.setItem("token", token);
        setAuthentication({ token });
    };

    const logout = () => {
        localStorage.removeItem("token");
        setAuthentication(null);
    };

    return (
        <AuthenticationContext.Provider value={{ authentication, login, logout }}>
            {children}
        </AuthenticationContext.Provider>
    );
}

export function useAuthentication() {
    return useContext(AuthenticationContext);
}