import { createContext, useState } from "react";

const API_BASE = "http://localhost:5170/api";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [token, setToken] = useState(() => localStorage.getItem("token"));
    const [user, setUser] = useState(() => {
        const saved = localStorage.getItem("user");
        return saved ? JSON.parse(saved) : null;
    });
    const [loading, setLoading] = useState(false);

    const login = async (identifier, password) => {
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE}/auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ identifier, password }),
            });

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || "Credenciais inválidas.");
            }

            const data = await response.json();
            localStorage.setItem("token", data.token);
            localStorage.setItem("user", JSON.stringify(data.user));

            setToken(data.token);
            setUser(data.user);
            return data.user;
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, token, login, logout, loading }}>
            {children}
        </AuthContext.Provider>
    );
}

export { AuthContext };