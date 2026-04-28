import { api, handleApiResponse } from "../lib/api";

const LOCATIONS_API = "/api/locations";

export const locationService = {

    async getLocations(params = {}) {
        const query = new URLSearchParams({
            skip: params.skip ?? 0,
            take: params.take ?? 20
        });

        const response = await api(`${LOCATIONS_API}?${query}`);
        return handleApiResponse(response);
    },

    async getLocation(id) {
        const response = await api(`${LOCATIONS_API}/${id}`);
        return handleApiResponse(response);
    },

    async createLocation(location) {
        const response = await api(LOCATIONS_API, {
            method: "POST",
            body: JSON.stringify(location)
        });
        return handleApiResponse(response);
    },

    async updateLocation(id, location) {
        const response = await api(`${LOCATIONS_API}/${id}`, {
            method: "PUT",
            body: JSON.stringify(location)
        });
        return handleApiResponse(response);
    },

    async deleteLocation(id) {
        const response = await api(`${LOCATIONS_API}/${id}`, {
            method: "DELETE"
        });
        return handleApiResponse(response);
    }
};
