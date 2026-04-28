import { api, handleApiResponse } from "../lib/api";

export const authService = {

    async login(username, password) {
        const response = await api("/api/authenticate/login", {
            method: "POST",
            body: JSON.stringify({ username, password })
        });

        return handleApiResponse(response);
    }
};
